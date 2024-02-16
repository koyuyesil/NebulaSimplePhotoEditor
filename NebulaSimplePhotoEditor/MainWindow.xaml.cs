
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace NebulaSimplePhotoEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapImage bitmapImage;
        private double imageWidth;
        private double imageHeight;
        private double aspectRatio;
        private DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            PrepareImage("pack://application:,,,/Resources/no_image.png");

        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            ShowBrowseImageDialog();
        }

        private void LoadImage(string imagePath)
        {
            try
            {
                PrepareImage(imagePath);

                MoveToCenterScreen();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrepareImage(string imagePath)
        {
            bitmapImage = new BitmapImage(new Uri(imagePath));
            imageWidth = bitmapImage.PixelWidth;
            imageHeight = bitmapImage.PixelHeight;
            imageControl.Source = bitmapImage;
            aspectRatio = imageWidth / imageHeight;
        }

        private void MoveToCenterScreen()
        {
            // Maksimum genişlik ve yükseklik sınırlarını belirleyin (ekranın 3'te biri).
            double maxScreenWidth = SystemParameters.PrimaryScreenWidth / 2;
            double maxScreenHeight = SystemParameters.PrimaryScreenHeight / 2;
            if (aspectRatio > 1) // Geniş resim
            {
                this.Width = Math.Min(imageWidth, maxScreenWidth);
                this.Height = this.Width / aspectRatio;
            }
            else // Yüksek resim
            {
                this.Height = Math.Min(imageHeight, maxScreenHeight);
                this.Width = this.Height * aspectRatio;
            }
            // Formu ekranın ortasına tekrar yerleştir
            this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
            this.Top = (SystemParameters.PrimaryScreenHeight - this.Height) / 2;
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Fare orta tıklama
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void imageControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            StartTimer();
            // Tekerleğin yukarı veya aşağı hareketine bağlı olarak boyutlandırmayı gerçekleştirin
            double scale = (e.Delta > 0) ? 1.1 : 0.9;

            // Yeni genişlik ve yüksekliği hesaplayın
            imageWidth *= scale;
            imageHeight *= scale;

            // BitmapImage'i yeni boyutlarına göre ayarlayın
            bitmapImage.DecodePixelWidth = (int)imageWidth;
            bitmapImage.DecodePixelHeight = (int)imageHeight;

            // Image kontrolünü güncel boyutlarına göre ayarlayın
            this.Width = imageWidth;
            this.Height = imageHeight;

            // İşlemlerinizi burada gerçekleştirin (isteğe bağlı)
            Console.WriteLine($"Mouse Wheel Moved: {e.Delta}, New Size: {imageWidth} x {imageHeight}");
        }

        private void StartTimer()
        {
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(2);
                timer.Tick += TimerTick;
            }
            if (timer.IsEnabled)
            {
                timer.Stop();
                timer.Start();
            }        
            // Timer'ı başlat
            timer.Start();
        }

        private void TimerTick(object? sender, EventArgs e)
        {
            MoveToCenterScreen();
            StopTimer();
        }

        private void StopTimer()
        {
            if (timer!=null&&timer.IsEnabled)
            {
                timer.Stop();
            }
            // Timer çalışıyorsa durdur

        }

        private void imageControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ShowBrowseImageDialog();
            }
            
        }

        private void ShowBrowseImageDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif)|*.png;*.jpg;*.jpeg;*.gif|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                LoadImage(openFileDialog.FileName);
            }
        }
    }
}