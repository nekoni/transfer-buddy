
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

        private string quote = "";

        /// <summary>
        /// Executes the transfer.
        /// </summary>
        [Route("api/transfer")]
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
                    sourceAccount = sourceAccountValue,
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

        public async Task<string> CreateQuote(string token, string id)
        {   
            var payload = new 
            {
                profile = id,
                source = "EUR",
                target = "SEK",
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
           
            var httpClient =new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            HttpResponseMessage response  = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, HttpContext.RequestAborted);
            var responsetext = await response.Content.ReadAsStringAsync();
        }
    }
}