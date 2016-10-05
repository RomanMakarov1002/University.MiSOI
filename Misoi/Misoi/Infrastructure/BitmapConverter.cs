using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Misoi.Models;

namespace Misoi.Infrastructure
{
    public static class BitmapConverter
    {
        public static string SaveFileToDisk(HttpPostedFileBase img, string mapPath, LoadedImage image)
        {
            Directory.CreateDirectory(mapPath + "/UserContent/");
            var ext = "." + img.FileName.Split('.').Last();
            var name = Guid.NewGuid().ToString() + ext;
            var fName = mapPath + "UserContent\\" + name;
            image.Format = ext;
            image.ImagePath = "/UserContent/" + name;
            image.ImagePhysicalPath = fName;
            img.SaveAs(fName);
            return fName;
        }

        public static Image GetImage(string path)
        {
            return Image.FromFile(path);
        }

        public static ImageChart GetChart(Image img)
        {
            var result = new ImageChart();
            result.Brightness = new int[256];
            result.BrightnessPersentage = new double[256];
            result.PixelsCount = img.Width * img.Height;
            Bitmap bitmap = new Bitmap(img);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite,
                bitmap.PixelFormat);
            IntPtr ptr = bitmapData.Scan0;
            var bytes = Math.Abs(bitmapData.Stride) * bitmapData.Height;
            byte[] resultArray = new byte[bytes];
            Marshal.Copy(ptr, resultArray, 0, bytes);
            bitmap.UnlockBits(bitmapData);
            for (int i = 0; i < bytes; i += 4)
            {
                result.Brightness[
                    Convert.ToInt32(
                        Math.Round(resultArray[i]*0.2126 + resultArray[i + 1]*0.7152 + resultArray[i + 2]*0.0722))]++;
                //(0.2126 * R + 0.7152 * G + 0.0722 * B     Luma formula
            }

            for (int i = 0; i < 256; i++)
            {
                result.BrightnessPersentage[i] = Math.Round((double)result.Brightness[i] / result.PixelsCount * 100, 2);
            }
            result.MaxPersentage = result.BrightnessPersentage.Max();
            for (int i = 0; i < 256; i++)
            {
                result.BrightnessPersentage[i] = Math.Round((double)result.BrightnessPersentage[i] / result.MaxPersentage * 100, 2);
            }
            return result;
        }     
        
        public static string MakeNegative(Image imgg, LoadedImage image, string mappath)
        {
            if (image.Negative != null)
                return image.Negative;
            Bitmap img = new Bitmap(imgg);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, 
                img.PixelFormat);
            IntPtr ptr = bitmapData.Scan0;
            var bytes = Math.Abs(bitmapData.Stride)*bitmapData.Height;
            byte[] resultArray = new byte[bytes];
            Marshal.Copy(ptr, resultArray, 0, bytes);
            int val = 255;
            for (int i = 0; i < bytes; i += 4)
            {
                resultArray[i] = Convert.ToByte(val - resultArray[i]);
                resultArray[i + 1] = Convert.ToByte(val - resultArray[i + 1]);
                resultArray[i + 2] = Convert.ToByte(val - resultArray[i + 2]);
            }
            Marshal.Copy(resultArray, 0, ptr, bytes);
            img.UnlockBits(bitmapData);
            var name = "Negative" + Guid.NewGuid().ToString() + image.Format;
            var path = mappath + "UserContent\\" + name;
            img.Save(path);
            sw.Stop();
            var time = sw.Elapsed;
            image.Negative = "/UserContent/" + name;
            return image.Negative;
        }

        public static string AddFilter(string mappath, LoadedImage image, Image source, double[,] xFilterMatrix, double[,] yFilterMatrix, double factor = 1, int bias = 0, bool grayscale = false)
        {
            if (image.ImageFiltered != null)
                return image.ImageFiltered;
            Bitmap sourceBitmap = new Bitmap(source);
            BitmapData sourceData =
                           sourceBitmap.LockBits(new Rectangle(0, 0,
                           sourceBitmap.Width, sourceBitmap.Height),
                                             ImageLockMode.ReadOnly,
                                        PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            if (grayscale == true)
            {
                float rgb = 0;
                for (int k = 0; k < pixelBuffer.Length; k += 4)
                {
                    rgb = pixelBuffer[k] * 0.11f;
                    rgb += pixelBuffer[k + 1] * 0.59f;
                    rgb += pixelBuffer[k + 2] * 0.3f;

                    pixelBuffer[k] = (byte)rgb;
                    pixelBuffer[k + 1] = pixelBuffer[k];
                    pixelBuffer[k + 2] = pixelBuffer[k];
                    pixelBuffer[k + 3] = 255;
                }
            }
            double blueX = 0.0;
            double greenX = 0.0;
            double redX = 0.0;
            double blueY = 0.0;
            double greenY = 0.0;
            double redY = 0.0;
            double blueTotal = 0.0;
            double greenTotal = 0.0;
            double redTotal = 0.0;
            int filterOffset = 1;
            int calcOffset = 0;
            int byteOffset = 0;

            for (int offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++)
                {
                    blueX = greenX = redX = 0;
                    blueY = greenY = redY = 0;
                    blueTotal = greenTotal = redTotal = 0.0;
                    byteOffset = offsetY * sourceData.Stride + offsetX * 4;
                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset + (filterX * 4) + (filterY * sourceData.Stride);
                            blueX += (double)(pixelBuffer[calcOffset]) * xFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            greenX += (double)(pixelBuffer[calcOffset + 1]) * xFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            redX += (double)(pixelBuffer[calcOffset + 2]) * xFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            blueY += (double)(pixelBuffer[calcOffset]) * yFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            greenY += (double)(pixelBuffer[calcOffset + 1]) * yFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            redY += (double)(pixelBuffer[calcOffset + 2]) * yFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                        }
                    }

                    blueTotal = Math.Sqrt((blueX * blueX) + (blueY * blueY));
                    greenTotal = Math.Sqrt((greenX * greenX) + (greenY * greenY));
                    redTotal = Math.Sqrt((redX * redX) + (redY * redY));
                    if (blueTotal > 255)
                    {
                        blueTotal = 255;
                    }
                    else if (blueTotal < 0)
                    {
                        blueTotal = 0;
                    }

                    if (greenTotal > 255)
                    {
                        greenTotal = 255;
                    }
                    else if (greenTotal < 0)
                    {
                        greenTotal = 0;
                    }
                    if (redTotal > 255)
                    {
                        redTotal = 255;
                    }
                    else if (redTotal < 0)
                    {
                        redTotal = 0;
                    }
                    resultBuffer[byteOffset] = (byte)(blueTotal);
                    resultBuffer[byteOffset + 1] = (byte)(greenTotal);
                    resultBuffer[byteOffset + 2] = (byte)(redTotal);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            BitmapData resultData =
                       resultBitmap.LockBits(new Rectangle(0, 0,
                       resultBitmap.Width, resultBitmap.Height),
                                        ImageLockMode.WriteOnly,
                                    PixelFormat.Format32bppArgb);
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);
            var name = "Filtered" + Guid.NewGuid().ToString() + image.Format;
            var path = mappath + "UserContent\\" + name;
            resultBitmap.Save(path);

            image.ImageFiltered = "/UserContent/" + name;
            return image.ImageFiltered;
        }
    }
}