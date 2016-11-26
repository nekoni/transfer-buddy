using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TransferWise.Client
{
    public partial class TransferWiseClient
    {
        public async void CreateProfile(string token)
        { 
            var payload = new 
            {
                firstName = "John",
                lastName = "Doe",
                dateOfBirth = "1983-08-06",
                phoneNumber = "+372111111",
                occupation = "student",
                primaryAddress  = "1"
            }; 

            var content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var httpClient = new HttpClient(); 
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            HttpResponseMessage response  = await httpClient.PostAsync(this.serviceUri, content);

            if (response.IsSuccessStatusCode)
            { 
                var responsetext = await response.Content.ReadAsStringAsync();
                
            }
        }
    }
}