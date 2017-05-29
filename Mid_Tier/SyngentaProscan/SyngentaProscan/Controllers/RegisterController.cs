using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace SyngentaProscan.Controllers
{
    public class RegisterController : ApiController
    {

        private NotificationHubClient hub;

        public RegisterController()
        {
            hub = Notifications.Instance.Hub;
        }

        // GET: api/Register
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Register/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Register
        public async Task<IHttpActionResult> Get(string loginId, string type, string handle = null)
        {
            try
            {
                var registrations = await hub.GetRegistrationsByChannelAsync(handle, 100);

                try
                {
                    //Identify User 
                    //Insert tag in DB and update notification information
                    using (var commom = new Common())
                    {
                        switch (type.ToLower())
                        {
                            case "a":
                                commom.AddUpdateAgent(loginId);
                                break;
                            case "r":
                                commom.AddUpdateRetailer(loginId);
                                break;
                        }
                    }

                    foreach (RegistrationDescription registration in registrations)
                    {
                        registration.Tags = new HashSet<string>(new string[] { loginId });
                        await hub.CreateOrUpdateRegistrationAsync(registration);
                    }
                    return Ok("Push Notification Registration Successful");
                }
                catch (MessagingException e)
                {
                    Common.ReturnGoneIfHubResponseIsGone(e);
                    return Ok("Push Notification Registration Failed");
                }
            }
            catch (Exception ex)
            {
                return Ok("Push Notification Registration Failed. " + ex.Message);                
            }
        }
    }
}
