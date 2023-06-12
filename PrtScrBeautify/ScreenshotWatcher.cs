using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PrtScrBeautify;

public class ScreenshotWatcher
{
    private readonly string _targetFolderPath;
    private string? _filePath;
    private readonly List<IModification> _modList;
    private FileSystemWatcher? _watcher;

    public ScreenshotWatcher(string targetFolderPath, List<IModification> modList)
    {
        _targetFolderPath = targetFolderPath;
        _modList = modList;
    }

    public void Start()
    {
        _watcher = new FileSystemWatcher();
        _watcher.Path = _targetFolderPath;
        _watcher.Filter = "*.png";
        _watcher.Created += OnFileCreated;
        _watcher.EnableRaisingEvents = true;
    }

    public void Stop()
    {
        _watcher?.Dispose(); //does this do anything for performance?
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        _filePath = e.FullPath;
        if (string.IsNullOrEmpty(_filePath)) return;
        try
        {
            Trace.WriteLine("New file detected! - " + _filePath);
            var img = Image.Load<Rgba32>(_filePath);
            var beautify = new Beautify(_modList);
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
            Trace.WriteLine(ex.StackTrace);
        }

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