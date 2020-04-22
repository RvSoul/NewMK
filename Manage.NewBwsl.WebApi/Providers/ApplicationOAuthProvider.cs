using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Manage.NewMK.WebApi.Models;
using NewMK.Domian.DM;
using NewMK.DTO.AdminUser;
using System.Web;
using Utility;

namespace Manage.NewMK.WebApi.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }
        public string getIP()
        {
            string uip = "";
            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            {
                uip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                uip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            return uip;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            bool ifww = false;
            //公司外访问验证
            //string ip = HttpHelper.GetUserIp();
            //    //getIP();

            //string[] sArray = System.Configuration.ConfigurationManager.ConnectionStrings["IP"].ConnectionString.Split('-');
            //foreach (string item in sArray)
            //{
            //    if (ip.Contains(item))
            //    {
            //        ifww = true;
            //    }
            //}
            //if (!ifww)//ip != System.Configuration.ConfigurationManager.ConnectionStrings["IP"].ConnectionString
            //{
            //    if (context.Scope[0] != null && context.Scope[0] != "" && context.Scope[0] != "undefined")
            //    {
            //        string cookyzmtt = HttpRuntime.Cache[context.Scope[0] + "-yzm"] == null ? "" : HttpRuntime.Cache[context.Scope[0] + "-yzm"].ToString();
            //        if (cookyzmtt != context.Scope[0])
            //        {
            //            context.SetError("invalid_grant", "验证码无效！");
            //            return;
            //        }
            //        else
            //        {
            //            HttpRuntime.Cache.Remove(context.Scope[0] + "-yzm");
            //        }
            //    }
            //    else
            //    {
            //        context.SetError("invalid_grant", "外网登录请输入验证码！");
            //        return;
            //    }

            //}

            //    var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            //    ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
            AdminUserDM DM = new AdminUserDM();
            AdminUserDTO user = DM.LoginUser(context.UserName, context.Password,"1");

            if (user == null)
            {
                context.SetError("invalid_grant", "用户名或密码不正确。");
                return;
            }

            ClaimsIdentity oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, user.ID.ToString()));
            //await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);

            //ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager, CookieAuthenticationDefaults.AuthenticationType);
            //context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // 资源所有者密码凭据未提供客户端 ID。
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}