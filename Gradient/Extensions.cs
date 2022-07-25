using System;
using System.Numerics;
using System.Windows;
using System.Windows.Media;

namespace Gradient;

public static class Extensions
{
    public static Vector2 ToVector2(this Point point)
    {
        return new Vector2((float)point.X, (float)point.Y);
    }

    public static System.Drawing.Color Convert(this Color color)
    {
        return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
    }
    
    public static Color Convert(this System.Drawing.Color color)
    {
        return Color.FromArgb(color.A, color.R, color.G, color.B);
    }
    
    /// <summary>
    /// Convert from System.Windows.Media.PixelFormat to System.Drawing.Imaging.PixelFormat
    /// </summary>
    /// <param name="pixelFormat"></param>
    /// <exception cref="T:System.NotSupportedException">Convertion is not available</exception>
    /// <returns></returns>
    public static System.Drawing.Imaging.PixelFormat Convert(this PixelFormat pixelFormat)
    {
        if (pixelFormat == PixelFormats.Gray16)
            return System.Drawing.Imaging.PixelFormat.Format16bppGrayScale;
        if (pixelFormat == PixelFormats.Bgr555)
            return System.Drawing.Imaging.PixelFormat.Format16bppRgb555;
        if (pixelFormat == PixelFormats.Bgr565)
            return System.Drawing.Imaging.PixelFormat.Format16bppRgb565;
        if (pixelFormat == PixelFormats.Bgr101010)
            return System.Drawing.Imaging.PixelFormat.Indexed;
        if (pixelFormat == PixelFormats.Indexed1)
            return System.Drawing.Imaging.PixelFormat.Format1bppIndexed;
        if (pixelFormat == PixelFormats.Indexed4)
            return System.Drawing.Imaging.PixelFormat.Format4bppIndexed;
        if (pixelFormat == PixelFormats.Indexed8)
            return System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
        if (pixelFormat == PixelFormats.Bgr24)
            return System.Drawing.Imaging.PixelFormat.Format24bppRgb;
        if (pixelFormat == PixelFormats.Bgr32)
            return System.Drawing.Imaging.PixelFormat.Format32bppArgb;
        if (pixelFormat == PixelFormats.Pbgra32)
            return System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
        if (pixelFormat == PixelFormats.Bgr32)
            return System.Drawing.Imaging.PixelFormat.Format32bppRgb;
        if (pixelFormat == PixelFormats.Rgb48)
            return System.Drawing.Imaging.PixelFormat.Format48bppRgb;
        if (pixelFormat == PixelFormats.Prgba64)
            return System.Drawing.Imaging.PixelFormat.Format64bppArgb;
        if (pixelFormat == PixelFormats.Default)
            return System.Drawing.Imaging.PixelFormat.Undefined;
        throw new NotSupportedException("Conversion not supported with " + pixelFormat);
    }
    
    /// <summary>
    /// Convert from System.Drawing.Imaging.PixelFormat to System.Windows.Media.PixelFormat
    /// </summary>
    /// <param name="pixelFormat"></param>
    /// <exception cref="T:System.NotSupportedException">Convertion is not available</exception>
    /// <returns></returns>
    public static PixelFormat Convert(this System.Drawing.Imaging.PixelFormat pixelFormat)
    {
        return pixelFormat switch
        {
            System.Drawing.Imaging.PixelFormat.Undefined => PixelFormats.Default,
            System.Drawing.Imaging.PixelFormat.Indexed => PixelFormats.Bgr101010,
            System.Drawing.Imaging.PixelFormat.Format16bppRgb555 => PixelFormats.Bgr555,
            System.Drawing.Imaging.PixelFormat.Format16bppRgb565 => PixelFormats.Bgr565,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb => PixelFormats.Bgr24,
            System.Drawing.Imaging.PixelFormat.Format32bppRgb => PixelFormats.Bgr32,
            System.Drawing.Imaging.PixelFormat.Format1bppIndexed => PixelFormats.Indexed1,
            System.Drawing.Imaging.PixelFormat.Format4bppIndexed => PixelFormats.Indexed4,
            System.Drawing.Imaging.PixelFormat.Format8bppIndexed => PixelFormats.Indexed8,
            System.Drawing.Imaging.PixelFormat.Format32bppPArgb => PixelFormats.Pbgra32,
            System.Drawing.Imaging.PixelFormat.Format16bppGrayScale => PixelFormats.Gray16,
            System.Drawing.Imaging.PixelFormat.Format48bppRgb => PixelFormats.Rgb48,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb => PixelFormats.Bgr32,
            System.Drawing.Imaging.PixelFormat.Format64bppArgb => PixelFormats.Prgba64,
            _ => throw new NotSupportedException("Conversion not supported with " + pixelFormat)
        };
    }
}