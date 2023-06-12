using System;
using System.Windows;

namespace PrtScrBeautify;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var app = new Application();
        var mainWindow = new MainWindow();
        app.Run(mainWindow);


    }
}