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
    public class AgentsController : ApiController
    {
        private MobileServiceContext db = new MobileServiceContext();

        // GET: api/Agents
        public IQueryable<Agent> GetAgents()
        {
            return db.Agents;
        }

        // GET: api/Agents/5
        [ResponseType(typeof(Agent))]
        public async Task<IHttpActionResult> GetAgent(string id)
        {
            Agent agent = await db.Agents.FindAsync(id);
            if (agent == null)
            {
                return NotFound();
            }

            return Ok(agent);
        }

        // PUT: api/Agents/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAgent(string id, Agent agent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != agent.Id)
            {
                return BadRequest();
            }

            db.Entry(agent).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgentExists(id))
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

        // POST: api/Agents
        [ResponseType(typeof(Agent))]
        public async Task<IHttpActionResult> PostAgent(Agent agent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Agents.Add(agent);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AgentExists(agent.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = agent.Id }, agent);
        }

        // DELETE: api/Agents/5
        [ResponseType(typeof(Agent))]
        public async Task<IHttpActionResult> DeleteAgent(string id)
        {
            Agent agent = await db.Agents.FindAsync(id);
            if (agent == null)
            {
                return NotFound();
            }

            db.Agents.Remove(agent);
            await db.SaveChangesAsync();

            return Ok(agent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AgentExists(string id)
        {
            return db.Agents.Count(e => e.Id == id) > 0;
        }
    }
}