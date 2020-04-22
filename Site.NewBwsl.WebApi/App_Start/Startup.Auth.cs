﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Site.NewMK.WebApi.Providers;
using Site.NewMK.WebApi.Models;
using Microsoft.Owin.Cors;

namespace Site.NewMK.WebApi
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // 有关配置身份验证的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            //// 将数据库上下文和用户管理器配置为对每个请求使用单个实例
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            //app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            //// 使应用程序可以使用 Cookie 来存储已登录用户的信息
            //// 并使用 Cookie 来临时存储有关使用第三方登录提供程序登录的用户的信息
            //app.UseCookieAuthentication(new CookieAuthenticationOptions());
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            //// 针对基于 OAuth 的流配置应用程序
            //PublicClientId = "self";
            //OAuthOptions = new OAuthAuthorizationServerOptions
            //{
            //    TokenEndpointPath = new PathString("/Token"),
            //    Provider = new ApplicationOAuthProvider(PublicClientId),
            //    AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
            //    AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
            //    //在生产模式下设 AllowInsecureHttp = false
            //    AllowInsecureHttp = true
            //};

            //// 使应用程序可以使用不记名令牌来验证用户身份
            //app.UseOAuthBearerTokens(OAuthOptions);

            //// 取消注释以下行可允许使用第三方登录提供程序登录
            ////app.UseMicrosoftAccountAuthentication(
            ////    clientId: "",
            ////    clientSecret: "");

            ////app.UseTwitterAuthentication(
            ////    consumerKey: "",
            ////    consumerSecret: "");

            ////app.UseFacebookAuthentication(
            ////    appId: "",
            ////    appSecret: "");

            ////app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            ////{
            ////    ClientId = "",
            ////    ClientSecret = ""
            ////});

            OAuthAuthorizationServerOptions OAuthOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,//允许客户端一http协议请求
                TokenEndpointPath = new PathString("/token"), //token请求的地址，即http://localhost:端口号/token；
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(600),
                Provider = new ApplicationOAuthProvider("PublicClientId") //提供具体的认证策略；
            };
            //app.UseCors(CorsOptions.AllowAll);
            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
