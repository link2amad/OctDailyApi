namespace OctDailyApi.Models
{


    // Request model
    public class Base64ImageRequest
    {
        public string Base64Image { get; set; }
    }
    public class AzureBlobStorageSettings
    {
        public string BlobServiceEndpoint { get; set; }
        public string ContainerName { get; set; }
        public string SasToken { get; set; }
    }

}
