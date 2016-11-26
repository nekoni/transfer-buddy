
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.WebUtilities;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TransferBuddy.Service.Controllers
{

    /// <summary>
    /// The transfers controller.
    /// </summary>
    public class TransfersController : Controller
    {
        private const string TransfersAPI = "https://test-restgw.transferwise.com/v1/transfers"; 
       
        private const string Quotes = "https://test-restgw.transferwise.com/v1/quotes";

        private const string Profiles = "https://test-restgw.transferwise.com/v1/profiles";

        private const string Accounts = "https://test-restgw.transferwise.com/v1/accounts";

        static string[] currencies = new string [] { "eur", "usd", "gbp", "sek", "aud", "chf", "dkk", "jpy", "czk", "hkd", "nok", "pln", "nzd", "rub" };

        private string quote = "";

        /// <summary>
        /// Executes the transfer.
        /// </summary>
        public async void MakeTransfer()
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)

            {  
                var token = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Authentication).FirstOrDefault();
                var id = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

                this.quote = await CreateQuote(token.Value, id.Value); 
    
                var sourceAccountValue = "111";
                var targetAccountValue = "111"; 
                var referenceValue = "Early Christmas gift";
                var payInMethodValue =  "transfer";
                var request = new HttpRequestMessage(HttpMethod.Post, TransfersAPI);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
  
                var payload = new 
                { 
                    targetAccount = targetAccountValue,
                    quote = this.quote,
                    reference = referenceValue,
                    payInMethod = payInMethodValue
                }; 

                var content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                var httpClient = new HttpClient(); 
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
                
                HttpResponseMessage response  = await httpClient.PostAsync(Quotes, content);
                
                if (response.IsSuccessStatusCode)
                { 
                    var responsetext = await response.Content.ReadAsStringAsync(); 
                }

            }
        }

        [Route("api/transfer")]
        public async void CreateTransfer()
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {  
                var token = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Authentication).FirstOrDefault();
                var id = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

                var accountsResponseText = await GetAccounts(token.Value, id.Value);
                
                var array = new []{
                    new { currency = string.Empty}
                };

                array = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(accountsResponseText, array);
                dynamic returnelement = Newtonsoft.Json.JsonConvert.DeserializeObject<object[]>(accountsResponseText);
                var sourceCurrency = array[1].currency;
                var targetCurrency = array[0].currency;
                var outputCurrrencies = String.Format("{0}-{1}", sourceCurrency, targetCurrency).ToString();
  
                var targetAccountValue = (string) returnelement[0].id;
                if(outputCurrrencies == string.Empty || targetAccountValue == string.Empty)
                {
                    return;
                }
 
                this.quote = await CreateQuote(token.Value, id.Value, sourceCurrency, targetCurrency);  
                // should be currencies = "eur-gbp";
            
                var payload = new 
                {
                    quote = this.quote,
                    targetAccount = targetAccountValue,
                    targetAmount = "1000",
                     details = new {
                        reference = outputCurrrencies
                    }
                }; 

                var content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                var httpClient = new HttpClient(); 
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
                
                HttpResponseMessage response  = await httpClient.PostAsync(TransfersAPI, content);
                
                if (response.IsSuccessStatusCode)
                { 
                    var responsetext = await response.Content.ReadAsStringAsync();
                    var json = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(responsetext);  
                } 
            }
        }

        public async Task<string> GetAccounts(string token, string id)
        { 
            var content = new System.Net.Http.StringContent("", Encoding.UTF8, "application/json");

            var httpClient = new HttpClient(); 
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            HttpResponseMessage response  = await httpClient.GetAsync(Accounts);
            if (response.IsSuccessStatusCode)
            { 
                var responseText = await response.Content.ReadAsStringAsync();
                 
                return responseText;
            }
            
            return null;
        }

        public async Task<string> CreateQuote(string token, string id, string sourceValue = "EUR", string targetValue = "SEK")
        {   
            var payload = new 
            {
                profile = id,
                source = sourceValue,
                target = targetValue,
                targetAmount = "1000",
                rateType = "FIXED"
            }; 

            var content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var httpClient = new HttpClient(); 
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            HttpResponseMessage response  = await httpClient.PostAsync(Quotes, content);
              
            if (response.IsSuccessStatusCode)
            { 
                var responsetext = await response.Content.ReadAsStringAsync();
                dynamic element = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(responsetext);

                var quoteId = (string) element.id;

                return quoteId;
            }

            return "Error";
        }

        public async void CreateProfile(string token)
        { 
            var payInMethod =  "transfer";
            var request = new HttpRequestMessage(HttpMethod.Post, TransfersAPI);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var details = new Dictionary<string, string>{
                ["firstName"] = "John",
                ["lastName"] = "Doe",
                ["dateOfBirth"] = "1983-08-06",
                ["phoneNumber"] = "+372111111",
                ["occupation"] = "student",
                ["primaryAddress"] = "1"
            }; 
           
            var httpClient = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            HttpResponseMessage response  = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, HttpContext.RequestAborted);
            var responsetext = await response.Content.ReadAsStringAsync();
        }
    }
}