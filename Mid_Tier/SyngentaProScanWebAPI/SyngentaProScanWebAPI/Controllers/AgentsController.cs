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
    public class AgentsController : ApiController
    {
        private SyngentaProScanContext db = new SyngentaProScanContext();

        // GET: api/Agents
        public IQueryable<Agent> GetAgents()
        {
            return db.Agents;
        }

        // GET: api/Agents/5
        [ResponseType(typeof(Agent))]
        public async Task<IHttpActionResult> GetAgent(int id)
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
        public async Task<IHttpActionResult> PutAgent(int id, Agent agent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != agent.AgentID)
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
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = agent.AgentID }, agent);
        }

        // DELETE: api/Agents/5
        [ResponseType(typeof(Agent))]
        public async Task<IHttpActionResult> DeleteAgent(int id)
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

        private bool AgentExists(int id)
        {
            return db.Agents.Count(e => e.AgentID == id) > 0;
        }
    }
}