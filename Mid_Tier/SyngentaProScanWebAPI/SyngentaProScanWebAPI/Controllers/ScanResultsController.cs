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
using SyngentaProScanWebAPI;

namespace SyngentaProScanWebAPI.Controllers
{
    public class ScanResultsController : ApiController
    {
        private SyngentaProScanContext db = new SyngentaProScanContext();

        // GET: api/ScanResults
        public IQueryable<ScanResult> GetScanResults()
        {
            return db.ScanResults;
        }

        // GET: api/ScanResults/5
        [ResponseType(typeof(ScanResult))]
        public async Task<IHttpActionResult> GetScanResult(int id)
        {
            ScanResult scanResult = await db.ScanResults.FindAsync(id);
            if (scanResult == null)
            {
                return NotFound();
            }

            return Ok(scanResult);
        }

        // PUT: api/ScanResults/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutScanResult(int id, ScanResult scanResult)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != scanResult.ScanResultID)
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
        [ResponseType(typeof(ScanResult))]
        public async Task<IHttpActionResult> PostScanResult(ScanResult scanResult)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ScanResults.Add(scanResult);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = scanResult.ScanResultID }, scanResult);
        }

        // DELETE: api/ScanResults/5
        [ResponseType(typeof(ScanResult))]
        public async Task<IHttpActionResult> DeleteScanResult(int id)
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

        private bool ScanResultExists(int id)
        {
            return db.ScanResults.Count(e => e.ScanResultID == id) > 0;
        }
    }
}