using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace PrtScrBeautify;

public class RoundedCornersMod : IModification
{
    private readonly int _cornerRadius;

    public RoundedCornersMod(int cornerRadius)
    {
        _cornerRadius = cornerRadius;
    }

    public Image<Rgba32> Apply(Image<Rgba32> image)
    {
        var corners = BuildCorners(image.Width, image.Height);

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

    private IPathCollection BuildCorners(int imageWidth, int imageHeight)
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
}