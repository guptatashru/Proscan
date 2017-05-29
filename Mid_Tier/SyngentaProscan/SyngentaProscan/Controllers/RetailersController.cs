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
    public class RetailersController : ApiController
    {
        private MobileServiceContext db = new MobileServiceContext();

        // GET: api/Retailers
        public IQueryable<Retailer> GetRetailers()
        {
            try
            {
                return db.Retailers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        // GET: api/Retailers/5
        [ResponseType(typeof(Retailer))]
        public async Task<IHttpActionResult> GetRetailer(string id)
        {
            try
            {
                Retailer retailer = await db.Retailers.FirstOrDefaultAsync(r => r.LoginID.ToLower() == id.ToLower());
                if (retailer == null)
                {
                    return Ok("No Retailer Found");
                }

                return Ok(retailer);

            }
            catch (Exception ex)
            {
                return Ok(ex.InnerException + " msg: " + ex.Message + " staktrace " + ex.StackTrace);
            }
            
        }

        // PUT: api/Retailers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRetailer(string id, Retailer retailer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != retailer.Id)
            {
                return BadRequest();
            }

            db.Entry(retailer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RetailerExists(id))
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

        // POST: api/Retailers
        [ResponseType(typeof(Retailer))]
        public async Task<IHttpActionResult> PostRetailer(Retailer retailer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Retailers.Add(retailer);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RetailerExists(retailer.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = retailer.Id }, retailer);
        }

        // DELETE: api/Retailers/5
        [ResponseType(typeof(Retailer))]
        public async Task<IHttpActionResult> DeleteRetailer(string id)
        {
            Retailer retailer = await db.Retailers.FindAsync(id);
            if (retailer == null)
            {
                return NotFound();
            }

            db.Retailers.Remove(retailer);
            await db.SaveChangesAsync();

            return Ok(retailer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RetailerExists(string id)
        {
            return db.Retailers.Count(e => e.Id == id) > 0;
        }
    }
}