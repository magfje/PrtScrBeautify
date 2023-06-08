using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace PrtScrBeautify;

public class BlurBehindMod : IModification
{
    public Image<Rgba32> Apply(Image<Rgba32> image)
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
            ctx.DrawImage(image, new Point(offset / 2, offset / 2), 1f);
        });

        return border;
    }
}