namespace TransferWise.Client
{
    public partial class TransferWiseClient
    {
        private readonly string serviceUri;

        public TransferWiseClient(string serviceUri)
        {
            this.serviceUri = serviceUri;
        }
    }
}