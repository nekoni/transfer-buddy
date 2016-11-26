using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TransferWise.Client
{
    public partial class TransferWiseClient
    {
        public async Task<IEnumerable<Rate>> GetRatesAsync(string source, string target, DateTime startPeriod, DateTime endPeriod)
        {
            var from = startPeriod.ToString("yyyy-MM-dd");
            var to = endPeriod.ToString("yyyy-MM-dd");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var url = $"{this.serviceUri}/v1/rates/?source={source}&target={target}&from={from}&to={to}";
            var json = await client.GetStringAsync(url);

            return JsonConvert.DeserializeObject<Rate[]>(json);
        }
    }
}