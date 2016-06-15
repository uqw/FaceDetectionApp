using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;

namespace FaceDetection.Model.Recognition
{
    internal class RecognitionEngine
    {
        #region Fields
        private FaceRecognizer _faceRecognizer;
        #endregion

        #region Construction
        public RecognitionEngine()
        {
            InitializeFaceRecognizer();
        }
        #endregion

        #region Methods

        private void InitializeFaceRecognizer()
        {
            _faceRecognizer = new EigenFaceRecognizer(256, double.PositiveInfinity);
            if (!File.Exists(Properties.Settings.Default.RecognitionTrainFile))
            {
                try
                {
                    File.Create(Properties.Settings.Default.RecognitionTrainFile);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Could not create recognition file: " + ex);
                }
            }
            else
            {
                try
                {
                    _faceRecognizer.Load(Properties.Settings.Default.RecognitionTrainFile);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Could not load recognition file: " + ex);
                }
            }
        }

        public void TrainRecognizer(List<Face> faces)
        {
            var images = new Image<Gray, byte>[faces.Count];
            var labels = new int[faces.Count];

            var i = 0;
            foreach (var face in faces)
            {
                images[i] = face.Grayframe;
                labels[i] = face.User.Id;
                i++;
            }

            _faceRecognizer.Train(images, labels);
            _faceRecognizer.Save(Properties.Settings.Default.RecognitionTrainFile);
        }

        public int RecognizeUser(Image<Gray, byte> grayframe)
        {
            var result = _faceRecognizer.Predict(grayframe);

            return result.Label;
        }
        #endregion
    }
}