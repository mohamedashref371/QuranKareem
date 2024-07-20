using System;
using System.Drawing;
using System.Drawing.Imaging;

public class SharpPixel
{
    private readonly byte[] rgbValues;
    private BitmapData bmpData;
    private IntPtr bmpPtr;
    private bool locked = false;

    private int withOriginal = 0;
    private byte[] rgbValuesCopy;
    private readonly int[] boundaries = new int[2];

    public int Width { get; }
    public int Height { get; }
    public bool IsAlphaBitmap { get; }
    public Bitmap Bitmap { get; }

    public SharpPixel(Bitmap bitmap, bool withOriginal = false)
    {
        if ((bitmap.PixelFormat & PixelFormat.Indexed) != 0) throw new Exception("Cannot lock an Indexed image.");

        Bitmap = bitmap;
        IsAlphaBitmap = (bitmap.PixelFormat & PixelFormat.Alpha) != 0;
        Width = bitmap.Width;
        Height = bitmap.Height;

        int bytes = (Width * Height) * (IsAlphaBitmap ? 4 : 3);
        rgbValues = new byte[bytes];

        this.withOriginal = withOriginal ? 1 : 0;
    }

    public void Lock()
    {
        if (locked) throw new Exception("Bitmap already locked.");

        Rectangle rect = new Rectangle(0, 0, Width, Height);
        bmpData = Bitmap.LockBits(rect, ImageLockMode.ReadWrite, Bitmap.PixelFormat);
        bmpPtr = bmpData.Scan0;

        if (withOriginal != 2)
        {
            System.Runtime.InteropServices.Marshal.Copy(bmpPtr, rgbValues, 0, rgbValues.Length);
            Reset();
        }
            
        if (withOriginal == 1)
        {
            rgbValuesCopy = (byte[])rgbValues.Clone();
            withOriginal = 2;
        }

        locked = true;
    }

    private void Reset()
    {
        boundaries[0] = int.MaxValue;
        boundaries[1] = int.MinValue;
    }

    public void Unlock(bool setPixels, bool setAsOriginal = false)
    {
        if (!locked) throw new Exception("Bitmap not locked.");

        if (setPixels)
            SetPixels(setAsOriginal);
        else if (withOriginal == 2)
            withOriginal = 1;

        Bitmap.UnlockBits(bmpData);
        locked = false;
    }

