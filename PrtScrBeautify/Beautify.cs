using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Windows;
using Point = SixLabors.ImageSharp.Point;
using Size = SixLabors.ImageSharp.Size;

namespace PrtScrBeautify;

public class Beautify
{
    private static bool _addRoundedCorners;
    private static bool _addDropShadow;
    private static int _cornerRadius;
    private static bool _addBlurBehind;
    private static bool _addSolidBorder;

    public static void SetRoundedCorners(bool value)
    {
        _addRoundedCorners = value;
    }

    public static void SetDropShadow(bool value)
    {
        _addDropShadow = value;
    }

    public static void SetCornerRadius(int value)
    {
        _cornerRadius = value;
    }

    public static void SetBlurBehind(bool value)
    {
        _addBlurBehind = value;
    }

    public static void SetSolidBorder(bool value)
    {
        _addSolidBorder = value;
    }

    public Image<Rgba32> ApplyModifications(Image<Rgba32> image)
    {
        var modifiedImage = image.Clone();
        //if (_addBlurBehind) return RoundedCorners_.ApplyBlurredBehind(modifiedImage);
        //if (_addSolidBorder) return RoundedCorners_.ApplySolidBorder(modifiedImage);
        //if (_addDropShadow) return RoundedCorners_.ApplyDropShadow(modifiedImage);
        //if (_addRoundedCorners) return RoundedCorners_.ApplyRoundedCorners(modifiedImage);
        return modifiedImage;
    }

}