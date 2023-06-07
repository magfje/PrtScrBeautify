using System;
using System.Windows;

namespace PrtScrBeautify;

public static class Program
{
    //public static bool addRounded { get; set; }
    //public static bool addDropShadow { get; set; }
    //public static bool addBlurBehind { get; set; }
    //public static int cornerRadius { get; set; }
    [STAThread]
    public static void Main(string[] args)
    {
        var app = new Application();
        var mainWindow = new MainWindow();
        app.Run(mainWindow);


    }
}