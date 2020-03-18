using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace K_Means
{
    class LockedBitmap
    {
        public Bitmap bitmap = null;
        private Rectangle retangulo;
        private IntPtr ponteiro;
        private byte[] pixels = null;
        private BitmapData btmData = null;

        public LockedBitmap(int Width, int Height)
        {
            if (this.bitmap == null)
            {
                this.bitmap = new Bitmap(Width, Height);
                this.retangulo = new Rectangle(new Point(0, 0), bitmap.Size);
            }
        }

        public LockedBitmap(Bitmap bitmap)
        {
            if (this.bitmap == null)
            {
                this.bitmap = new Bitmap(bitmap);
                this.retangulo = new Rectangle(new Point(0, 0), bitmap.Size);
            }
        }

        public static implicit operator LockedBitmap(Bitmap bitmap)
        {
            return new LockedBitmap(bitmap);
        }

        public void LockBits()
        {
            btmData = bitmap.LockBits(retangulo, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            ponteiro = btmData.Scan0;
            pixels = new byte[Math.Abs(btmData.Stride) * bitmap.Height];
            System.Runtime.InteropServices.Marshal.Copy(ponteiro, pixels,
                0, Math.Abs(btmData.Stride) * bitmap.Height);
        }

        public void UnlockBits()
        {
            ponteiro = btmData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(pixels, 0,
                ponteiro, Math.Abs(btmData.Stride) * bitmap.Height);

            bitmap.UnlockBits(btmData);
        }

        public Color GetPixel(int Row, int Col)
        {
            int Channel = Bitmap.GetPixelFormatSize(btmData.PixelFormat);
            int Pixel = (Row + Col * bitmap.Width) * (Channel / 8);

            int Red = 0, Green = 0, Blue = 0, Alpha = 0;

            if (Channel == 32)
            {
                Blue = pixels[Pixel];
                Green = pixels[Pixel + 1];
                Red = pixels[Pixel + 2];
                Alpha = pixels[Pixel + 3];
            }

            else if (Channel == 24)
            {
                Blue = pixels[Pixel];
                Green = pixels[Pixel + 1];
                Red = pixels[Pixel + 2];
            }

            else if (Channel == 16)
            {
                Blue = pixels[Pixel];
                Green = pixels[Pixel + 1];
            }

            else if (Channel == 8)
                Blue = pixels[Pixel];

            return (Channel != 8) ? Color.FromArgb(Red, Green, Blue) : Color.FromArgb(Blue, Blue, Blue);
        }

        public void SetPixel(int Row, int Col, Color Clr)
        {
            int Channel = Bitmap.GetPixelFormatSize(btmData.PixelFormat);
            int Pixel = (Row + Col * bitmap.Width) * (Channel / 8);

            if (Channel == 32)
            {
                pixels[Pixel] = Clr.B;
                pixels[Pixel + 1] = Clr.G;
                pixels[Pixel + 2] = Clr.R;
                pixels[Pixel + 3] = Clr.A;
            }

            else if (Channel == 24)
            {
                pixels[Pixel] = Clr.B;
                pixels[Pixel + 1] = Clr.G;
                pixels[Pixel + 2] = Clr.R;
            }

            else if (Channel == 16)
            {
                pixels[Pixel] = Clr.B;
                pixels[Pixel + 1] = Clr.G;
            }

            else if (Channel == 8)
                pixels[Pixel] = Clr.B;
        }

        public int Width { get { return bitmap.Width; } }

        public int Height { get { return bitmap.Height; } }
 
        public Bitmap getBitMap() { return new Bitmap(bitmap); }
    }
}
