using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;


namespace PrtScrBeautify;

public class ScreenshotWatcher
{
    private readonly string _targetFolderPath;
    private string? _filePath;
    private FileSystemWatcher? _watcher;

    public ScreenshotWatcher(string targetFolderPath) {
        this._targetFolderPath = targetFolderPath;
    }

    public void Start()
    {
        // Initialize the FileSystemWatcher
        _watcher = new FileSystemWatcher();
        _watcher.Path = _targetFolderPath;
        _watcher.Filter = "*.png";
        _watcher.Created += OnFileCreated;

        // Start watching
        _watcher.EnableRaisingEvents = true;
    }

    public void Stop()
    {
        _watcher?.Dispose(); //does this do anything for performance?
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        _filePath = e.FullPath;
        // Perform actions on the first newly created file
        if (string.IsNullOrEmpty(_filePath)) return;
        // Perform actions on the newly created file (e.g., add to clipboard)
        try
        {
            Trace.WriteLine("New file detected! - " + _filePath);
            var img = Image.Load<Rgba32>(_filePath);
            var beautify = new Beautify();
            var modifiedImage = beautify.ApplyModifications(img);


            modifiedImage.Save($"C:\\Users\\magfj\\Pictures\\{e.Name}");
            FileToClipboard($"C:\\Users\\magfj\\Pictures\\{e.Name}");
            //
            // Fix: This works ish, need to fix transparency
            //var ms = new MemoryStream();
            //modifiedImage.Save(ms, new PngEncoder());
            //var myBitmapImage = new BitmapImage();
            //myBitmapImage.BeginInit();
            //myBitmapImage.StreamSource = ms;
            //myBitmapImage.EndInit();
            //myBitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            //ImgToClip(myBitmapImage);
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex.Message, ex.StackTrace);
        }

        // Disable further file watching after a timeout of 2 seconds
        if (_watcher == null) return;
        _watcher.EnableRaisingEvents = false;
        Task.Delay(2000).ContinueWith(_ => { _watcher.EnableRaisingEvents = true; });
    }

    private static void FileToClipboard(string p)
    {
        var paths = new StringCollection { p };
        var thread = new Thread(() => Clipboard.SetFileDropList(paths));
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        thread.Interrupt(); // Does this do anything for performance?
        Trace.WriteLine("Image copied to clipboard");
    }
    //private static void ImgToClip(BitmapImage img)
    //{
    //    var thread = new Thread(() => Clipboard.SetImage(img));
    //    thread.SetApartmentState(ApartmentState.STA);
    //    thread.Start();
    //    thread.Join();
    //    thread.Interrupt(); // Does this do anything for performance?
    //    Trace.WriteLine("Image copied to clipboard");
    //}
}