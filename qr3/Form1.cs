using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using AForge.Video;
using AForge.Video.DirectShow;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;

namespace qr
{
    public partial class Form1 : Form
    {
        BackgroundWorker worker;
        private FilterInfoCollection videoDevicesList;
        private IVideoSource videoSource;
        Bitmap CapImage;
        System.Timers.Timer t = new System.Timers.Timer();
        bool isTimerStarted;
        IWebDriver driver;
        public Form1()
        {
            InitializeComponent();
            isTimerStarted = false;
            videoDevicesList = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo videoDevice in videoDevicesList)
            {
                cmbVideoSource.Items.Add(videoDevice.Name);
            }
            if (cmbVideoSource.Items.Count > 0)
            {
                cmbVideoSource.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No video sources found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            worker = new BackgroundWorker();

            worker.DoWork += Worker_DoWork;
            this.Closing += Form1_Closing;
            t = new System.Timers.Timer();
            t.Elapsed += T_Tick;
            t.Interval = 1;
        }
        private void T_Tick(object sender, EventArgs e)
        {
            if (CapImage != null && !worker.IsBusy)
                worker.RunWorkerAsync();
        }
        private void NewFormStarter(string a)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");
            driver = new ChromeDriver(options);

            System.Drawing.Image screenshot;
            try
            {
                driver.Manage().Window.Size = new Size(586, 550);
                driver.Url = a;
                ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile("1.png");
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка, попробуйте еще раз", "Результат");
            }
            driver.Quit();
            using (FileStream stream = new FileStream("1.png", FileMode.Open))
            {
                screenshot = System.Drawing.Image.FromStream(stream);
            }
            Bitmap bmp = new Bitmap(screenshot);
            MessageBox.Show(PixelCheck(bmp), "Результат");
            if (File.Exists("1.png"))
            {
                File.Delete("1.png");
            }
        }
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            QRCodeDecoder Decoder = new QRCodeDecoder();

            try
            {
                string html = Decoder.decode(new QRCodeBitmapImage(CapImage));
                NewFormStarter(html);
            }
            catch (Exception)
            {

            }

        }
        private void button1_Click(object sender, EventArgs e)
        {

                videoSource = new VideoCaptureDevice(videoDevicesList[cmbVideoSource.SelectedIndex].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                videoSource.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            videoSource.SignalToStop();
            if (videoSource != null && videoSource.IsRunning && pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }
            t.Stop();
        }
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = (Image)bitmap.Clone();
            CapImage = bitmap;

            if (!isTimerStarted)
            {
                t.Start();
                isTimerStarted = true;
            }
        }
        private void Form1_Closing(object sender, CancelEventArgs e)
        {

            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
            }
            Application.Exit();
        }
        static string PixelCheck(Bitmap bitmap)
        {
            Color now_color = bitmap.GetPixel(247, 137);
            switch (now_color.Name)
            {
                case "ff2dc36a":
                    return "Действительный";
                case "ffe4170b":
                    return "Недействительный";
                default:
                    return "Ошибка, попробуйте еще раз";
            }
        }
    }
}
          
















          