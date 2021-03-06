﻿using System;
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
    public class RetailersController : ApiController
    {
        private SyngentaProScanContext db = new SyngentaProScanContext();

        // GET: api/Retailers
        public IQueryable<Retailer> GetRetailers()
        {
            return db.Retailers;
        }

        // GET: api/Retailers/5
        [ResponseType(typeof(Retailer))]
        public async Task<IHttpActionResult> GetRetailer(int id)
        {
            Retailer retailer = await db.Retailers.FindAsync(id);
            if (retailer == null)
            {
                return NotFound();
            }

            return Ok(retailer);
        }

        // PUT: api/Retailers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRetailer(int id, Retailer retailer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != retailer.RetailerID)
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
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = retailer.RetailerID }, retailer);
        }

        // DELETE: api/Retailers/5
        [ResponseType(typeof(Retailer))]
        public async Task<IHttpActionResult> DeleteRetailer(int id)
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

        private bool RetailerExists(int id)
        {
            return db.Retailers.Count(e => e.RetailerID == id) > 0;
        }
    }
}