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
    public class ScanResultController : TableController<ScanResult>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<ScanResult>(context, Request);
        }

        // GET tables/ScanResult
        public IQueryable<ScanResult> GetAllScanResult()
        {
            return Query(); 
        }

        // GET tables/ScanResult/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<ScanResult> GetScanResult(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/ScanResult/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<ScanResult> PatchScanResult(string id, Delta<ScanResult> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/ScanResult
        public async Task<IHttpActionResult> PostScanResult(ScanResult item)
        {
            ScanResult current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/ScanResult/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteScanResult(string id)
        {
             return DeleteAsync(id);
        }
    }
}
