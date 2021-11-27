using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using MessagingToolkit.QRCode;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using AForge.Video;
using AForge.Video.DirectShow;
using Freezer.Core;

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

            var screenshotJob = ScreenshotJobBuilder.Create(a)
                           .SetBrowserSize(10, 10)
                           .SetCaptureZone(CaptureZone.FullPage)
                           .SetTrigger(new WindowLoadTrigger());

            System.Drawing.Image screenshot = screenshotJob.Freeze();
            screenshot.Save("pic.jpg");

            using (Bitmap bmp = new Bitmap(screenshot))
            {
                MessageBox.Show(PixelCheck(bmp));
            }

        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            QRCodeDecoder Decoder = new QRCodeDecoder();

            try
            {
 
                CapImage.Save("pic.png");
                string html = Decoder.decode(new QRCodeBitmapImage(CapImage));
                NewFormStarter(html);

            }
            catch (Exception)
            {

            }

        }
        // создаём "раскодирование изображения"
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                videoSource = new VideoCaptureDevice(videoDevicesList[cmbVideoSource.SelectedIndex].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                videoSource.Start();
            }
            catch (Exception y)
            {
                MessageBox.Show(y.Message);
            }
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
            // signal to stop
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
            }
            Application.Exit();
        }
        static string PixelCheck(Bitmap bitmap)
        {
            Color now_color = bitmap.GetPixel(10, 100);
            switch (now_color.Name)
            {
                case "ff2dc36a":
                    return "Действительный";
                case "ffe4170b":
                    return "Недействительный";
                default:
                    return "Ошибка";
            }
        }
    }
}
          
















          