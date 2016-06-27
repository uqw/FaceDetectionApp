using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;

namespace FaceDetection.Model.Recognition
{
    /// <summary>
    /// The engine for face recognition
    /// </summary>
    public static class RecognitionEngine
    {
        #region Fields
        private static FaceRecognizer _faceRecognizer;
        private static bool _trained;
        #endregion

        #region Construction        
        /// <summary>
        /// Initializes a new instance of the <see cref="RecognitionEngine"/> class.
        /// </summary>
        static RecognitionEngine()
        {
            InitializeFaceRecognizer();
        }
        #endregion

        #region Methods

        public static void InitializeFaceRecognizer()
        {
            _faceRecognizer?.Dispose();
            _faceRecognizer = new LBPHFaceRecognizer(Properties.Settings.Default.RecognitionRadius, Properties.Settings.Default.RecognitionNeighbours, 8, 8, Properties.Settings.Default.RecognitionThreshold);
            if (!File.Exists(Properties.Settings.Default.RecognitionTrainFile))
            {
                try
                {
                    File.Create(Properties.Settings.Default.RecognitionTrainFile).Close();
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
                    _trained = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Could not load recognition file: " + ex);
                }
            }
        }

        /// <summary>
        /// Trains the recognizer.
        /// </summary>
        /// <param name="faces">The faces.</param>
        public static void TrainRecognizer(List<Face> faces)
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

            _faceRecognizer?.Train(images, labels);
            _trained = true;
            _faceRecognizer?.Save(Properties.Settings.Default.RecognitionTrainFile);
        }

        /// <summary>
        /// Recognizes the user.
        /// </summary>
        /// <param name="grayframe">The grayframe.</param>
        /// <returns></returns>
        public static int RecognizeUser(Image<Gray, byte> grayframe)
        {
            if (!_trained || _faceRecognizer == null)
                return -1;

            var result = _faceRecognizer.Predict(grayframe);

            return result.Label;
        }
        #endregion
    }
}