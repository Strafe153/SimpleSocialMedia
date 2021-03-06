using Microsoft.AspNetCore.Http;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;

namespace SimpleSocialMedia.Data
{
    public static class PictureUtility
    {
        public static byte[] ResizeImage(byte[] imageToResize, int height)
        {
            byte[] resizedImage = null;

            using (var memoryStream = new MemoryStream())
            {
                using (Image image = Image.Load(imageToResize))
                {
                    double coefficient = (double)image.Height / height;
                    double width = image.Width / coefficient;

                    image.Mutate(img => img.Resize((int)width, height));
                    image.Save(memoryStream, new PngEncoder());
                }

                resizedImage = memoryStream.ToArray();
            }

            return resizedImage;
        }

        public static IFormFile ConvertByteArrayToIFormFile(byte[] bytePicture)
        {
            var stream = new MemoryStream(bytePicture);
            var formFilePicture = new FormFile(stream, 0, bytePicture.Length,
                "default_profile_picture", "default_profile_pic.jpg");

            return formFilePicture;
        }

        public static byte[] ConvertIFormFileToByteArray(IFormFile formFilePicture, byte[] bytePicture)
        {
            if (formFilePicture != null)
            {
                using (var binaryReader = new BinaryReader(formFilePicture.OpenReadStream()))
                {
                    bytePicture = binaryReader.ReadBytes((int)formFilePicture.Length);
                }
            }

            return bytePicture;
        }
    }
}
