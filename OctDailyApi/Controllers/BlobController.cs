using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OctDailyApi.Models;

[ApiController]
[Route("api/[controller]")]
public class BlobController : ControllerBase
{
    private readonly AzureBlobStorageSettings _blobStorageSettings;

    public BlobController(IOptions<AzureBlobStorageSettings> blobStorageSettings)
    {
        _blobStorageSettings = blobStorageSettings.Value;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage([FromBody] Base64ImageRequest request)
    {
        if (string.IsNullOrEmpty(request?.Base64Image))
            return BadRequest("Image is required.");

        try
        {
            // Decode the base64 image
            var imageBytes = Convert.FromBase64String(request.Base64Image);

            // Generate a unique image name
            string imageName = $"{Guid.NewGuid()}.jpg";

            // Create a BlobContainerClient
            var containerClient = new BlobContainerClient(new Uri($"{_blobStorageSettings.BlobServiceEndpoint}/{_blobStorageSettings.ContainerName}?{_blobStorageSettings.SasToken}"));

            // Get a reference to the blob
            var blobClient = containerClient.GetBlobClient(imageName);

            // Upload the image to blob storage
            using (var stream = new MemoryStream(imageBytes))
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            // Return the image name in the response
            return Ok(new { ImageName = imageName });
        }
        catch (FormatException)
        {
            return BadRequest("Invalid Base64 string.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
    [HttpGet("get/{imageName}")]
    public async Task<IActionResult> GetImage(string imageName)
    {
        try
        {
            // Create a BlobContainerClient
            var containerClient = new BlobContainerClient(new Uri($"{_blobStorageSettings.BlobServiceEndpoint}/{_blobStorageSettings.ContainerName}?{_blobStorageSettings.SasToken}"));

            // Get a reference to the blob
            var blobClient = containerClient.GetBlobClient(imageName);

            // Check if the blob exists
            if (!await blobClient.ExistsAsync())
                return NotFound("Blob not found.");

            // Download the blob
            var downloadResponse = await blobClient.DownloadAsync();

            // Read the blob's content into a memory stream
            using (var memoryStream = new MemoryStream())
            {
                await downloadResponse.Value.Content.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                // Convert the image bytes to Base64
                var base64Image = Convert.ToBase64String(imageBytes);

                // Return the Base64 string in the response
                return Ok(new { Base64Image = base64Image });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
