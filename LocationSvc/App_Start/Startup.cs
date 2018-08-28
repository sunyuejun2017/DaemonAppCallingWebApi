using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Owin;
using Owin;

using Microsoft.Owin.Security.ActiveDirectory;
using System.Configuration;

[assembly: OwinStartup(typeof(LocationSvc.Startup))]
namespace LocationSvc
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            ConfigureAuth(app);
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            var azureADBearerAuthOptions = new WindowsAzureActiveDirectoryBearerAuthenticationOptions
            {
                Tenant = ConfigurationManager.AppSettings["ida:Tenant"]
            };

            azureADBearerAuthOptions.TokenValidationParameters =
                new System.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidAudience = ConfigurationManager.AppSettings["ida:Audience"]
                };

            app.UseWindowsAzureActiveDirectoryBearerAuthentication(azureADBearerAuthOptions);
        }
    }
}