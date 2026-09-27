using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HotelManagement.Exceptions;

namespace HotelManagement.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }
        private static readonly string[] AllowedImageTypes = {
                                                                "image/jpeg",
                                                                "image/png",
                                                                "image/webp"
                                                             };
        private static readonly string[] AllowedImageExtensions = {
                                                                    ".jpg",
                                                                    ".jpeg",
                                                                    ".png",
                                                                    ".webp"
                                                                  };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
        // Check Image
        private void ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0) throw new BadRequestException("Vui lòng chọn hình ảnh!");// Kiểm tra dung lượng
            if (file.Length > MaxFileSize)
                throw new BadRequestException("Kích thước hình ảnh không được vượt quá 5MB!");
            // Kiểm tra Content-Type
            if (!AllowedImageTypes.Contains(file.ContentType.ToLower()))
                throw new BadRequestException("Chỉ được phép upload ảnh JPG, JPEG, PNG hoặc WEBP!");
            // Kiểm tra extension
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedImageExtensions.Contains(extension))
                throw new BadRequestException("Định dạng hình ảnh không hợp lệ!");
        }
        // ===========================================================
        // =                      UPLOAD IMAGE                       =
        // ===========================================================
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            // Kiểm tra file
            ValidateImage(file);
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams    // Upload ảnh và trả về URL HTTPS của asset
            {
                File = new FileDescription(file.FileName, stream)
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null)
                throw new BadRequestException(uploadResult.Error.Message);
            return uploadResult.SecureUrl.ToString();
        }
        // ===========================================================
        // =                      DELETE IMAGE                       =
        // ===========================================================
        public async Task DeleteImageAsync(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return;
            try
            {
                var uri = new Uri(imageUrl);
                var path = uri.AbsolutePath;
                // Tìm phần /upload/
                const string uploadFolder = "/upload/";
                var uploadIndex = path.IndexOf(uploadFolder);
                if (uploadIndex == -1)
                    return;
                // Lấy phần sau /upload/
                var publicIdWithExtension =
                    path.Substring(uploadIndex + uploadFolder.Length);
                // Bỏ version nếu có
                if (publicIdWithExtension.StartsWith("v"))
                {
                    var slashIndex = publicIdWithExtension.IndexOf('/');
                    if (slashIndex != -1)
                        publicIdWithExtension = publicIdWithExtension.Substring(slashIndex + 1);
                }
                // Bỏ extension .jpg / .png / .webp
                var publicId = Path.ChangeExtension( publicIdWithExtension, null);
                if (string.IsNullOrWhiteSpace(publicId))
                    return;
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync( deleteParams);
                if (result.Error != null)
                    throw new BadRequestException(result.Error.Message);
            }
            catch (UriFormatException)
            {
                throw new BadRequestException("URL hình ảnh Cloudinary không hợp lệ!");
            }
        }
    }
}
