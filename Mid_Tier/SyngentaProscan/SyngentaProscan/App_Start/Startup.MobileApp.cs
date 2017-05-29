using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using SyngentaProscan.DataObjects;
using SyngentaProscan.Models;
using Owin;
using Microsoft.Azure.Mobile.Server.Tables.Config;
using System.Linq;

namespace SyngentaProscan
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            //new MobileAppConfiguration()
            //    .AddTables(
            //        new MobileAppTableConfiguration()
            //            .MapTableControllers()
            //            .AddEntityFramework())
            //    .MapApiControllers()
            //    .ApplyTo(config);


            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional });

            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new MobileServiceInitializer());

            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    // This middleware is intended to be used locally for debugging. By default, HostName will
                    // only have a value when running in an App Service application.
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }

            app.UseWebApi(config);

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
        }
    }

    public class MobileServiceInitializer : CreateDatabaseIfNotExists<MobileServiceContext>
    {
        protected override void Seed(MobileServiceContext context)
        {
            List<TodoItem> todoItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid().ToString(), Text = "First item", Complete = false },
                new TodoItem { Id = Guid.NewGuid().ToString(), Text = "Second item", Complete = false }
            };

            foreach (TodoItem todoItem in todoItems)
            {
                context.Set<TodoItem>().Add(todoItem);
            }

            List<Product> products = new List<Product>
            {
                new Product {Id = Guid.NewGuid().ToString(), AUn= "AUN Number", CountryCode="DE", EAN_UPC=123456, description="Dummy Product", Denom =1, MaterialID=345, EANNumber=989898, Numerat=5  },
                new Product {Id = Guid.NewGuid().ToString(), AUn= "AUN Number", CountryCode="DE", EAN_UPC=987654321, description="Dummy 2 Product", Denom =1, MaterialID=345, EANNumber=989898, Numerat=5  }
            };

            foreach (var product in products)
            {
                context.Set<Product>().Add(product);
            }


            List<Agent> agents = new List<Agent>
            {
                new Agent {Id = Guid.NewGuid().ToString(), Address="255 Address", Email="agent@hpe.com", isEnabled=false, MobileNo="999998888", LoginID="agent@hpe.com", Name="Agent ankit", PNSID="agent@hpe.com" }
                
            };

            foreach (var agent in agents)
            {
                context.Set<Agent>().Add(agent);
            }


            base.Seed(context);
        }
    }
}

