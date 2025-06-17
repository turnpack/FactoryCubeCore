using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace FactoryCube.UI.Helpers
{
    public static class ImageHelpers
    {
        public static BitmapImage ConvertToBitmapImage(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;

            using var stream = new MemoryStream(data);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze(); // makes it cross-thread usable
            return image;
        }

        public static byte[] BitmapSourceToByteArray(BitmapSource source)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using var stream = new MemoryStream();
            encoder.Save(stream);
            return stream.ToArray();
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();
            return image;
        }

        public static byte[] BitmapImageToByteArray(BitmapImage bitmap)
        {
            var encoder = new PngBitmapEncoder(); // Or BmpBitmapEncoder if using BMP
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using var stream = new MemoryStream();
            encoder.Save(stream);
            return stream.ToArray();
        }
    }
}
