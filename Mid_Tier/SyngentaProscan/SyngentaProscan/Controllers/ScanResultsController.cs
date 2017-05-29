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
    public class ScanResultsController : ApiController
    {
        private MobileServiceContext db = new MobileServiceContext();

        // GET: api/ScanResults
        //public IQueryable<ScanResult> GetScanResults()
        //{
        //    return db.ScanResults;
        //}

        // GET: api/ScanResults/5
        [ResponseType(typeof(ProductScanResult))]
        public async Task<IHttpActionResult> GetScanResult(string retailerId = null, string agentId = null)
        {
            var minDate = DateTime.Now.AddMonths(-1);

            if (retailerId != null && agentId != null)
            {
                var dynamic = (from scan in db.ScanResults
                               join prod in db.Products
                               on scan.ProductID equals prod.Id into t
                               from rt in t.DefaultIfEmpty()
                               where (scan.AgentID == agentId && scan.RetailerID == retailerId
                               && scan.CreatedAt >= minDate)
                               select new ProductScanResult
                               {
                                   AgentEmail = scan.Agent.Email,
                                   Id = scan.Id,
                                   AgentID = scan.AgentID,
                                   RetailerID = scan.RetailerID,
                                   isAgentNotified = scan.isAgentNotified,
                                   isProcessed = scan.isProcessed,
                                   isValid = scan.isValid,
                                   BarcodeValue = scan.BarcodeValue,
                                   BatchID = scan.BatchID,
                                   ProductID = scan.ProductID,
                                   CountryCode = scan.CountryCode,
                                   ScanDate = scan.ScanDate,
                                   ScanLocationLat = scan.ScanLocationLat,
                                   ScanLocationLong = scan.ScanLocationLong,
                                   ValidationResponse = scan.ValidationResponse,
                                   ProductName = rt.description ?? "Invalid Product",
                                   ProductImage = rt.ProfileImage ?? "https://proscanstorage.blob.core.windows.net/proscancontainer/default.JPG"
                               }).ToListAsync();

                //List<ScanResult> scanResults = await db.ScanResults.Where(s => s.AgentID == agentId && s.RetailerID == retailerId).ToListAsync();

                return Ok(dynamic);
            }
            if (retailerId != null && agentId == null)
            {
                var dynamic = (from scan in db.ScanResults
                               join prod in db.Products
                               on scan.ProductID equals prod.Id into t
                               from rt in t.DefaultIfEmpty()
                               where (scan.RetailerID == retailerId && scan.CreatedAt >= minDate)
                               select new ProductScanResult
                               {
                                   AgentEmail = scan.Agent.Email,
                                   Id = scan.Id,
                                   AgentID = scan.AgentID,
                                   RetailerID = scan.RetailerID,
                                   isAgentNotified = scan.isAgentNotified,
                                   isProcessed = scan.isProcessed,
                                   isValid = scan.isValid,
                                   BarcodeValue = scan.BarcodeValue,
                                   BatchID = scan.BatchID,
                                   ProductID = scan.ProductID,
                                   CountryCode = scan.CountryCode,
                                   ScanDate = scan.ScanDate,
                                   ScanLocationLat = scan.ScanLocationLat,
                                   ScanLocationLong = scan.ScanLocationLong,
                                   ValidationResponse = scan.ValidationResponse,
                                   ProductName = rt.description ?? "Invalid Product",
                                   ProductImage = rt.ProfileImage ?? "https://proscanstorage.blob.core.windows.net/proscancontainer/default.JPG"
                               }).ToListAsync();
                //List<ScanResult> scanResults = await db.ScanResults.Where(s => s.RetailerID == retailerId).ToListAsync();
                return Ok(dynamic);
            }
            if (retailerId == null && agentId != null)
            {
                var dynamic = (from scan in db.ScanResults
                               join prod in db.Products
                               on scan.ProductID equals prod.Id into t
                               from rt in t.DefaultIfEmpty()
                               where (scan.AgentID == agentId && scan.CreatedAt >= minDate)
                               select new ProductScanResult
                               {
                                   AgentEmail = scan.Agent.Email,
                                   Id = scan.Id,
                                   AgentID = scan.AgentID,
                                   RetailerID = scan.RetailerID,
                                   isAgentNotified = scan.isAgentNotified,
                                   isProcessed = scan.isProcessed,
                                   isValid = scan.isValid,
                                   BarcodeValue = scan.BarcodeValue,
                                   BatchID = scan.BatchID,
                                   ProductID = scan.ProductID,
                                   CountryCode = scan.CountryCode,
                                   ScanDate = scan.ScanDate,
                                   ScanLocationLat = scan.ScanLocationLat,
                                   ScanLocationLong = scan.ScanLocationLong,
                                   ValidationResponse = scan.ValidationResponse,
                                   ProductName = rt.description ?? "Invalid Product",
                                   ProductImage = rt.ProfileImage ?? "https://proscanstorage.blob.core.windows.net/proscancontainer/default.JPG"
                               }).ToListAsync();
                //List<ScanResult> scanResults = await db.ScanResults.Where(s => s.AgentID == agentId).ToListAsync();
                return Ok(dynamic);
            }
            return Ok("No record found");

        }

        // PUT: api/ScanResults/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutScanResult(string id, ScanResult scanResult)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != scanResult.Id)
            {
                return BadRequest();
            }

            db.Entry(scanResult).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScanResultExists(id))
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

        // POST: api/ScanResults
        //[ResponseType(typeof(ScanResult))]
        [HttpPost]
        public async Task<IHttpActionResult> PostScanResult(List<ScanResult> scanResults)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                foreach (var scanResult in scanResults)
                {
                    scanResult.Id = Guid.NewGuid().ToString();
                    db.ScanResults.Add(scanResult);
                }

                try
                {
                    await db.SaveChangesAsync();
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

        // DELETE: api/ScanResults/5
        [ResponseType(typeof(ScanResult))]
        public async Task<IHttpActionResult> DeleteScanResult(string id)
        {
            ScanResult scanResult = await db.ScanResults.FindAsync(id);
            if (scanResult == null)
            {
                return NotFound();
            }

            db.ScanResults.Remove(scanResult);
            await db.SaveChangesAsync();

            return Ok(scanResult);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ScanResultExists(string id)
        {
            return db.ScanResults.Count(e => e.Id == id) > 0;
        }
    }
}