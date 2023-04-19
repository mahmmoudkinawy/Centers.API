namespace Centers.API.Services;
public sealed class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(IConfiguration config)
    {
        _cloudinary = new Cloudinary(
            new Account
            {
                Cloud = config[Constants.CloudinarySettings.CloudName],
                ApiKey = config[Constants.CloudinarySettings.ApiKey],
                ApiSecret = config[Constants.CloudinarySettings.ApiSecret]
            });
    }

    public async Task<string> UploadPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Width(600).Height(600).Crop("fill").Gravity("face")
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult.Url.AbsoluteUri;
    }


}
