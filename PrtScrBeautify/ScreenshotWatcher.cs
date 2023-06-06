using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Point = SixLabors.ImageSharp.Point;
using ResizeMode = SixLabors.ImageSharp.Processing.ResizeMode;
using Size = SixLabors.ImageSharp.Size;

namespace PrtScrBeautify;

public class ScreenshotWatcher
{
    private readonly string targetFolderPath;
    private string filePath;
    private FileSystemWatcher watcher;

    public ScreenshotWatcher(string targetFolderPath)
    {
        this.targetFolderPath = targetFolderPath;
        AddRoundedCorners = true;
        AddDropShadow = true;
        CornerRadius = 10;
        ApplyBlurBehind = true;
    }

    public static bool AddRoundedCorners { get; set; }
    public static bool AddDropShadow { get; set; }
    public int CornerRadius { get; set; }
    public static bool ApplyBlurBehind { get; set; }


    public void Start()
    {
        // Initialize the FileSystemWatcher
        watcher = new FileSystemWatcher();
        watcher.Path = targetFolderPath;
        watcher.Filter = "*.png";
        watcher.Created += OnFileCreated;

        // Start watching
        watcher.EnableRaisingEvents = true;
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        filePath = e.FullPath;
        // Perform actions on the first newly created file
        if (string.IsNullOrEmpty(filePath)) return;
        // Perform actions on the newly created file (e.g., add to clipboard)
        try
        {
            Trace.WriteLine("New file detected! - " + filePath);
            var img = Image.Load<Rgba32>(filePath);

            var destRound = ApplyModifications(img, new Size(img.Width, img.Height), CornerRadius,
                AddRoundedCorners,
                AddDropShadow);

            destRound.Save($"C:\\Users\\magfj\\Pictures\\{e.Name}");
            FileToClipboard($"C:\\Users\\magfj\\Pictures\\{e.Name}");
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex.Message, ex.StackTrace);
        }

        // Disable further file watching after a timeout of 2 seconds
        watcher.EnableRaisingEvents = false;
        Task.Delay(2000).ContinueWith(_ => { watcher.EnableRaisingEvents = true; });
    }

    private static void FileToClipboard(string p)
    {
        var paths = new StringCollection();
        paths.Add(p);
        var thread = new Thread(() => Clipboard.SetFileDropList(paths));
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        Trace.WriteLine("Image copied to clipboard");
    }

    private static Image<Rgba32> ApplyModifications(Image<Rgba32> image, Size size, int cornerRadius,
        bool addRoundedCorners, bool addDropShadow)
    {
        var modifiedImage = image.Clone(ctx =>
        {
            ctx.Resize(new ResizeOptions
            {
                Size = size,
                Mode = ResizeMode.Crop
            });
        });
        if (ApplyBlurBehind) return ApplyBlurredBehind(modifiedImage, cornerRadius);
        if (AddDropShadow && AddRoundedCorners) return ApplyDropShadow(modifiedImage, cornerRadius);
        if (AddDropShadow) return ApplyDropShadow(modifiedImage, cornerRadius);
        if (AddRoundedCorners) return ApplyRoundedCorners(modifiedImage, cornerRadius);


        //if (addDropShadow) using modifiedImage = ApplyDropShadow(modifiedImage, cornerRadius);
        //if (addRoundedCorners) ApplyRoundedCorners(modifiedImage, cornerRadius);
        //might have to do something with the return value and modiffiedimagevariable... dunno

        return modifiedImage;
    }


    private static Image<Rgba32> ApplyRoundedCorners(Image<Rgba32> image, float cornerRadius)
    {
        var size = image.Size;
        var corners = BuildCorners(size.Width, size.Height, cornerRadius);

        image.Mutate(ctx =>
        {
            ctx.SetGraphicsOptions(new GraphicsOptions
            {
                Antialias = true,
                AlphaCompositionMode = PixelAlphaCompositionMode.DestOut
            });

            foreach (var path in corners) ctx.Fill(Color.Red, path);
            //ctx.Glow(Color.AliceBlue);
        });
        //image.Save("C:\\Users\\magfj\\Pictures\\hgahgdh.png");
        return image;
    }

    private static IPathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
    {
        var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);
        var cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));
        var rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1;
        var bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1;
        var cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
        var cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
        var cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);

        return new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
    }

    private static Image<Rgba32> ApplyDropShadow(Image<Rgba32> image, int cornerRadius)
    {
        // copy and modify this to create solid border / blurred border.
        // TODO: add shadowColor and offset to object properties!
        //
        var shadowColor = new Rgba32(0, 0, 0, 100); // Shadow color (black with transparency)
        var shadowColorP = new Rgba32(255, 99, 71, 102); // Shadow color (black with transparency)
        var shadowOffset = new Size(10, 40); // Shadow offset (adjust as needed)

        var border = new Image<Rgba32>(image.Width, image.Height);

        var shadow = new Image<Rgba32>(image.Width + shadowOffset.Width + 20, image.Height + shadowOffset.Height + 20);

        border.Mutate(ctx =>
        {
            ctx.Fill(shadowColor);
            //ctx.GaussianBlur(10);
            //ctx.DrawImage(ApplyRoundedCorners(image, cornerRadius), new Point(0, 0), 1f);
        });
        shadow.Mutate(ctx =>
        {
            //ctx.Fill(shadowColor);
            //ctx.GaussianBlur(15);
            //var graph = new GraphicsOptions { Antialias = true };
            //ctx.Glow(graph, Color.DeepPink, 2000f, rect1);
            //ctx.Glow(graph, Color.DeepPink, 2000f, rect2);
            //ctx.Opacity(0.3f);
            ctx.DrawImage(AddRoundedCorners ? ApplyRoundedCorners(border, cornerRadius) : border, new Point(5,
                4), 1f);
            //ctx.DrawImage(ApplyRoundedCorners(border, cornerRadius), new Point(5, 4), 1f);
            ctx.GaussianBlur(4);
            //ctx.Opacity(.3f);
            ctx.DrawImage(AddRoundedCorners ? ApplyRoundedCorners(image, cornerRadius) : image, new Point(5, 0),
                1f);
            //ctx.DrawImage(ApplyRoundedCorners(image, cornerRadius), new Point(5, 0), 1f);
        });
        //shadow.Save("C:\\Users\\magfj\\Pictures\\testShadow.png");
        // Combine the original image and the shadow
        //image.Mutate(ctx => ctx.DrawImage(shadow, new Point(0, 0), 1f));
        if (AddRoundedCorners) return ApplyRoundedCorners(shadow, cornerRadius);
        return shadow;
    }

    private static Image<Rgba32> ApplyBlurredBehind(Image<Rgba32> image, int cornerRadius)
    {
        // copy and modify this to create solid border / blurred border.
        // TODO: add shadowColor and offset to object properties!
        //

        var offset = 100;
        var borderOffset = new Size(image.Width + offset, image.Height + offset);

        //var border = new Image<Rgba32>(image.Width, image.Height);
        var border = image.Clone();
        border.Mutate(ctx =>
        {
            ctx.Resize(borderOffset);
            ctx.GaussianBlur(5);
            ctx.DrawImage(ApplyDropShadow(image, cornerRadius),
                new Point(offset / 2, offset / 2),
                1f);
        });

        return ApplyRoundedCorners(border, cornerRadius);
    }
}