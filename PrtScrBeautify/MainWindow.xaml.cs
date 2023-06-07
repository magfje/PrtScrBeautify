using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace PrtScrBeautify
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static bool AddRounded { get; set; }
        public static bool AddDropShadow { get; set; }
        public static bool AddBlurBehind { get; set; }
        public static int CornerRadius { get; set; }
        public string TargetFolderPath { get; set; }
        public Image<Rgba32> TesterImage { get; set; }
        
        private ScreenshotWatcher? _screenshotWatcher;
        private MemoryStream _ms;
        private BitmapImage mImage;
        public BitmapImage MImage
        {
            get { return mImage; }
            set
            {
                if (mImage != value)
                {
                    mImage = value;
                    OnPropertyChanged(nameof(MImage));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            AddRounded = true;
            AddDropShadow = true;
            AddBlurBehind = true;
            CornerRadius = 10;
            TargetFolderPath = @"C:\Users\magfj\Pictures\Screenshots";
            DataContext = this;
            StartScreenshotWatcher();
        }

        // Implement the INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;

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
            var imgLocation = @"C:\Users\magfj\source\repos\magfje\PrtScrBeautify\PrtScrBeautify\img.jpg";
            var imgSource = Image.Load<Rgba32>(imgLocation);
            var img = imgSource.Clone();
            var b = new Beautify();
            var modImg = b.ApplyModifications(img);
            _ms = new MemoryStream();
            modImg.Save(_ms, new PngEncoder());

            var myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.StreamSource = _ms;
            myBitmapImage.EndInit();
            MImage = myBitmapImage;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //StartScreenshotWatcher();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {

        }
    }
}
