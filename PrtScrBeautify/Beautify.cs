using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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
        if (_addBlurBehind) return ApplyBlurredBehind(modifiedImage);
        if (_addSolidBorder) return ApplySolidBorder(modifiedImage);
        if (_addDropShadow) return ApplyDropShadow(modifiedImage);
        if (_addRoundedCorners) return ApplyRoundedCorners(modifiedImage);
        return modifiedImage;
    }

    private static Image<Rgba32> ApplyRoundedCorners(Image<Rgba32> image)
    {
        var size = image.Size;
        var corners = BuildCorners(size.Width, size.Height);

        image.Mutate(ctx =>
        {
            ctx.SetGraphicsOptions(new GraphicsOptions
            {
                Antialias = true,
                AlphaCompositionMode = PixelAlphaCompositionMode.DestOut
            });
            foreach (var path in corners) ctx.Fill(Color.Red, path);
        });
        return image;
    }

    private static IPathCollection BuildCorners(int imageWidth, int imageHeight)
    {
        var rect = new RectangularPolygon(-0.5f, -0.5f, _cornerRadius, _cornerRadius);
        var cornerTopLeft = rect.Clip(new EllipsePolygon(_cornerRadius - 0.5f, _cornerRadius - 0.5f, _cornerRadius));
        var rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1;
        var bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1;
        var cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
        var cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
        var cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);

        return new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
    }

    private static Image<Rgba32> ApplyDropShadow(Image<Rgba32> image)
    {
        // copy and modify this to create solid border / blurred border.
        // TODO: add shadowColor and offset to object properties!
        //
        var shadowColor = new Rgba32(0, 0, 0, 100); // Shadow color (black with transparency)
        var shadowOffset = new Size(10, 10); // Shadow offset (adjust as needed)

        var dropShadow = new Image<Rgba32>(image.Width, image.Height);

        var fullImage = new Image<Rgba32>(image.Width + shadowOffset.Width, image.Height + shadowOffset.Height);

        dropShadow.Mutate(ctx => { ctx.Fill(shadowColor); });
        fullImage.Mutate(ctx =>
        {
            ctx.DrawImage(_addRoundedCorners ? ApplyRoundedCorners(dropShadow) : dropShadow, new Point(0, 3), 1f);
            ctx.GaussianBlur(4);
            ctx.DrawImage(_addRoundedCorners ? ApplyRoundedCorners(image) : image, new Point(0, 0),
                1f);
        });
        return _addRoundedCorners ? ApplyRoundedCorners(fullImage) : fullImage;
    }

    private static Image<Rgba32> ApplyBlurredBehind(Image<Rgba32> image)
    {
        // copy and modify this to create solid border / blurred border.
        // TODO: add shadowColor and offset to object properties!

        var offset = 100;
        var borderOffset = new Size(image.Width + offset, image.Height + offset);
        var border = image.Clone();
        border.Mutate(ctx =>
        {
            ctx.Resize(borderOffset);
            ctx.GaussianBlur(5);

            if (_addDropShadow) image = ApplyDropShadow(image);
            if (_addRoundedCorners) image = ApplyRoundedCorners(image);
            ctx.DrawImage(image, new Point(offset / 2, offset / 2), 1f);
        });

        return _addRoundedCorners ? ApplyRoundedCorners(border) : border;
    }

    private static Image<Rgba32> ApplySolidBorder(Image<Rgba32> image)
    {
        // copy and modify this to create solid border / blurred border.
        // TODO: add shadowColor and offset to object properties!

        var offset = 100;
        var borderOffset = new Size(image.Width + offset, image.Height + offset);
        var border = new Image<Rgba32>(borderOffset.Width, borderOffset.Height);
        border.Mutate(ctx =>
        {
            //ctx.Resize(borderOffset);
            //ctx.GaussianBlur(5);
            ctx.Fill(Color.Crimson);

            if (_addDropShadow) image = ApplyDropShadow(image);
            if (_addRoundedCorners) image = ApplyRoundedCorners(image);
            ctx.DrawImage(image, new Point(offset / 2, offset / 2), 1f);
        });

        return _addRoundedCorners ? ApplyRoundedCorners(border) : border;
    }
}