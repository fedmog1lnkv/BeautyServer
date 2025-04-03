using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Infrastructure.Utils;

public class S3StorageUtils(IAmazonS3 s3Client, IConfiguration configuration)
{
    private readonly string _bucketName = configuration["AWS:BucketName"]!;

    public async Task<string?> UploadPhotoAsync(
        string base64Photo,
        string fileName,
        string folder = "photos")
    {
        try
        {
            var photoBytes = Convert.FromBase64String(base64Photo);
            var contentType = GetContentType(photoBytes);

            string filePath = $"{folder}/{fileName}";

            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = filePath,
                InputStream = new MemoryStream(photoBytes),
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            };

            var response = await s3Client.PutObjectAsync(putRequest);

            return response.HttpStatusCode == HttpStatusCode.OK
                ? $"https://{_bucketName}.storage.yandexcloud.net/{filePath}"
                : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке фотографии: {ex.Message}");
            return null;
        }
    }

    private static string GetContentType(byte[] photoBytes)
    {
        if (photoBytes.Length > 2 && photoBytes[0] == 0xFF && photoBytes[1] == 0xD8)
            return "image/jpeg";

        if (photoBytes.Length > 8 && photoBytes[0] == 0x89 && photoBytes[1] == 0x50 &&
            photoBytes[2] == 0x4E && photoBytes[3] == 0x47 && photoBytes[4] == 0x0D &&
            photoBytes[5] == 0x0A && photoBytes[6] == 0x1A && photoBytes[7] == 0x0A)
            return "image/png";

        if (photoBytes.Length > 3 && photoBytes[0] == 0x47 && photoBytes[1] == 0x49 &&
            photoBytes[2] == 0x46 && (photoBytes[3] == 0x38))
            return "image/gif";

        if (photoBytes.Length > 1 && photoBytes[0] == 0x42 && photoBytes[1] == 0x4D)
            return "image/bmp";

        if (photoBytes.Length > 3 && ((photoBytes[0] == 0x49 && photoBytes[1] == 0x49) ||
                                      (photoBytes[0] == 0x4D && photoBytes[1] == 0x4D)))
            return "image/tiff";

        return "application/octet-stream";
    }
}