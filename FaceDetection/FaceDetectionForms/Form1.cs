
using Emgu.CV;
using System;
using System.Windows.Forms;

namespace FaceDetectionForms
{
    public partial class Form1 : Form
    {
        private Capture _capture;

        
        public Form1()
        {
            InitializeComponent();

            try
            {
                _capture = new Capture();
                 imageBox.Image = _capture.QueryFrame();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Could not get capture: " + ex);
            }
            
        }
        
    }
}
