using System;
using System.Windows;


namespace PrtScrBeautify;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var targetFolderPath = @"C:\Users\magfj\Pictures\Screenshots";
        var screenshotWatcher = new ScreenshotWatcher(targetFolderPath);
        screenshotWatcher.Start();

        var app = new Application();
        var window = new Window();
        window.MaxHeight = 100;
        window.MaxWidth = 100;
        app.Run(window);
    }
}