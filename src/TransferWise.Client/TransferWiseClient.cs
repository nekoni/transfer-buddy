namespace TransferWise.Client
{
    public partial class TransferWiseClient
    {
        public const string TransfersAPI = "https://test-restgw.transferwise.com/v1/transfers"; 
       
        public const string Quotes = "https://test-restgw.transferwise.com/v1/quotes";

        public const string Profiles = "https://test-restgw.transferwise.com/v1/profiles";

        public const string Rates = "https://api.transferwise.com/v1/rates";


        private readonly string serviceUri;

        public TransferWiseClient(string serviceUri)
        {
            this.serviceUri = serviceUri;
        }
    }
}