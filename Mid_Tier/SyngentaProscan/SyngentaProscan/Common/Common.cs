using Microsoft.Azure.NotificationHubs.Messaging;
using Newtonsoft.Json.Linq;
using SyngentaProscan.DataObjects;
using SyngentaProscan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SyngentaProscan
{
    public class Common : IDisposable
    {
        private MobileServiceContext db = new MobileServiceContext();

        internal static string GetCountryCode(decimal latitude, decimal longitude)
        {
            //Get Country code from google api and validate against the product code
            var strLatitude = latitude.ToString();
            var strLongitude = longitude.ToString();

            WebClient client = new WebClient();
            string Url = "http://maps.googleapis.com/maps/api/geocode/json?latlng=" + strLatitude + "," + strLongitude + "&sensor=true";
            var jsonResponse = client.DownloadString(new Uri(Url, UriKind.RelativeOrAbsolute));

            JObject parseJson = JObject.Parse(jsonResponse);
            var getJsonres = parseJson["results"][0];

            var countryCode = string.Empty;

            int count = 0;
            while (count >= 0)
            {
                var addressInfo = getJsonres["address_components"][count];
                var addressType = addressInfo["types"];
                if (addressType.ToString().Contains("country"))
                {
                    countryCode = addressInfo["short_name"].ToString();
                    break;
                }
                count++;
            }

            return countryCode;
        }

        internal async Task<string> SendPushNotificationToAgent(string userID, string message)
        {
            var response = string.Empty;

            //if (userType == "a")
            {
                var agentTag = GetTagForAgent(userID);
                response = await SendPushNotificationToUser(agentTag, message);
            }
            //else //retailer
            //{
            //    var retailerTag = GetTagForRetailer(userID);
            //    response = await SendPushNotificationToUser(retailerTag, message);
            //}

            return response;
        }

        internal async Task<string> SendPushNotificationToUser(string uniqueTag, string message)
        {
            string[] userTag = new string[1];
            userTag[0] = uniqueTag;

            Microsoft.Azure.NotificationHubs.NotificationOutcome outcome = null;
            HttpStatusCode ret = HttpStatusCode.InternalServerError;

            //We only have android devices as of now. 
            //If other platforms are targeted, update code with switch for other platform                       
            var notification = "{ \"data\" : {\"message\":\"" + message + "\"}}";
            outcome = await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notification, userTag);

            if (outcome != null)
            {
                if (!((outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Abandoned) ||
                    (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Unknown)))
                {
                    ret = HttpStatusCode.OK;
                }
            }

            return ret.ToString();
        }

        /// <summary>
        /// Get Tag information for agent
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        internal string GetTagForAgent(string agentId)
        {
            var pnsId = string.Empty;
            var agentInfo = db.Agents.FirstOrDefault(a => a.Id == agentId);
            if (agentInfo != null)
            {
                pnsId = agentInfo.PNSID;
            }
            return pnsId;
        }

        /// <summary>
        /// Get tag for retailer
        /// </summary>
        /// <param name="retailerId"></param>
        /// <returns></returns>
        internal string GetTagForRetailer(string retailerId)
        {
            var pnsId = string.Empty;
            var agentInfo = db.Retailers.FirstOrDefault(a => a.Id == retailerId);
            if (agentInfo != null)
            {
                pnsId = agentInfo.PNSID;
            }
            return pnsId;
        }

        internal void AddUpdateAgent(string loginId)
        {
            var user = db.Agents.FirstOrDefault(a => a.LoginID == loginId);
            if (user != null)
            {
                user.PNSID = loginId;
                db.SaveChanges();
            }
            //else
            //{
            //    var agent = new Agent
            //    {
            //        Id = Guid.NewGuid().ToString(),
            //        LoginID = loginId,
            //        PNSID = loginId,
            //        isEnabled = false,
            //    };

            //    db.Agents.Add(agent);
            //    db.SaveChanges();
            //}
        }

        internal void AddUpdateRetailer(string loginId)
        {
            var user = db.Retailers.FirstOrDefault(a => a.LoginID == loginId);
            if (user != null)
            {
                user.PNSID = loginId;
                db.SaveChanges();
            }
            //else
            //{
            //    var retailer = new Retailer
            //    {
            //        Id = Guid.NewGuid().ToString(),
            //        AgentID = agentId,
            //        LoginID = loginId,
            //        PNSID = loginId,
            //        isEnabled = false,
            //    };

            //    db.Retailers.Add(retailer);
            //    db.SaveChanges();
            //}
        }

        internal static void ReturnGoneIfHubResponseIsGone(MessagingException e)
        {
            var webex = e.InnerException as WebException;
            if (webex.Status == WebExceptionStatus.ProtocolError)
            {
                var response = (HttpWebResponse)webex.Response;
                if (response.StatusCode == HttpStatusCode.Gone)
                    throw new HttpRequestException(HttpStatusCode.Gone.ToString());
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    db.Dispose();
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Common() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}