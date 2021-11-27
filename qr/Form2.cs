using System;
using System.Drawing;
using System.Windows.Forms;
using Freezer.Core;


namespace qr
{
    public partial class Form2 : Form
    {
        public Form2(string adr)
        {
            InitializeComponent();
            var screenshotJob = ScreenshotJobBuilder.Create(adr)
                          .SetBrowserSize(10, 10)
                          .SetCaptureZone(CaptureZone.FullPage)
                          .SetTrigger(new WindowLoadTrigger());

            System.Drawing.Image screenshot = screenshotJob.Freeze();
            screenshot.Save("pic.jpg");
            //pictureBox1.Image = screenshot;

            Bitmap bmp = new Bitmap(screenshot);
            textBox1.Text = PixelCheck(bmp);
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
