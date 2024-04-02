// https://www.codeproject.com/Articles/15192/FastPixel-A-much-faster-alternative-to-Bitmap-SetP

using System;
using System.Drawing;
using System.Drawing.Imaging;

public class FastPixel
{
    private byte[] rgbValues;
    private BitmapData bmpData;
    private IntPtr bmpPtr;
    private bool locked = false;

    public int Width { get; }
    public int Height { get; }
    public bool IsAlphaBitmap { get; }
    public Bitmap Bitmap { get; }

    public FastPixel(Bitmap bitmap)
    {
        if ((bitmap.PixelFormat & PixelFormat.Indexed) != 0) throw new Exception("Cannot lock an Indexed image.");

        Bitmap = bitmap;
        IsAlphaBitmap = (bitmap.PixelFormat & PixelFormat.Alpha) != 0;
        Width = bitmap.Width;
        Height = bitmap.Height;
    }

    public void Lock()
    {
        if (locked) throw new Exception("Bitmap already locked.");

        Rectangle rect = new Rectangle(0, 0, Width, Height);
        bmpData = Bitmap.LockBits(rect, ImageLockMode.ReadWrite, Bitmap.PixelFormat);
        bmpPtr = bmpData.Scan0;

        int bytes = (Width * Height) * (IsAlphaBitmap ? 4 : 3);
        rgbValues = new byte[bytes];

        System.Runtime.InteropServices.Marshal.Copy(bmpPtr, rgbValues, 0, rgbValues.Length);

        locked = true;
    }

    public void Unlock(bool setPixels)
    {
        if (!locked) throw new Exception("Bitmap not locked.");

        if (setPixels)
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmpPtr, rgbValues.Length);

        Bitmap.UnlockBits(bmpData);
        locked = false;
    }

    public void Clear(Color color)
    {
        if (!locked) throw new Exception("Bitmap not locked.");

        if (IsAlphaBitmap)
        {
            for (int index = 0; index < rgbValues.Length; index += 4)
            {
                rgbValues[index] = color.B;
                rgbValues[index + 1] = color.G;
                rgbValues[index + 2] = color.R;
                rgbValues[index + 3] = color.A;
            }
        }
        else
        {
            for (int index = 0; index < rgbValues.Length; index += 3)
            {
                rgbValues[index] = color.B;
                rgbValues[index + 1] = color.G;
                rgbValues[index + 2] = color.R;
            }
        }
    }

    public void SetPixel(Point location, Color color) { SetPixel(location.X, location.Y, color); }

    public void SetPixel(int x, int y, Color color)
    {
        if (!locked) throw new Exception("Bitmap not locked.");

        if (IsAlphaBitmap)
        {
            int index = ((y * Width + x) * 4);
            rgbValues[index] = color.B;
            rgbValues[index + 1] = color.G;
            rgbValues[index + 2] = color.R;
            rgbValues[index + 3] = color.A;
        }
        else
        {
            int index = ((y * Width + x) * 3);
            rgbValues[index] = color.B;
            rgbValues[index + 1] = color.G;
            rgbValues[index + 2] = color.R;
        }
    }

    public Color GetPixel(Point location) { return GetPixel(location.X, location.Y); }

    public Color GetPixel(int x, int y)
    {
        if (!locked) throw new Exception("Bitmap not locked.");

        if (IsAlphaBitmap)
        {
            int index = ((y * Width + x) * 4);
            int b = rgbValues[index];
            int g = rgbValues[index + 1];
            int r = rgbValues[index + 2];
            int a = rgbValues[index + 3];
            return Color.FromArgb(a, r, g, b);
        }
        else
        {
            int index = ((y * Width + x) * 3);
            int b = rgbValues[index];
            int g = rgbValues[index + 1];
            int r = rgbValues[index + 2];
            return Color.FromArgb(r, g, b);
        }
    }
}

// from https://www.codeproject.com/Articles/15192/FastPixel-A-much-faster-alternative-to-Bitmap-SetP
// This code was converted from VB.net to C# by ChatGPT 3.5
