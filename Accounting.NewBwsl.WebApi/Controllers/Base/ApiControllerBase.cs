using Manage.NewMK.WebApi.App_Start.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Accounting.NewMK.WebApi.Controllers.Base
{
    [CommonApiException]
    public class ApiControllerBase : ApiController
    {
    }
}