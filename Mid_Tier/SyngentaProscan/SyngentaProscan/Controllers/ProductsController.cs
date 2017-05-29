using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SyngentaProscan.DataObjects;
using SyngentaProscan.Models;

namespace SyngentaProscan.Controllers
{
    //[Route("api/Products")]
    public class ProductsController : ApiController
    {
        private MobileServiceContext db = new MobileServiceContext();

        // GET: api/Products
        public IQueryable<Product> GetProducts()
        {
            return db.Products;
        }

        // GET: api/Products/5
        //[Route("api/Products/5")]
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProduct(string id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);

        }

        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProductByBarcode(string id, decimal latitude, decimal longitude, string retailerId)
        {
            try
            {
                return await ProcessScanBarcode(id, latitude, longitude, retailerId);

            }
            catch (Exception ex)
            {
                var excep = ex.Message;
                return Ok(excep);
            }
        }

        private async Task<IHttpActionResult> ProcessScanBarcode(string id, decimal latitude, decimal longitude, string retailerId)
        {
            long ean_upc;
            long.TryParse(id, out ean_upc);
            if (ean_upc == 0)
            {
                ean_upc = 999999999; 
            }

            var product = await db.Products.FirstOrDefaultAsync(i => i.EAN_UPC == ean_upc);
            var retailer = await db.Retailers.FirstOrDefaultAsync(r => r.Id == retailerId);
            var countryCode = Common.GetCountryCode(latitude, longitude);
            var notifcationMessage = string.Empty;
            var isValidProduct = true;

            if (product == null)
            {
                var failScanResult = new ScanResult
                {
                    Id = Guid.NewGuid().ToString(),
                    BarcodeValue = ean_upc,
                    isProcessed = true,
                    ScanDate = DateTime.Now,
                    ScanLocationLat = latitude,
                    ScanLocationLong = longitude,
                    isAgentNotified = true,
                    isValid = false,
                    CountryCode = countryCode
                };

                if (retailer != null)
                {
                    var response = (ean_upc == 999999999) ? "Invalid Product Code - Non Numeric" : "Invalid Product Code";
                    failScanResult.ValidationResponse = response;
                    failScanResult.RetailerID = retailer.Id;
                    failScanResult.AgentID = retailer.AgentID;
                    db.ScanResults.Add(failScanResult);
                    await db.SaveChangesAsync();
                    notifcationMessage = "Invalid Product: " + id + " scanned by retailer: " + retailer.Name;
                    isValidProduct = false;
                }

                await SendNotifcation(retailer.AgentID, notifcationMessage, isValidProduct);

                return Content(HttpStatusCode.OK, "Product Not Found");
            }

            //Validate the product against the country code. 
            //if not valid insert record in db 
            // + send notification to agent                

            //Insert new scan result information in the table
            ScanResult scanResult = new ScanResult
            {
                Id = Guid.NewGuid().ToString(),
                ProductID = product.Id,
                BarcodeValue = ean_upc,
                RetailerID = retailer.Id,
                AgentID = retailer.AgentID,

                ScanDate = DateTime.Now,
                ScanLocationLat = latitude,
                ScanLocationLong = longitude
            };

            scanResult.CountryCode = countryCode;

            if (countryCode.ToLower() == product.CountryCode.ToLower())
            {
                //valid product 
                scanResult.isAgentNotified = false;
                scanResult.isValid = true;
                scanResult.ValidationResponse = "Valid Product code";
                scanResult.isProcessed = true;
                isValidProduct = true;

                db.ScanResults.Add(scanResult);
                await db.SaveChangesAsync();

                return Ok(product);

            }
            else
            {
                //invalid product
                scanResult.isAgentNotified = true;
                scanResult.isValid = false;
                scanResult.isProcessed = true;
                scanResult.ValidationResponse = "Invalid Country Code. - Product Location Code: " + product.CountryCode + ", Scan Location :" + countryCode;
                isValidProduct = false;
                notifcationMessage = "Invalid Country code scanned by retailer " + retailer.Name + "  – Product Location Code: " + product.CountryCode + ", Scan Location: + " + countryCode + " Product Desc: " + product.description + " , Barcode: " + id;

                await SendNotifcation(retailer.AgentID, notifcationMessage, isValidProduct);

                db.ScanResults.Add(scanResult);
                await db.SaveChangesAsync();

                return Content(HttpStatusCode.OK, "Product Not Found");

            }
        }

        private static async Task SendNotifcation(string agentId, string notifcationMessage, bool isValidProduct)
        {
            if (!isValidProduct)
            {
                using (var common = new Common())
                {
                    await common.SendPushNotificationToAgent(agentId, notifcationMessage);
                }
            }
        }

        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProduct(string id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Products
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PostProduct(BarcodeScan product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return await ProcessScanBarcode(product.Barcode, product.Latitude, product.Longitude, product.RetailerId);

            //return Ok(product);



            //db.Products.Add(product);

            //try
            //{
            //    await db.SaveChangesAsync();
            //}
            //catch (DbUpdateException)
            //{
            //    if (ProductExists(product.Id))
            //    {
            //        return Conflict();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> DeleteProduct(string id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(string id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }

    }
}