    private void SetPixels(bool setAsOriginal)
    {
        if (boundaries[1] >= 0)
        {
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, boundaries[0], IntPtr.Add(bmpPtr, boundaries[0]), boundaries[1] - boundaries[0]);
            if (withOriginal == 2 && setAsOriginal)
            {
                Array.Copy(rgbValues, boundaries[0], rgbValuesCopy, boundaries[0], boundaries[1] - boundaries[0]);
                Reset();
            }
        }
    }

    public void SetOriginal()
    {
        if (!locked) throw new Exception("Bitmap not locked.");

        if (withOriginal == 2 && boundaries[1] >= 0)
        {
            Array.Copy(rgbValuesCopy, boundaries[0], rgbValues, boundaries[0], boundaries[1] - boundaries[0]);
            Reset();
        }
    }

    public void Clear(Color color, bool withAlpha = true, bool skip0alpha = false)
    {
        if (!locked) throw new Exception("Bitmap not locked.");

        int plus = IsAlphaBitmap ? 4 : 3;
        bool alpha = IsAlphaBitmap && withAlpha;
        bool skip = IsAlphaBitmap && skip0alpha;
        byte Alpha = color.A, Red = color.R, Green = color.G, Blue = color.B;

        for (int index = 0; index < rgbValues.Length; index += plus)
        {
            if (!skip || rgbValues[index + 3] > 0)
            {
                rgbValues[index] = Blue;
                rgbValues[index + 1] = Green;
                rgbValues[index + 2] = Red;
                if (alpha) rgbValues[index + 3] = Alpha;
            }
        }

        boundaries[0] = 0;
        boundaries[1] = rgbValues.Length;
    }

    public void Clear(Color color, int x0, int y0, int x1, int y1, Color Modifiedcolor, int delta = 0, bool withAlpha = true, bool skip0alpha = false)
    {
        if (!locked) throw new Exception("Bitmap not locked.");

        int plus = IsAlphaBitmap ? 4 : 3;
        bool alpha = IsAlphaBitmap && withAlpha;
        bool skip = IsAlphaBitmap && skip0alpha;
        int index = (y0 * Width + x0) * plus;
        int finish = (y1 * Width + x1) * plus;
        byte Alpha = color.A, Red = color.R, Green = color.G, Blue = color.B;
        byte RedM = Modifiedcolor.R, GreenM = Modifiedcolor.G, BlueM = Modifiedcolor.B;

        int val;

        for (int j = y0; j <= y1; j++)
        {
            for (int i = x0; i <= x1; i++)
            {
                val = (j * Width + i) * plus;
                if ((!skip || rgbValues[val + 3] > 0) && (Modifiedcolor.IsEmpty || (Math.Abs(rgbValues[val + 2] - RedM) <= delta && Math.Abs(rgbValues[val + 1] - GreenM) <= delta && Math.Abs(rgbValues[val] - BlueM) <= delta)))
                {
                    rgbValues[val] = Blue;
                    rgbValues[val + 1] = Green;
                    rgbValues[val + 2] = Red;
                    if (alpha) rgbValues[val + 3] = Alpha;
                }
            }
        }

        if (index < boundaries[0])
            boundaries[0] = index;
        if (finish > boundaries[1])
            boundaries[1] = finish;
    }

    public void ReverseColors()
    {
        if (!locked) throw new Exception("Bitmap not locked.");

        int plus = IsAlphaBitmap ? 4 : 3;

        for (int index = 0; index < rgbValues.Length; index+= plus)
        {
            if (!IsAlphaBitmap || rgbValues[index + 3] > 0)
            {
                rgbValues[index] = (byte)(255 - rgbValues[index]);
                rgbValues[index + 1] = (byte)(255 - rgbValues[index + 1]);
                rgbValues[index + 2] = (byte)(255 - rgbValues[index + 2]);
            }
        }

        boundaries[0] = 0;
        boundaries[1] = rgbValues.Length;
    }

    public void SetPixel(Point location, Color color) => SetPixel(location.X, location.Y, color);

    public void SetPixel(int x, int y, Color color)
    {
        if (!locked) throw new Exception("Bitmap not locked.");

        int plus = IsAlphaBitmap ? 4 : 3;
        int index = (y * Width + x) * plus;

        rgbValues[index] = color.B;
        rgbValues[index + 1] = color.G;
        rgbValues[index + 2] = color.R;
        if (IsAlphaBitmap) rgbValues[index + 3] = color.A;

        if (index < boundaries[0])
            boundaries[0] = index;
        if (index + plus > boundaries[1])
            boundaries[1] = index + plus;
    }

    public Color GetPixel(Point location) => GetPixel(location.X, location.Y);

    public Color GetPixel(int x, int y)
    {
        if (!locked) throw new Exception("Bitmap not locked.");

        int index = (y * Width + x) * (IsAlphaBitmap ? 4 : 3);

        return Color.FromArgb(
            alpha: IsAlphaBitmap ? rgbValues[index + 3] : 255,
            red: rgbValues[index + 2],
            green: rgbValues[index + 1],
            blue: rgbValues[index]
            );
    }

    public static bool Equal2Color(Color clr1, Color clr2, int delta = 0)
            => clr1.A == clr2.A && Math.Abs(clr1.R - clr2.R) <= delta && Math.Abs(clr1.G - clr2.G) <= delta && Math.Abs(clr1.B - clr2.B) <= delta;

    public static bool Equal2AlphaColor(Color clr1, Color clr2, int delta = 0)
            => Math.Abs(clr1.A - clr2.A) <= delta && Math.Abs(clr1.R - clr2.R) <= delta && Math.Abs(clr1.G - clr2.G) <= delta && Math.Abs(clr1.B - clr2.B) <= delta;
}

// from https://www.codeproject.com/Articles/15192/FastPixel-A-much-faster-alternative-to-Bitmap-SetP
// Then I converted FastPixel to SharpPixel for more speed
