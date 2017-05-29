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
    public class RetailerController : TableController<Retailer>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Retailer>(context, Request);
        }

        // GET tables/Retailer
        public IQueryable<Retailer> GetAllRetailer()
        {
            return Query(); 
        }

        // GET tables/Retailer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Retailer> GetRetailer(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Retailer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Retailer> PatchRetailer(string id, Delta<Retailer> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Retailer
        public async Task<IHttpActionResult> PostRetailer(Retailer item)
        {
            Retailer current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Retailer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteRetailer(string id)
        {
             return DeleteAsync(id);
        }
    }
}
