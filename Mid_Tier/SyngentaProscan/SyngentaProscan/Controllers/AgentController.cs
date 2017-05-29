using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using SyngentaProscan.DataObjects;
using SyngentaProscan.Models;

namespace SyngentaProscan.Controllers
{
    public class AgentController : TableController<Agent>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Agent>(context, Request);
        }

        // GET tables/Agent
        public IQueryable<Agent> GetAllAgent()
        {
            return Query(); 
        }

        // GET tables/Agent/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Agent> GetAgent(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Agent/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Agent> PatchAgent(string id, Delta<Agent> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Agent
        public async Task<IHttpActionResult> PostAgent(Agent item)
        {
            Agent current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Agent/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteAgent(string id)
        {
             return DeleteAsync(id);
        }
    }
}
