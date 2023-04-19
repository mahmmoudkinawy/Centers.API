namespace Centers.API.Services;
public interface IPhotoService
{
    Task<string> UploadPhotoAsync(IFormFile file);
}
