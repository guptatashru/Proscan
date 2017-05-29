using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using SyngentaProscan.DataObjects;
using SyngentaProscan.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


namespace SyngentaProscan.Controllers
{
    public class UserController : ApiController
    {
        private MobileServiceContext db = new MobileServiceContext();

        string accountName = "proscanstorage";
        string accountkey = "56sL+T7n+t7xH3I7/zU1/JSDpzJO6WsaUxDfxP8Ji5OJJJCYP9gNHkS06OD+wk5nj0YX1GIOUl6SC4qh4FrvNA==";

        // GET: api/User
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/User/5
        public async Task<IHttpActionResult> Get(string email)
        {
            var authority = "https://login.windows.net/syngentaB2C.onmicrosoft.com";
            Uri graphUri = new Uri("https://graph.windows.net");
            Uri serviceRoot = new Uri(graphUri, "syngentaB2C.onmicrosoft.com");
            ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot, async () =>
            {
                AuthenticationContext ctx = new AuthenticationContext(authority);
                ClientCredential clientCred = new ClientCredential("74aff4b6-1e45-4998-ac44-27b151045544", "173ZqmBqQemyszufkGs1TDD8XM9Ml51tKRoX383WkqQ=");
                AuthenticationResult auth = await ctx.AcquireTokenAsync(graphUri.ToString(), clientCred);
                var token = auth.AccessToken;
                return token;
            });
            var allActiveUsers = activeDirectoryClient.Users.ExecuteAsync().Result.CurrentPage.ToArray();

            var myUser = allActiveUsers.FirstOrDefault(a => a.UserPrincipalName == email);

            var retailer = await db.Retailers.FirstOrDefaultAsync(a => a.LoginID.ToLower() == email.ToLower());
            var user = new SyngentaUser() { Role = myUser.JobTitle, Email = myUser.UserPrincipalName };
            if (retailer != null)
            {
                user.AgentId = retailer.AgentID;
                user.RetailerId = retailer.Id;
            }

            return Ok(user);
        }

        // POST: api/User
        public async Task<IHttpActionResult> Post(List<BatchScan> scanResults)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                int batchId = 0;
                string retailerId = null;

                foreach (var scanResult in scanResults)
                {
                    long ean_upc;
                    long.TryParse(scanResult.BarcodeValue, out ean_upc);
                    if (ean_upc == 0)
                    {
                        ean_upc = 999999999;
                    }

                    //Create new scan object to add in table

                    var scanObject = new ScanResult()
                    {
                        Id = Guid.NewGuid().ToString(),
                        BarcodeValue = ean_upc,
                        AgentID = scanResult.AgentID,
                        RetailerID = scanResult.RetailerID,
                        ValidationResponse = scanResult.ValidationResponse,
                        isValid = false,
                        isAgentNotified = false,
                        ScanDate = scanResult.ScanDate,
                        ScanLocationLat = scanResult.ScanLocationLat,
                        ScanLocationLong = scanResult.ScanLocationLong,
                        BatchID = scanResult.BatchID,
                        isProcessed = false,
                    };

                    db.ScanResults.Add(scanObject);
                    if (batchId == 0)
                    {
                        batchId = Convert.ToInt32(scanResult.BatchID);
                    }
                    if (retailerId == null)
                    {
                        retailerId = scanResult.RetailerID;
                    }
                }

                try
                {
                    await db.SaveChangesAsync();

                    //Insert Message in Queue
                    StorageCredentials credentails = new StorageCredentials(accountName, accountkey);
                    CloudStorageAccount account = new CloudStorageAccount(credentails, true);
                    CloudQueueClient queueCLient = account.CreateCloudQueueClient();
                    CloudQueue queue = queueCLient.GetQueueReference("proscanqueue");
                    queue.CreateIfNotExists();

                    CloudQueueMessage message = new CloudQueueMessage(batchId + "&" + retailerId);
                    queue.AddMessage(message);
                }
                catch (DbUpdateException dbex)
                {
                    return Ok(dbex.Message);
                }

                return Ok("Scan table successfully updated");

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        // PUT: api/User/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/User/5
        public void Delete(int id)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
