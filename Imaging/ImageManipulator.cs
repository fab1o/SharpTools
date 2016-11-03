using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Fabio.SharpTools.Imaging
{
    public sealed class ImageManipulator : IDisposable
    {
        #region privare properties
        Byte[] imageContent = null;
        Graphics g = null;
        Bitmap bitmap = null;
        OctreeQuantizer quantizer = null;
        #endregion

        #region public properties
        private MemoryStream imageResult = null;
        public MemoryStream ImageResult
        {
            get
            {
                if (imageResult == null)
                    imageResult = new MemoryStream();

                return imageResult;
            }
            set
            {
                imageResult = value;
            }
        }
        #endregion

        #region constructors
        public ImageManipulator(Byte[] imageContent)
        {
            this.imageContent = imageContent;
            this.quantizer = new OctreeQuantizer(255, 8);
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            imageContent = null;

            if (g != null)
                g.Dispose();

            if (bitmap != null)
                bitmap.Dispose();

            if (ImageResult != null)
                ImageResult.Dispose();
        }
        #endregion

        #region public Methods

        public static byte[] ReadImage(string p_postedImageFileName, string[] p_fileType)
        {
            bool isValidFileType = false;

            try
            {
                FileInfo file = new FileInfo(p_postedImageFileName);

                foreach (string strExtensionType in p_fileType)
                {
                    if (strExtensionType == file.Extension)
                    {
                        isValidFileType = true;
                        break;
                    }
                }

                if (isValidFileType)
                {

                    FileStream fs = new FileStream(p_postedImageFileName, FileMode.Open, FileAccess.Read);

                    BinaryReader br = new BinaryReader(fs);

                    byte[] image = br.ReadBytes((int)fs.Length);

                    br.Close();
                    fs.Close();

                    return image;

                }

                return null;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void AddWatermark(string WatermarkText)
        {
            if (imageContent == null || imageContent.Length == 0)
                return;

            if (g == null)
            {
                if (ImageResult == null || ImageResult.Length == 0)
                    bitmap = new Bitmap(new MemoryStream(imageContent));
                else
                    bitmap = new Bitmap(ImageResult);

                g = Graphics.FromImage(bitmap);

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(bitmap, 0, 0);
            }

            Font font = new Font("Arial", (bitmap.Width * 0.1f) + 2, FontStyle.Bold, GraphicsUnit.Pixel);
            SolidBrush brush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)); //intensity
            Matrix matrix = new Matrix();
            matrix.Rotate(-45, MatrixOrder.Append); //rotation angle
            g.Transform = matrix;
            g.DrawString(WatermarkText, font, brush, new Point((bitmap.Height / 2) * -1, bitmap.Height - (int)(bitmap.Height * 0.3f)));

            Bitmap quantized = quantizer.Quantize(bitmap);

            ImageResult = new MemoryStream(bitmap.Width * bitmap.Height);

            quantized.Save(ImageResult, ImageFormat.Png);
            quantized.Dispose();

        }


        public void AddImage(byte[] image)
        {
            if (image == null || image.Length == 0)
                return;

            Bitmap pic = new Bitmap(new MemoryStream(image));

            AddImage(image, 0, 0, 0, pic.Width, pic.Height);

            pic.Dispose();
        }

        public void AddImage(byte[] image, int Width, int Height)
        {
            AddImage(image, Width, Height, 0, 0, 0);
        }

        public void AddImage(byte[] image, int Width, int Height, int X, int Y, int Rotation)
        {
            if (imageContent == null || imageContent.Length == 0)
                return;

            if (Width == 0 || Height == 0)
                return;

            if (image == null || image.Length == 0)
                return;

            if (g == null)
            {
                if (ImageResult == null || ImageResult.Length == 0)
                    bitmap = new Bitmap(new MemoryStream(imageContent));
                else
                    bitmap = new Bitmap(ImageResult);

                g = Graphics.FromImage(bitmap);

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(bitmap, 0, 0);
            }

            Image imageOriginal = Image.FromStream(new MemoryStream(image));

            if (Rotation > 0)
            {
                Matrix matrix = new Matrix();
                matrix.Rotate(Rotation * -1, MatrixOrder.Append);
                g.Transform = matrix;
            }

            g.DrawImage(imageOriginal, new Rectangle(X, Y, Width, Height), 0, 0,
                imageOriginal.Width, imageOriginal.Height,
                GraphicsUnit.Pixel);

            ImageResult = new MemoryStream(bitmap.Width * bitmap.Height);

            bitmap.Save(ImageResult, ImageFormat.Png);

            imageOriginal.Dispose();

        }

        public void AddTriangle(int X1, int Y1, int X2, int Y2, int X3, int Y3, string HtmlColor = null)
        {
            if (imageContent == null || imageContent.Length == 0)
                return;

            if (g == null)
            {
                if (ImageResult == null || ImageResult.Length == 0)
                    bitmap = new Bitmap(new MemoryStream(imageContent));
                else
                    bitmap = new Bitmap(ImageResult);

                g = Graphics.FromImage(bitmap);

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(bitmap, 0, 0);
            }

            Point[] points = new Point[3];
            points[0] = new Point(X1, Y1);
            points[1] = new Point(X2, Y2);
            points[2] = new Point(X3, Y3);

            if (!string.IsNullOrWhiteSpace(HtmlColor))
            {
                if (HtmlColor[0] != '#')
                    HtmlColor = "#" + HtmlColor;

                System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(HtmlColor);

                var brush = new SolidBrush(color);
                g.FillPolygon(brush, points);
            }
            else
            {
                var pen = new Pen(Color.Black);
                g.DrawPolygon(pen, points);
            }

            ImageResult = new MemoryStream(bitmap.Width * bitmap.Height);

            bitmap.Save(ImageResult, ImageFormat.Png);

        }

        public void AddRectangle(int Width, int Height, int X, int Y, int Rotation, string HtmlColor = null)
        {
            if (imageContent == null || imageContent.Length == 0)
                return;

            if (Width == 0 || Height == 0)
                return;

            if (g == null)
            {
                if (ImageResult == null || ImageResult.Length == 0)
                    bitmap = new Bitmap(new MemoryStream(imageContent));
                else
                    bitmap = new Bitmap(ImageResult);

                g = Graphics.FromImage(bitmap);

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(bitmap, 0, 0);
            }

            if (Rotation > 0)
            {
                Matrix matrix = new Matrix();
                matrix.Rotate(Rotation * -1, MatrixOrder.Append);
                g.Transform = matrix;
            }

            if (!string.IsNullOrWhiteSpace(HtmlColor))
            {
                if (HtmlColor[0] != '#')
                    HtmlColor = "#" + HtmlColor;

                System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(HtmlColor);

                var brush = new SolidBrush(color);
                g.FillRectangle(brush, X, Y, Width, Height);
            }
            else
            {
                var pen = new Pen(Color.Black);
                g.DrawRectangle(pen, X, Y, Width, Height);
            }

            ImageResult = new MemoryStream(bitmap.Width * bitmap.Height);

            bitmap.Save(ImageResult, ImageFormat.Png);

        }

        public void GrayScale()
        {
            if (imageContent == null || imageContent.Length == 0)
                return;

            if (g == null)
            {
                if (ImageResult == null || ImageResult.Length == 0)
                    bitmap = new Bitmap(new MemoryStream(imageContent));
                else
                    bitmap = new Bitmap(ImageResult);

                g = Graphics.FromImage(bitmap);

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                //g.DrawImage(bitmap, 0, 0);
            }

            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
                  {
                     new float[] {.3f, .3f, .3f, 0, 0},
                     new float[] {.59f, .59f, .59f, 0, 0},
                     new float[] {.11f, .11f, .11f, 0, 0},
                     new float[] {0, 0, 0, 1, 0},
                     new float[] {0, 0, 0, 0, 1}
                  });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Width,
                GraphicsUnit.Pixel, attributes);


            ImageResult = new MemoryStream(bitmap.Width * bitmap.Height);

            bitmap.Save(ImageResult, ImageFormat.Png);

        }

        public void ShadeColor(string NewColorInHtml)
        {
            if (imageContent == null || imageContent.Length == 0)
                return;

            Color newColor = Color.Transparent;

            if (!string.IsNullOrWhiteSpace(NewColorInHtml))
            {
                if (NewColorInHtml[0] != '#')
                    NewColorInHtml = "#" + NewColorInHtml;

                newColor = ColorTranslator.FromHtml(NewColorInHtml);
            }

            if (g == null)
            {
                if (ImageResult == null || ImageResult.Length == 0)
                    bitmap = new Bitmap(new MemoryStream(imageContent));
                else
                    bitmap = new Bitmap(ImageResult);

                g = Graphics.FromImage(bitmap);

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                //g.DrawImage(bitmap, 0, 0);
            }

            float r = (1.0f / 255) * newColor.R;
            float b = (1.0f / 255) * newColor.B;
            float gr = (1.0f / 255) * newColor.G;

            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][]
                {
                    new float[] {r, gr, b, 0, 0},         // red scaling factor of 2
                    new float[] {0,  0,  0,  0, 0},       // green scaling factor of 1
                    new float[] {0,  0, 0,  0, 0},       // blue scaling factor of 1
                    new float[] {0,  0,  0,  1, 0},       // alpha scaling factor of 1
                    new float[] {0, 0, 0, 0, 1}
                });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Width,
                GraphicsUnit.Pixel, attributes);


            ImageResult = new MemoryStream(bitmap.Width * bitmap.Height);

            bitmap.Save(ImageResult, ImageFormat.Png);

        }

        /// <summary>
        /// Resize all kinds of bitmap, quantize (optional), tranform it and change its color
        /// </summary>
        public void Resize(int Width, int Height, bool Quantize = false)
        {
            if (imageContent == null || imageContent.Length == 0)
                return;

            ImageResult = new MemoryStream(imageContent);

            if (Width == 0 || Height == 0)
                return;

            try
            {
                Image imageOriginal = Image.FromStream(ImageResult);

                bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
                g = Graphics.FromImage(bitmap);

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(imageOriginal, new Rectangle(0, 0, Width, Height), 0, 0,
                    imageOriginal.Width, imageOriginal.Height,
                    GraphicsUnit.Pixel);

                ImageResult = new MemoryStream(Width * Height);

                if (Quantize)
                {
                    Bitmap quantized = quantizer.Quantize(bitmap);
                    quantized.Save(ImageResult, ImageFormat.Png);
                    quantized.Dispose();
                }
                else
                {
                    bitmap.Save(ImageResult, ImageFormat.Png);
                }

                imageOriginal.Dispose();

            }
            catch { }

        }

        /// <summary>
        /// Crop and resizes all kinds of bitmap, quantize (optional), tranform it and change its color
        /// </summary>
        public void CropAndResize(int X, int Y, int CropWidth, int CropHeight, int Width, int Height, bool Quantize = false)
        {
            if (imageContent == null || imageContent.Length == 0)
                return;

            ImageResult = new MemoryStream(imageContent);

            if (Width == 0 || Height == 0)
                return;

            if (CropWidth == 0 || CropHeight == 0)
                return;

            try
            {
                Image imageOriginal = Image.FromStream(ImageResult);

                bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
                g = Graphics.FromImage(bitmap);

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                if (X == 0 && Y == 0 && imageOriginal.Width == Width && imageOriginal.Height == Height)
                {
                    g.DrawImage(imageOriginal, 0, 0);
                }
                else
                {
                    g.DrawImage(imageOriginal, new Rectangle(0, 0, Width, Height), X, Y,
                        CropWidth, CropHeight,
                        GraphicsUnit.Pixel);
                }

                ImageResult = new MemoryStream(Width * Height);

                if (Quantize)
                {
                    Bitmap quantized = quantizer.Quantize(bitmap);
                    quantized.Save(ImageResult, ImageFormat.Png);
                    quantized.Dispose();
                }
                else
                {
                    bitmap.Save(ImageResult, ImageFormat.Png);
                }

                imageOriginal.Dispose();

            }
            catch { }

        }
        #endregion
    }

    public class ImageManipulatorException : Exception
    {
        public ImageManipulatorException() : base() { }
        public ImageManipulatorException(string message) : base(message) { }
    }
}

