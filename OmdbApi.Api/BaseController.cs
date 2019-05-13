using Microsoft.AspNetCore.Mvc;
using OmdbApi.DAL.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OmdbApi.Api
{
    public abstract class BaseController : ControllerBase
    {
        protected int GetUserId()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            return int.Parse(claimsIdentity.FindFirst(StaticVariables.UserId)?.Value);
        }
    }
}
