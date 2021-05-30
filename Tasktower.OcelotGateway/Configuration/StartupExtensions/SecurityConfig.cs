using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Tasktower.OcelotGateway.Configuration.StartupExtensions
{
    public static class SecurityConfig
    {
        
        public const string XsrfTokenHeaderName = "X-XSRF-TOKEN";
        public const string XsrfCookieName = "XSRF-TOKEN";
        
        public static void ConfigureAntiForgery(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAntiforgery(options =>
            {
                options.HeaderName = XsrfTokenHeaderName;
            });
        }
        
        public static void UseCorsConfig(this IApplicationBuilder app, IConfiguration configuration)
        {
            if (configuration.GetValue("Cors:Enable", false))
            {
                app.UseCors();
            }
        }
        public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue("Cors:Enable", false))
            {
                services.AddCors();
            }
        }

        public static void ConfigureCookies(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
                options.OnAppendCookie = cookieContext => CheckSameSite(cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext => CheckSameSite(cookieContext.CookieOptions);
            });

        }
        
        public static void CheckSameSite(CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None && options.Secure == false)
            {
                options.SameSite = SameSiteMode.Lax;
            }
        }

        public static void ConfigureWebAppAuth(this IServiceCollection services, IConfiguration configuration)
        {
            // Add authentication services
            services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOpenIdConnect("Auth0", options => {
                    // Set the authority to your Auth0 domain
                    options.Authority = $"https://{configuration["Authentication:Openid:Domain"]}";

                    // Configure the Auth0 Client ID and Client Secret
                    options.ClientId = configuration["Authentication:Openid:ClientId"];
                    options.ClientSecret = configuration["Authentication:Openid:ClientSecret"];

                    // Set response type to code
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    // options.ResponseMode = OpenIdConnectResponseMode.Query;

                    // Configure the scope
                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");

                    // Set the callback path, so Auth0 will call back to http://<your_domain>:<insert port>/
                    // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
                    options.CallbackPath = new PathString("/auth-client/callback");

                    // Configure the Claims Issuer to be Auth0
                    options.ClaimsIssuer = "Auth0";
                    options.SaveTokens = true;

                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = context =>
                        {
                            // The context's ProtocolMessage can be used to pass along additional query parameters
                            // to Auth0's /authorize endpoint.
                            // 
                            // Set the audience query parameter to the API identifier to ensure the returned Access Tokens can be used
                            // to call protected endpoints on the corresponding API.
                            context.ProtocolMessage.SetParameter("audience", configuration["Authentication:Openid:Audience"]);

                            return Task.FromResult(0);
                        },
                        // handle the logout redirection
                        OnRedirectToIdentityProviderForSignOut = (context) =>
                        {
                            var logoutUri = $"https://{configuration["Authentication:Openid:Domain"]}/v2/logout?client_id={configuration["Authentication:Openid:ClientId"]}";

                            var postLogoutUri = context.Properties.RedirectUri;
                            if (!string.IsNullOrEmpty(postLogoutUri))
                            {
                                if (postLogoutUri.StartsWith("/"))
                                {
                                    // transform to absolute
                                    var request = context.Request;
                                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                                }
                                logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                            }

                            context.Response.Redirect(logoutUri);
                            context.HandleResponse();

                            return Task.CompletedTask;
                        }
                        
                    };
                });
        }

    }
}