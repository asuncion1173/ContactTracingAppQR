using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.Aztec;
using System.IO;

namespace ContactTracingAppQR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
      
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice captureDevice;

        private void Form1_Load(object sender, EventArgs e)
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                comboBox.Items.Add(filterInfo.Name);
            comboBox.SelectedIndex = 0;
            timerDateTime.Start();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            captureDevice = new VideoCaptureDevice(filterInfoCollection[0].MonikerString);
            captureDevice.NewFrame += CaptureDevice_NewFrame;
            captureDevice.Start();
            timer1.Start();
        }

        private void CaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            cameraQR.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (captureDevice.IsRunning)
                captureDevice.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (cameraQR.Image != null)
            {
                ZXing.BarcodeReader barcodeReader = new ZXing.BarcodeReader();
                ZXing.Result qrData = barcodeReader.Decode((Bitmap)cameraQR.Image);
                if (qrData != null)
                {
                    MessageBox.Show("Your data has been added to our system!");
                    if (captureDevice.IsRunning)
                        captureDevice.Stop();
                    timer1.Stop();
                    StreamWriter output;
                    output = File.CreateText(@"C:\Users\ASUNCION\Desktop\ContactTracingQR\" + "Data from QR" + ".txt");
                    output.WriteLine(DateTime.Now.ToString());
                    output.WriteLine("__________________________________________________________" + "\n");
                    output.WriteLine(qrData.ToString());
                    output.Close();
                }
            }
            

        }

        private void timerDateTime_Tick(object sender, EventArgs e)
        {
            labelTime.Text = DateTime.Now.ToString();
        }
    }
}

