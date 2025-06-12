using HalconDotNet;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace FactoryCube.Vision.Vision
{
    public static class HalconImageConverter
    {
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