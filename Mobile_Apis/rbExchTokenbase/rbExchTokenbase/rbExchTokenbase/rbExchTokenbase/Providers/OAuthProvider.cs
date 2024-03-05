using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using rbExchTokenbase.ClassFiles;

namespace rbExchTokenbase.Providers
{
    public class OAuthProvider : OAuthAuthorizationServerProvider
    {
        #region[GrantResourceOwnerCredentials]           
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            if (!context.OwinContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "https://10221975.com" });
            var form = context.Request.ReadFormAsync().Result;
            // var country = form.ElementAt(3);
            //var logintime = form.GetValues("login_time");
            var key = form.GetValues("key");
            var appname = form.GetValues("appname");

            string _key = key[0];
            //var LoginTime = form.ElementAt(4);
            // string _LoginTime = logintime[0];
            // var IP = form.ElementAt(5);
            string _appname = appname[0];
            Utils _utils = new Utils();

            return Task.Factory.StartNew(() =>
            {

                if (context.Request.Headers.ContainsKey("Postman-Token"))
                {
                    context.SetError("Invalid_grant", "Unauthorized Access..!!!");
                }
                else
                {
                    var userName = context.UserName;
                    var password = context.Password;
                    var pass = _utils.Encryptdata(password);

                    var userService = new UserService();// our created one
                    var user = userService.ValidateUser(userName, pass);
                    if (user.message == "No data available in Limits")
                    {
                        context.SetError("Invalid_grant", "Please Set Users LIMIT");
                    }
                    else if (user.clientcode != null)
                    {
                        var status = _utils.validateuserstatus(user.clientcode);
                        bool flag = _utils.VerfiyAppAccess(userName, _appname, _key);
                        if (flag == true)
                        {
                            if (status == 1)
                            {
                                if (user.usertype == 4)
                                {
                                    var claims = new List<Claim>()
                                {
                                    new Claim(ClaimTypes.Name,user.clientcode),
                                    new Claim(ClaimTypes.Expiration, user.createdby),
                                };
                                    ClaimsIdentity oAuthIdentity = new ClaimsIdentity(claims, Startup.OAuthOptions.AuthenticationType);
                                    ClaimsIdentity cookiesIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType);
                                    // user.KycStatus = userService.getKycstatus(user.usercode);
                                    var properties = CreateProperties(user);
                                    var ticket = new AuthenticationTicket(oAuthIdentity, properties);
                                    context.Request.Context.Authentication.SignIn(cookiesIdentity);
                                    context.Validated(ticket);

                                    //  utils.loginData(user.usercode, IP, contry, loginIsp);
                                }
                                else
                                {
                                    context.SetError("Invalid_grant", "This application for clients Only");
                                }
                            }
                            else
                            {
                                context.SetError("Invalid_grant", "Account blocked by admin");
                            }
                        }
                        else
                        {
                            context.SetError("Invalid_grant", "Unauthrized App access");
                        }
                    }
                    else
                    {
                        context.SetError("Invalid_grant", "The user name or password is incorrect");
                    }

                    //  }
                    // else
                    // {
                    //   context.SetError("Invalid_grant", "Unauthrized access via Postman");
                    //}
                }
            }
       );



        }
        #endregion
        #region[ValidateClientAuthentication]
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
                context.Validated();
            return Task.FromResult<object>(null);
        }
        #endregion
        #region[TokenEndpoint]
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {

            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {


                if (property.Key == ".issued")
                {
                    var Issued = DateTime.Now.ToString();
                    context.AdditionalResponseParameters.Add(property.Key, Issued);
                }
                else if (property.Key == ".expires")
                {
                    var Expires = DateTime.Now.AddMinutes(10).ToString();
                    context.AdditionalResponseParameters.Add(property.Key, Expires);
                }
                else
                {
                    context.AdditionalResponseParameters.Add(property.Key, property.Value);
                }
            }

            return Task.FromResult<object>(null);
        }
        #endregion
        #region[CreateProperties]
        public static AuthenticationProperties CreateProperties(userinfo user)
        {
            Utils _utils = new Utils();
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "Message","Success" },
                { "base_Url", "https://10221975.com/RbexchNew/v1/"},
                { "Clientcode", user.clientcode },
                { "id", user.id },
                { "Name", user.name },
                { "createdby", user.createdby },
                { "emailid", user.email_id },
                { "exchanhges", user.exchanhges },
                { "usertype", user.usertype.ToString() },
                { "validity", user.validity },
                { "producttype", user.producttype },
                { "userstatus", user.userstatus.ToString() },
            };
            return new AuthenticationProperties(data);
        }
        #endregion
    }
}