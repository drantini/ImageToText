using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using IronOcr;
using Microsoft.Win32;
using System.IO;
using System.Drawing;

namespace ImageToText
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Bitmap inputBitmap;
        private bool was_converted = false;
        private bool was_uploaded = false;
        private bool denoise = false;
        private string pdf_path;
        private int selectedType = -1;
        public MainWindow()
        {
            InitializeComponent();
            Type.SelectedIndex = 0;
        }
        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            selectedType = Type.SelectedIndex;

            if (selectedType == 0)
            {
                OpenFileDialog fd = new OpenFileDialog();
                if (fd.ShowDialog() == true)
                {
                    inputImage.Source = new BitmapImage(new Uri(fd.FileName));
                    inputBitmap = BitmapImage2Bitmap(new BitmapImage(new Uri(fd.FileName)));
                    was_uploaded = true;
                }
            }
            else if (selectedType == 1)
            {
                OpenFileDialog fd = new OpenFileDialog();
                if (fd.ShowDialog() == true)
                {
                    pdf_path = fd.FileName;
                    was_uploaded = true;
                }
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (was_uploaded)
            {
                using (var input = new OcrInput())
                {
                    if (selectedType == 0)
                    {
                        input.AddImage(inputBitmap);
                    }
                    else
                    {
                        input.AddPdf(pdf_path);
                        pdf_path = null;
                    }
                    
                    input.Deskew();
                    if (DeNoise.IsChecked == true)
                    {
                        input.DeNoise();
                    }

                    var Result = new IronTesseract().Read(input).Text;
                    OutputText.Text = Result;
                    was_uploaded = false;
                    was_converted = true;
                }
            }
            else
            {
                MessageBox.Show("Upload image first.");
            }

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (was_converted)
            {
                OpenFileDialog fd = new OpenFileDialog();
                if (fd.ShowDialog() == true)
                {
                    File.WriteAllText(fd.FileName, OutputText.Text);
                }
            }
            else
            {
                MessageBox.Show("Please convert file first!");
            }
        }

    }
}
