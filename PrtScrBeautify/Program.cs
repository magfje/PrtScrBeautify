using ImageMagick;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;


namespace PrtScrBeautify;

public class Program
{
    private static FileSystemWatcher watcher;

    private static string targetFolderPath =
        @"C:\Users\Magnus\AppData\Local\Packages\MicrosoftWindows.Client.CBS_cw5n1h2txyewy\TempState\ScreenClip";

    [STAThread]
    public static void Main(string[] args)
    {
        //InitializeComponent();

        // Initialize the FileSystemWatcher
        watcher = new FileSystemWatcher();
        watcher.Path = targetFolderPath;
        watcher.Filter = "*.png";
        watcher.Created += OnFileCreated;

        // Start watching
        watcher.EnableRaisingEvents = true;

        var app = new Application();
        var window = new Window();
        app.Run(window);
    }

    private static void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        string filePath = e.FullPath;

        // Perform actions on the first newly created file
        if (!string.IsNullOrEmpty(filePath))
        {
            // Perform actions on the newly created file (e.g., add to clipboard)
            try
            {
                Trace.WriteLine("New file detected! - " + filePath);
                using var image2 = new MagickImage(filePath);
                using var image = new MagickImage(MagickColors.Transparent, image2.Width, image2.Height);
                using var mask = new MagickImage(MagickColors.White, image.Width, image.Height);

                ApplyModifications(image, mask, image2);
                //TestMod(image, mask);


                //image.Write("rounded-corners.png");
                var bitmapSource = ToBitmapSource(image);

                SetTheClipboard(bitmapSource);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            // Disable further file watching after a timeout of 2 seconds
            watcher.EnableRaisingEvents = false;
            Task.Delay(2000).ContinueWith(_ => { watcher.EnableRaisingEvents = true; });
        }
    }


    private static void SetTheClipboard(BitmapSource bitmapSource)
    {
        // Create a DataObject and set the BitmapSource as the image data
        var dataObject = new DataObject();
        dataObject.SetImage(bitmapSource);

        //Clipboard.SetDataObject(dataObject, true);

        Thread thread = new Thread(() => Clipboard.SetDataObject(dataObject, true));
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        Trace.WriteLine("Image copied to clipboard");
    }

    private static BitmapSource ToBitmapSource(MagickImage image)
    {
        using (var memoryStream = new MemoryStream())
        {
            image.Write(memoryStream, MagickFormat.Png);
            memoryStream.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }
    }


    private static void ApplyModifications(MagickImage image, MagickImage mask, MagickImage image2)
    {
        var size = 25;

        new ImageMagick.Drawables()
            .FillColor(MagickColors.Black)
            .StrokeColor(MagickColors.Black)
            .Polygon(
                new PointD(0, 0),
                new PointD(0, size),
                new PointD(size, 0))
            .Polygon(
                new PointD(mask.Width, 0),
                new PointD(mask.Width, size),
                new PointD(mask.Width - size, 0))
            .Polygon(
                new PointD(0, mask.Height),
                new PointD(0, mask.Height - size),
                new PointD(size, mask.Height))
            .Polygon(
                new PointD(mask.Width, mask.Height),
                new PointD(mask.Width, mask.Height - size),
                new PointD(mask.Width - size, mask.Height))
            .FillColor(MagickColors.White)
            .StrokeColor(MagickColors.White)
            .Circle(size, size, size, 0)
            .Circle(mask.Width - size, size, mask.Width - size, 0)
            .Circle(size, mask.Height - size, 0, mask.Height - size)
            .Circle(mask.Width - size, mask.Height - size, mask.Width - size, mask.Height)
            //.Draw(image)
            .Draw(mask);


        // This copies the pixels that were already transparent on the mask.
        //using (var imageAlpha = image.Clone())
        //{
        //    imageAlpha.Alpha(AlphaOption.Extract);
        //    imageAlpha.Opaque(MagickColors.White, MagickColors.None);
        //    mask.Composite(imageAlpha, CompositeOperator.Over);
        //}

        //MagickGeometry gm = new MagickGeometry();
        //gm.Width = image.Width;
        //gm.Height = image.Height;
        //gm.IgnoreAspectRatio = true;
        //mask.Density = image.Density;
        //mask.SetBitDepth(image.DetermineBitDepth());

        //mask.LiquidRescale(gm);

        //mask.Transparent(MagickColors.White);

        //mask.HasAlpha = true;
        //image.HasAlpha = false;
        image.Composite(image2, CompositeOperator.Atop);
        image.Composite(mask, CompositeOperator.CopyAlpha);
        //image.Alpha(AlphaOption.Extract);
        //image.Transparent(MagickColors.Black);
        image.Write(@"C:\Users\Magnus\Pictures\test.Png");
    }
}