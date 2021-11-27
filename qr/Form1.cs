using System;
using System.Drawing;
using System.Windows.Forms;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;

namespace qr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
             // создаём "раскодирование изображения"
        private void button1_Click(object sender, EventArgs e)
        {
             QRCodeBitmapImage a;
             a = new QRCodeBitmapImage(pictureBox1.Image as Bitmap);
             QRCodeDecoder decoder = new QRCodeDecoder();
             string html = decoder.decode(a);
            string html2 = string.Empty;
            for (int i = 0; i < html.Length-2; i++)
             {
                 html2 += html[i];
             }
            html2 += "en";
            Form2 form2 = new Form2(html);
            html = string.Empty;
            form2.Show();
            form2 = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog load = new OpenFileDialog(); //  load будет запрашивать у пользователя место, из которого он хочет загрузить файл.
            if (load.ShowDialog() == System.Windows.Forms.DialogResult.OK) //если пользователь нажимает в обозревателе кнопку "Открыть".
            {
                pictureBox1.ImageLocation = load.FileName; // в pictureBox'e открывается файл, который был по пути, запрошенном пользователем.
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }
    }
}
          
















          