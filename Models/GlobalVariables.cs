using Azure.Storage.Blobs;
namespace AplicatieCamine.Models
{
	public static class GlobalVariables
	{
		public static bool isSetUp = false;
		public static bool IsAdmin { get; set; }
		public static string LinkStatsStud = "https://app.powerbi.com/reportEmbed?reportId=5d28aad3-09b0-4f24-8e78-6821c0b8369b&autoAuth=true&ctid=90bc7298-1c17-48c6-830b-e88b375f216d&config=eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly93YWJpLWV1cm9wZS1ub3J0aC1iLXJlZGlyZWN0LmFuYWx5c2lzLndpbmRvd3MubmV0LyJ9";
		public static string LinkStatsAdmin = "https://app.powerbi.com/reportEmbed?reportId=e36f0b85-4122-489e-9d44-7fa381fe8047&autoAuth=true&ctid=90bc7298-1c17-48c6-830b-e88b375f216d&config=eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly93YWJpLWV1cm9wZS1ub3J0aC1iLXJlZGlyZWN0LmFuYWx5c2lzLndpbmRvd3MubmV0LyJ9";
		public static BlobServiceClient BlobClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=camineuvtstorage;AccountKey=s9ifIu1cH0Y9KXCFhQTNED+VmEy1eECvG5HAFrUHWtmsO5zLC9eV1V+vj4rG2yJPntm7gOHE0baigX5YW8dQ/A==;EndpointSuffix=core.windows.net");
		public static string SendGridApiKey = "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";
		public static Student Student = null;
		public static string env;
	}
}
