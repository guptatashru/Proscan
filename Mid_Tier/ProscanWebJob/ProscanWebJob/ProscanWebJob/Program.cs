using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Net;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace ProscanWebJob
{
    class Program
    {
        private static ProScanWebJobModel db = new ProScanWebJobModel();
        static void Main(string[] args)
        {
            Task t = MainAsync(args);
            t.Wait();
        }

        static async Task MainAsync(string[] args)
        {
            string accountName = "proscanstorage";
            string accountkey = "56sL+T7n+t7xH3I7/zU1/JSDpzJO6WsaUxDfxP8Ji5OJJJCYP9gNHkS06OD+wk5nj0YX1GIOUl6SC4qh4FrvNA==";

            StorageCredentials credentails = new StorageCredentials(accountName, accountkey);
            CloudStorageAccount account = new CloudStorageAccount(credentails, true);
            CloudQueueClient queueCLient = account.CreateCloudQueueClient();
            CloudQueue queue = queueCLient.GetQueueReference("proscanqueue");
            bool isQueueExist = queue.CreateIfNotExists();

            int currentBackoff = 0;
            int maximumBackoff = 10;

            while (true)
            {
                try
                {
                    // Get the next message
                    CloudQueueMessage retrievedMessage = queue.GetMessage();
                    //var retrievedMessage = "1&45DEAAB4-DFCC-4851-917A-83FE6102C1F5";
                    if (retrievedMessage != null)
                    {
                        //var retrievedMessage = "3&45DEAAB4-DFCC-4851-917A-83FE6102C1F5";
                        currentBackoff = 0;

                        int batchId;
                        string retailerId;
                        string[] batchRetailer = retrievedMessage.AsString.Split('&');
                        //string[] batchRetailer = peekedMessage.AsString.Split('&');
                        batchId = Convert.ToInt32(batchRetailer[0]);
                        retailerId = batchRetailer[1];
                        string agentId;

                        var scanResults = db.ScanResults.Where(s => s.BatchID == batchId && s.RetailerID == retailerId);

                        agentId = scanResults.FirstOrDefault().AgentID;
                        var count = scanResults.Count();
                        Product product = null;
                        foreach (var scanResult in scanResults)
                        {
                            //Console.WriteLine(scanResult.BarcodeValue);
                            product = db.Products.FirstOrDefault(p => p.EAN_UPC == scanResult.BarcodeValue);
                            var countryCode = GetCountryCode(Convert.ToDecimal(scanResult.ScanLocationLat), Convert.ToDecimal(scanResult.ScanLocationLong));
                            if (product != null) // Product found
                            {
                                if (product.CountryCode.ToLower() == countryCode.ToLower()) //valid based on country code match
                                {
                                    scanResult.isValid = true;
                                    scanResult.isProcessed = true;
                                    scanResult.isAgentNotified = false;
                                    scanResult.ProductID = product.Id;
                                    scanResult.ValidationResponse = "Valid Product code";
                                    scanResult.CountryCode = countryCode;
                                }
                                else
                                {
                                    scanResult.isValid = false;
                                    scanResult.isProcessed = true;
                                    scanResult.isAgentNotified = true;
                                    scanResult.ProductID = product.Id;
                                    scanResult.ValidationResponse = "Invalid Country Code – Product Location Code: " + product.CountryCode + " , Scan Location: " + countryCode;
                                    scanResult.CountryCode = countryCode;

                                    var tag = GetAgentTag(agentId);
                                    var message = "Invalid Country code scanned by retailer " + GetRetilerName(retailerId) + "  – Product Location Code: " + product.CountryCode + ", Scan Location: " + countryCode + " Product Desc: " + product.description + " , Barcode: " + scanResult.BarcodeValue;
                                    await SendPushNotificationToUser(tag, message);
                                }
                            }
                            else //Product Not found
                            {
                                scanResult.isValid = false;
                                scanResult.isProcessed = true;
                                scanResult.isAgentNotified = true;
                                var response = (scanResult.BarcodeValue == 999999999) ? "Invalid Product Code - Non Numeric" : "Invalid Product Code";
                                scanResult.ValidationResponse = response;
                                scanResult.CountryCode = countryCode;
                                var tag = GetAgentTag(agentId);
                                //Console.WriteLine(tag);
                                //Console.WriteLine(scanResult.BarcodeValue);
                                //Console.WriteLine(GetRetilerName(retailerId));
                                var message = "Invalid Product: " + scanResult.BarcodeValue + " scanned by retailer: " + GetRetilerName(retailerId);
                                await SendPushNotificationToUser(tag, message);
                            }
                        }

                        db.SaveChanges();

                        // Send Push notification to User retailer
                        var retailerTag = GetRetailerTag(retailerId);
                        var finalMessage = "Validation completed for " + count + " Products for Batch Id: " + batchId + ". Please sync to get latest update.";
                        await SendPushNotificationToUser(retailerTag, finalMessage);

                        queue.DeleteMessage(retrievedMessage);
                    }
                    else
                    {
                        if (currentBackoff < maximumBackoff)
                        {
                            currentBackoff++;
                        }

                        Thread.Sleep(TimeSpan.FromSeconds(currentBackoff));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }

        private static string GetAgentTag(string id)
        {
            var tag = string.Empty;
            var agent = db.Agents.FirstOrDefault(a => a.Id == id);
            if (agent != null)
            {
                tag = agent.PNSID;
            }
            return tag;
        }

        private static string GetRetailerTag(string id)
        {
            var tag = string.Empty;
            var retailer = db.Retailers.FirstOrDefault(a => a.Id == id);
            if (retailer != null)
            {
                tag = retailer.PNSID;
            }
            return tag;
        }

        private static string GetRetilerName(string id)
        {
            var name = string.Empty;
            var retailer = db.Retailers.FirstOrDefault(a => a.Id == id);
            if (retailer != null)
            {
                name = retailer.Name;
            }
            return name;
        }

        private static async Task<string> SendPushNotificationToUser(string uniqueTag, string message)
        {
            string[] userTag = new string[1];
            userTag[0] = uniqueTag;

            Microsoft.Azure.NotificationHubs.NotificationOutcome outcome = null;
            HttpStatusCode ret = HttpStatusCode.InternalServerError;

            //We only have android devices as of now. 
            //If other platforms are targeted, update code with switch for other platform                       
            var notification = "{ \"data\" : {\"message\":\"" + message + "\"}}";
            outcome = await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notification, userTag);

            if (outcome != null)
            {
                if (!((outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Abandoned) ||
                    (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Unknown)))
                {
                    ret = HttpStatusCode.OK;
                }
            }

            return ret.ToString();
        }

        internal static string GetCountryCode(decimal latitude, decimal longitude)
        {
            //Get Country code from google api and validate against the product code
            var strLatitude = latitude.ToString();
            var strLongitude = longitude.ToString();

            WebClient client = new WebClient();
            string Url = "http://maps.googleapis.com/maps/api/geocode/json?latlng=" + strLatitude + "," + strLongitude + "&sensor=true";
            var jsonResponse = client.DownloadString(new Uri(Url, UriKind.RelativeOrAbsolute));

            JObject parseJson = JObject.Parse(jsonResponse);
            var getJsonres = parseJson["results"][0];

            var countryCode = string.Empty;

            int count = 0;
            while (count >= 0)
            {
                var addressInfo = getJsonres["address_components"][count];
                var addressType = addressInfo["types"];
                if (addressType.ToString().Contains("country"))
                {
                    countryCode = addressInfo["short_name"].ToString();
                    break;
                }
                count++;
            }

            return countryCode;
        }
    }
}
