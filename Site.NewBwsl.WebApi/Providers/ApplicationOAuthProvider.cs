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
using Site.NewMK.WebApi.Models;
using NewMK.Domian.DM;
using NewMK.DTO.User;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.Web;

namespace Site.NewMK.WebApi.Providers
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

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            //ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            UserDM DM = new UserDM();
            UserDTO user = new UserDTO();
            if (context.Scope[0] == "1")
            {
                //密码登录
                user = DM.LoginUser(context.UserName, context.Password);
                
                if (user == null)
                {
                    context.SetError("invalid_grant", "用户名或密码不正确！");
                    return;
                }
                else
                {
                    if (user.UserState == 0)
                    {
                        context.SetError("invalid_grant", "账号未激活！");
                        return;
                    }
                    if (user.UserState == 2)
                    {
                        context.SetError("invalid_grant", "账号被禁用！");
                        return;
                    }
                    if (user.UserState == 3)
                    {
                        context.SetError("invalid_grant", "账号已失效！");
                        return;
                    }
                    #region (数据库设置单处登录)
                    //单点登录判断  result.Data.LatelyIP== localaddr.ToString()
                    string localaddr = HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString();
                    DateTime t1 = DateTime.Now;
                    TimeSpan ts = t1 - Convert.ToDateTime(user.LastTime);
                    int a = ts.Days;
                    int b = ts.Hours;
                    int c = ts.Minutes;
                    if (a < 1)
                    {
                        if (b < 1)
                        {
                            //10分钟过期
                            if (c < 10)
                            {
                                if (user.LastIP != localaddr.ToString())
                                {
                                    //context.SetError("invalid_grant", "该用户已登录，请10分钟后在试！");
                                    //return;
                                }
                            }
                        }
                    }
                    DM.UpLoginTime(context.UserName, localaddr.ToString());
                    #endregion
                }
            }
            else if (context.Scope[0] == "2")
            {
                string openid = context.UserName;
                //微信登录
                string unionid = context.Password;
                //验证是否注册
                user = DM.LoginUserWx(openid, unionid);
                if (user == null)
                {
                    context.SetError("invalid_grant", "未找到该用户！");
                    return;
                }
                else
                {
                    if (user.UserState == 0)
                    {
                        context.SetError("invalid_grant", "账号未激活！");
                        return;
                    }
                    if (user.UserState == 2)
                    {
                        context.SetError("invalid_grant", "账号被禁用！");
                        return;
                    }
                    if (user.UserState == 3)
                    {
                        context.SetError("invalid_grant", "账号已失效！");
                        return;
                    }
                }

            }
            else if (context.Scope[0] == "3")
            {
                //验证码登录
            }



            ClaimsIdentity oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, user.ID.ToString()));
            //await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(user.Phone);
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