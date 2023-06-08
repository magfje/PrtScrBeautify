using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace PrtScrBeautify;

public class DropShadowMod : IModification
{
    public Image<Rgba32> Apply(Image<Rgba32> image)
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
            ctx.DrawImage(dropShadow, new Point(0, 3), 1f);
            ctx.GaussianBlur(4);
            ctx.DrawImage(image, new Point(0, 0),
                1f);
        });
        return fullImage;
    }
}