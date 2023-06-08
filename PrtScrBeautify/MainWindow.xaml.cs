using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace PrtScrBeautify;

public partial class MainWindow : INotifyPropertyChanged
{
    //private MemoryStream _ms;
    private BitmapImage _mImage;
    private ScreenshotWatcher? _screenshotWatcher;
    public List<IModification> modList;

    public MainWindow()
    {
        InitializeComponent();
        var bbl = new BlurBehindMod();
        var ds = new DropShadowMod();
        var sb = new SolidBorderMod();
        var rc = new RoundedCornersMod(50);
        var rc2 = new RoundedCornersMod(10);
        modList = new List<IModification>() { rc, ds, rc2, bbl, rc };

        TargetFolderPath = @"C:\Users\magfj\Pictures\Screenshots";
        DataContext = this;
        StartScreenshotWatcher();
    }

    public static bool AddRounded { get; set; }
    public static bool AddDropShadow { get; set; }
    public static bool AddBlurBehind { get; set; }
    public static int CornerRadius { get; set; }
    public static bool AddSolidBorder { get; set; }
    public string TargetFolderPath { get; set; }

    public BitmapImage MImage
    {
        get => _mImage;
        set
        {
            if (_mImage != value)
            {
                _mImage = value;
                OnPropertyChanged(nameof(MImage));
            }
        }
    }

    // Implement the INotifyPropertyChanged interface
    public event PropertyChangedEventHandler PropertyChanged; //INotifyCollectionChanged  https://learn.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.observablecollection-1?view=net-7.0

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ModificationsToApply(object sender, RoutedEventArgs routedEventArgs)
    {
        Beautify.SetRoundedCorners(AddRounded);
        Beautify.SetDropShadow(AddDropShadow);
        Beautify.SetBlurBehind(AddBlurBehind);
        Beautify.SetCornerRadius(CornerRadius);
        Beautify.SetSolidBorder(AddSolidBorder);
        StartScreenshotWatcher();
    }

    private void StartScreenshotWatcher()
    {
        UpdateImage();
        if (_screenshotWatcher != null) _screenshotWatcher.Stop();
        _screenshotWatcher = new ScreenshotWatcher(TargetFolderPath);
        _screenshotWatcher.Start();
    }

    private void UpdateImage()
    {
        //UpdateLayout();
        var imgLocation = "C:\\Users\\magfj\\Source\\Repos\\magfje\\PrtScrBeautify\\PrtScrBeautify\\img.jpg";
        var imgSource = Image.Load<Rgba32>(imgLocation);
        var img = imgSource.Clone();
        var b = new Beautify();
        //var modImg = b.ApplyModifications(img);
        var modImg = ImageMods(img);
        var ms = new MemoryStream();
        modImg.Save(ms, new PngEncoder());

        var myBitmapImage = new BitmapImage();
        myBitmapImage.BeginInit();
        myBitmapImage.StreamSource = ms;
        myBitmapImage.EndInit();
        MImage = myBitmapImage;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        //StartScreenshotWatcher();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
    }

    private Image<Rgba32> ImageMods(Image<Rgba32> image)
    {
        foreach (var mod in modList)
        {
            image = mod.Apply(image);
        }

        return image;
    }
}