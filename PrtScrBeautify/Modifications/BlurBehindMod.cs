using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace PrtScrBeautify;

public class BlurBehindMod : IModification
{
    private readonly int _blurAumount;
    private readonly int _offset;

    public BlurBehindMod(int offset, int blurAmount)
    {
        _offset = offset;
        _blurAumount = blurAmount;
    }

    public Image<Rgba32> Apply(Image<Rgba32> image)
    {
        // copy and modify this to create solid border / blurred border.
        // TODO: add shadowColor and offset to object properties!

        var borderOffset = new Size(image.Width + _offset, image.Height + _offset);
        var border = image.Clone();
        border.Mutate(ctx =>
        {
            ctx.Resize(borderOffset);
            ctx.GaussianBlur(_blurAumount);
            ctx.DrawImage(image, new Point(_offset / 2, _offset / 2), 1f);
        });


        return border;
    }
}