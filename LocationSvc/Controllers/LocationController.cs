using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace LocationSvc.Controllers
{
    [Authorize]
    public class LocationController : ApiController
    {
        // https://localhost:44300/api/location?cityName=dc
        public Models.Location GetLocation(string cityName)
        {
            return new Models.Location() { Latitude = 10, Longitude = 20 };
        }
    }
}