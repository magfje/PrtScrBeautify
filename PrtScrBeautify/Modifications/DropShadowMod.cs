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
            ctx.DrawImage(dropShadow, new Point(shadowOffset.Width, shadowOffset.Height), 1f);
            ctx.BoxBlur(10);
            ctx.DrawImage(image, new Point(0, 0),
                1f);
        });
        return fullImage;

        //var width = image.Width;
        //var height = image.Height;
        //var shadowOffset = new Size(10, 20);

        //// Create a linear gradient brush for the drop shadow
        //var gradient = new LinearGradientBrush(
        //    new Point(0, height / 2),
        //    new Point(0, height + 20),
        //    GradientRepetitionMode.None,
        //    new ColorStop(0, Color.Black), // Start color (fully transparent)
        //    new ColorStop(1, Color.Transparent)); // End color (fully opaque)

        ////var shadow = new Image<Rgba32>(width, height);
        //var shadow = image.Clone();
        //shadow.Mutate(ctx => ctx
        //    .Fill(Color.Black)
        //    .SetGraphicsOptions(new GraphicsOptions
        //    {
        //        Antialias = true,
        //        AlphaCompositionMode = PixelAlphaCompositionMode.SrcOver
        //    }));

        //var shadowImage = new Image<Rgba32>(width + shadowOffset.Width, height + shadowOffset.Height);

        //shadowImage.Mutate(ctx => ctx
        //    //.Fill(gradient)
        //    .DrawImage(shadow, new Point(shadowOffset.Width / 2, shadowOffset.Height / 2), 1f)
        //    .BoxBlur(10)
        //    .DrawImage(image, new Point(shadowOffset.Width / 2, shadowOffset.Height / 2),
        //        1f)); // Draw the original image at full opacity

        //return shadowImage;
    }
}