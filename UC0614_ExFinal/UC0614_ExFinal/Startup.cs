using System;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Owin;

[assembly: OwinStartup(typeof(UC0614_ExFinal.Startup))]

namespace UC0614_ExFinal
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Cookie temporário utilizado durante o login Google/Facebook
            app.SetDefaultSignInAsAuthenticationType("ExternalCookie");

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ExternalCookie",
                AuthenticationMode = AuthenticationMode.Passive,
                CookieName = ".UC0614.External"
            });

            // -------------------------
            // GOOGLE
            // -------------------------

            string googleId =
                Environment.GetEnvironmentVariable(
                    "UC0614_GOOGLE_CLIENT_ID");

            string googleSecret =
                Environment.GetEnvironmentVariable(
                    "UC0614_GOOGLE_CLIENT_SECRET");

            if (Configurado(googleId) &&
                Configurado(googleSecret))
            {
                GoogleOAuth2AuthenticationOptions googleOptions =
                    new GoogleOAuth2AuthenticationOptions
                    {
                        ClientId = googleId,
                        ClientSecret = googleSecret
                    };

                googleOptions.Scope.Add("email");
                googleOptions.Scope.Add("profile");

                app.UseGoogleAuthentication(
                    googleOptions);
            }

            // -------------------------
            // FACEBOOK
            // -------------------------

            string facebookId =
                Environment.GetEnvironmentVariable(
                    "UC0614_FACEBOOK_APP_ID");

            string facebookSecret =
                Environment.GetEnvironmentVariable(
                    "UC0614_FACEBOOK_APP_SECRET");

            if (Configurado(facebookId) &&
                Configurado(facebookSecret))
            {
                FacebookAuthenticationOptions facebookOptions =
                    new FacebookAuthenticationOptions
                    {
                        AppId = facebookId,
                        AppSecret = facebookSecret
                    };

                facebookOptions.Scope.Add("email");

                app.UseFacebookAuthentication(
                    facebookOptions);
            }
        }

        private static bool Configurado(string valor)
        {
            return !String.IsNullOrWhiteSpace(valor);
        }
    }
}