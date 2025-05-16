using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Processing;
using System.Net;

namespace Infrastructure.Utils;

public sealed class S3StorageUtils(IAmazonS3 s3Client, IConfiguration configuration)
{
    private readonly string _bucketName = configuration["AWS:BucketName"]!;

    public async Task<string?> UploadPhotosAsync(
        string base64Photo,
        string fileName,
        string folder = "photos")
    {
        try
        {
            var photoBytes = Convert.FromBase64String(base64Photo);
            var contentType = GetContentType(photoBytes);
            var fileExtension = GetFileExtension(contentType);

            var originalFilePath = $"{folder}/{fileName}.{fileExtension}";
            var originalUploadResult = await UploadSinglePhotoToS3(photoBytes, originalFilePath, contentType);

            if (originalUploadResult == null)
                return null;

            var thumbnail256 = ResizeImage(photoBytes, 256, fileExtension);
            var thumbnail600 = ResizeImage(photoBytes, 600, fileExtension);

            var thumbnail256Path = $"{folder}/{fileName}_256.jpg";
            var thumbnail600Path = $"{folder}/{fileName}_600.jpg";

            var thumbnail256UploadResult = await UploadSinglePhotoToS3(thumbnail256, thumbnail256Path, contentType);
            var thumbnail600UploadResult = await UploadSinglePhotoToS3(thumbnail600, thumbnail600Path, contentType);

            return originalUploadResult;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке фотографий: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> UploadPhotoAsync(
        string base64Photo,
        string fileName,
        string folder = "photos")
    {
        try
        {
            var photoBytes = Convert.FromBase64String(base64Photo);
            var contentType = GetContentType(photoBytes);
            var fileExtension = GetFileExtension(contentType);

            var originalFilePath = $"{folder}/{fileName}.{fileExtension}";
            var originalUploadResult = await UploadSinglePhotoToS3(photoBytes, originalFilePath, contentType);

            if (originalUploadResult == null)
                return null;

            return originalUploadResult;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке фотографии: {ex.Message}");
            return null;
        }
    }

    private async Task<string?> UploadSinglePhotoToS3(
        byte[] photoBytes,
        string filePath,
        string contentType)
    {
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

    public async Task<bool> DeletePhoto(string photoUrl, string folder)
    {
        try
        {
            var fileName = ExtractFilePathFromUrl(photoUrl, folder);
            if (string.IsNullOrEmpty(fileName))
                return false;

            var deletedAny = false;

            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var fileKeys = new[]
            {
                $"{fileName}",
                $"{folder}/{nameWithoutExtension}_256.jpg",
                $"{folder}/{nameWithoutExtension}_600.jpg"
            };

            foreach (var key in fileKeys)
            {
                try
                {
                    var deleteRequest = new DeleteObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = key
                    };

                    await s3Client.DeleteObjectAsync(deleteRequest);
                    deletedAny = true;
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }

            return deletedAny;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении файлов: {ex.Message}");
            return false;
        }
    }

    private string ExtractFilePathFromUrl(string url, string folder)
    {
        var uri = new Uri(url);
        var path = uri.AbsolutePath.TrimStart('/');

        if (path.StartsWith(folder + "/"))
            return path;

        return string.Empty;
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

    private static string GetFileExtension(string contentType)
    {
        return contentType switch
        {
            "image/jpeg" => "jpg",
            "image/png" => "png",
            "image/gif" => "gif",
            "image/bmp" => "bmp",
            "image/tiff" => "tiff",
            _ => "bin"
        };
    }

    private byte[] ResizeImage(
        byte[] imageBytes,
        int height,
        string fileExtension)
    {
        using var image = Image.Load(imageBytes);
        var width = (int)((double)height / image.Height * image.Width);

        image.Mutate(x => x.Resize(width, height));

        using var ms = new MemoryStream();
        IImageEncoder encoder = fileExtension.ToLower() switch
        {
            "jpg" or "jpeg" => new JpegEncoder(),
            "png" => new PngEncoder(),
            "gif" => new GifEncoder(),
            "bmp" => new BmpEncoder(),
            "tiff" => new TiffEncoder(),
            _ => throw new NotSupportedException($"Формат {fileExtension} не поддерживается")
        };

        image.Save(ms, encoder);
        return ms.ToArray();
    }
}