using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;

namespace IdentityApp.Utilities
{
    public static class PictureUtility
    {
        public static byte[] ResizeImage(byte[] imageToResize, int height)
        {
            byte[] resizedImage = null;

            using (MemoryStream memoryStream = new MemoryStream())
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
    }
}
