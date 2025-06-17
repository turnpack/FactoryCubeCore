// File: FactoryCube.Vision/Helpers/HalconImageConverter.cs
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HalconDotNet;

namespace FactoryCube.Vision.Helpers
{
    public static class HalconImageConverter
    {
        /// <summary>
        /// Converts a WPF BitmapSource into a Halcon HImage (byte gray image).
        /// </summary>
        public static HImage ToHImage(byte[] bmp)
        {
            if (bmp == null)
                throw new ArgumentNullException(nameof(bmp));

            // Convert byte[] to BitmapImage
            var bitmap = new BitmapImage();
            using (var stream = new MemoryStream(bmp))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
            }

            var gray = new FormatConvertedBitmap(bitmap, PixelFormats.Gray8, null, 0);
            int width = gray.PixelWidth;
            int height = gray.PixelHeight;
            int stride = (width * gray.Format.BitsPerPixel + 7) / 8;

            byte[] pixels = new byte[height * stride];
            gray.CopyPixels(pixels, stride, 0);

            IntPtr unmanagedPtr = Marshal.AllocHGlobal(pixels.Length);
            try
            {
                Marshal.Copy(pixels, 0, unmanagedPtr, pixels.Length);
                var hImg = new HImage();
                hImg.GenImage1("byte", width, height, unmanagedPtr);
                return hImg;
            }
            finally
            {
                Marshal.FreeHGlobal(unmanagedPtr);
            }
        }



        public static BitmapImage ConvertToBitmapImage(HObject halconImage)
        {
            if (halconImage == null || !halconImage.IsInitialized()) return new BitmapImage();

            try
            {
                HOperatorSet.GetImagePointer1(halconImage, out HTuple pointer, out HTuple type, out HTuple width, out HTuple height);
                int stride = width.I;
                byte[] buffer = new byte[width.I * height.I];
                System.Runtime.InteropServices.Marshal.Copy(pointer.IP, buffer, 0, buffer.Length);

                var bitmapSource = BitmapSource.Create(width.I, height.I, 96, 96, System.Windows.Media.PixelFormats.Gray8, null, buffer, stride);

                var bitmapImage = new BitmapImage();
                using var stream = new MemoryStream();
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
            catch
            {
                return new BitmapImage();
            }
        }
    }
}
