/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */
using System;
using System.Collections.Generic; 
using System.Text; 
 
using Microsoft.AspNetCore.WebUtilities; 

using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Extensions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
 
namespace AspNet.Security.OAuth.Transferwise {
    /// <summary>
    /// TransferwiseAuthenticationHandler.
    /// </summary>
    public class TransferwiseAuthenticationHandler : OAuthHandler<TransferwiseAuthenticationOptions> {
        /// <summary>
        /// TransferwiseAuthenticationHandler.
        /// </summary>
        public TransferwiseAuthenticationHandler([NotNull] HttpClient client)
            : base(client) {
        }

        /// <summary>
        /// CreateTicketAsync.
        /// </summary>
        protected override async Task<AuthenticationTicket> CreateTicketAsync([NotNull] ClaimsIdentity identity,
            [NotNull] AuthenticationProperties properties, [NotNull] OAuthTokenResponse tokens) {
            
            var request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            var response = await Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, Context.RequestAborted);
            if (!response.IsSuccessStatusCode) {
                Logger.LogError("An error occurred when retrieving the user profile: the remote server " +
                                "returned a {Status} response with the following payload: {Headers} {Body}.",
                                /* Status: */ response.StatusCode,
                                /* Headers: */ response.Headers.ToString(),
                                /* Body: */ await response.Content.ReadAsStringAsync());

                throw new HttpRequestException("An error occurred when retrieving the user profile.");
            } 

            var json = await response.Content.ReadAsStringAsync();
            var array = Newtonsoft.Json.JsonConvert.DeserializeObject<object[]>(json);
            dynamic element = array[0];
            var idtoken = (string) element.id;
            var payload = new JObject(element); 
                
            identity.AddOptionalClaim(ClaimTypes.Authentication, tokens.AccessToken, Options.ClaimsIssuer)
            .AddOptionalClaim(ClaimTypes.NameIdentifier, idtoken, Options.ClaimsIssuer);
            // todo: retrieve profile information here
    /*        var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

             
            var request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            var response = await Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, Context.RequestAborted);
              
             
            identity.AddOptionalClaim(ClaimTypes.NameIdentifier, TransferwiseAuthenticationHelper.GetIdentifier(payload), Options.ClaimsIssuer)
                    .AddOptionalClaim(ClaimTypes.Name, TransferwiseAuthenticationHelper.GetFullName(payload), Options.ClaimsIssuer)
                    .AddOptionalClaim(ClaimTypes.GivenName, TransferwiseAuthenticationHelper.GetGivenName(payload), Options.ClaimsIssuer)
                    .AddOptionalClaim(ClaimTypes.Surname, TransferwiseAuthenticationHelper.GetFamilyName(payload), Options.ClaimsIssuer)
                    .AddOptionalClaim(ClaimTypes.Email, TransferwiseAuthenticationHelper.GetEmail(payload), Options.ClaimsIssuer);
*/
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, properties, Options.AuthenticationScheme);

            var context = new OAuthCreatingTicketContext(ticket, Context, Options, Backchannel, tokens, payload);
            await Options.Events.CreatingTicket(context);

            return context.Ticket;
        }

        /// <summary>
        /// Exchanges code for a token.
        /// </summary>
        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync([NotNull] string code, [NotNull] string redirectUri) {

            System.Diagnostics.Debug.WriteLine("Exchange code async");
            var request = new HttpRequestMessage(HttpMethod.Post, Options.TokenEndpoint);
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Options.ClientId}:{Options.ClientSecret}"));
 
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
 
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                ["client_id"] = Options.ClientId,
                ["redirect_uri"] = redirectUri,
                ["client_secret"] = Options.ClientSecret,
                ["code"] = code,
                ["grant_type"] = "authorization_code"
            });

            var response = await Backchannel.SendAsync(request, Context.RequestAborted);
            if (!response.IsSuccessStatusCode) {
                Logger.LogError("An error occurred when retrieving an access token: the remote server " +
                               "returned a {Status} response with the following payload: {Headers} {Body}.",
                               /* Status: */ response.StatusCode,
                               /* Headers: */ response.Headers.ToString(),
                               /* Body: */ await response.Content.ReadAsStringAsync());

                return OAuthTokenResponse.Failed(new Exception("An error occurred when retrieving an access token."));
            }
            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
             
            return OAuthTokenResponse.Success(payload);
        }
    }
}
