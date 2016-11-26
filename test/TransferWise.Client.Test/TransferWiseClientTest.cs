using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TransferWise.Client.Test
{
    public class TransferWiseClientTest
    {
        [Fact]
        public async Task TestExtraction()
        {
            var client = new TransferWiseClient("https://api.transferwise.com");
            var startDate = new DateTime(2016, 11, 20);
            var endDate = new DateTime(2016, 11, 23);
            var result = await client.GetRatesAsync("EUR", "USD", startDate, endDate);
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            var rates = result.ToArray();
            var rate0 = rates[0];
            var rate2 = rates[2];


            var array = new[] { new { ok = 2 } };

            Assert.True(rate0.Time.Day == 21 && rate0.Time.Month == 11 && rate0.Time.Year == 2016);
            Assert.True(rate2.Time.Day == 23 && rate2.Time.Month == 11 && rate2.Time.Year == 2016);
        }
    }
}