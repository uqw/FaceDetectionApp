using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using DirectShowLib;
using Emgu.CV;
using Emgu.CV.Structure;
using FaceDetection.Model.Recognition;

namespace FaceDetection.Model
{
    /// <summary>
    /// Handles the camera
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class CameraHandler: IDisposable
    {
        /// <summary>
        /// Represents different process types
        /// </summary>
        public enum ProcessType
        {
            /// <summary>
            /// Processes the front of the face
            /// </summary>
            Front,
            /// <summary>
            /// Processes the profile of the face
            /// </summary>
            Profile,
            /// <summary>
            /// Processes both, <see cref="Profile"/> and <see cref="Front"/> of the face.
            /// </summary>
            Both
        }

        #region Fields
        private static CascadeClassifier _cascadeProfileFace;
        private static CascadeClassifier _cascadeFrontDefault;

        private static int _processedFrames = 0;
        private static List<Rectangle> _lastDetectedPositionsFront;
        private static List<Rectangle> _lastDetectedPositionsProfile;
        #endregion

        #region Construction                
        /// <summary>
        /// Initializes the <see cref="CameraHandler"/> class.
        /// </summary>
        static CameraHandler()
        {
            _lastDetectedPositionsFront = new List<Rectangle>();
            _lastDetectedPositionsProfile = new List<Rectangle>();
            InitializeFaceDetection();
        }
        #endregion

        #region Methods        
        /// <summary>
        /// Gets all connected webcams.
        /// </summary>
        /// <returns>A <see cref="List{Camera}"/> of all webcams.</returns>
        public List<Camera> GetAllCameras()
        {
            var cameras = new List<Camera>();

            var systemCameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            var i = 0;
            foreach (var camera in systemCameras)
            {
                cameras.Add(new Camera(i, camera.Name));
                i++;
            }

            return cameras;
        }

        private static void InitializeFaceDetection()
        {
            try
            {
                var cascadeFile = Path.GetTempFileName();

                File.WriteAllText(cascadeFile, Properties.Resources.haarcascade_profileface);
                _cascadeProfileFace = new CascadeClassifier(cascadeFile);

                File.WriteAllText(cascadeFile, Properties.Resources.haarcascade_frontalface_default);
                _cascadeFrontDefault = new CascadeClassifier(cascadeFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not write temp cascade file: " + ex);
            }
        }

        /// <summary>
        /// Captures and processes the image.
        /// </summary>
        /// <param name="capture">The capture.</param>
        /// <param name="processType">Type of the process.</param>
        /// <returns>null if any exception occured, otherwise the processed image as a <see cref="Bitmap"/>.</returns>
        public Bitmap ProcessImage(Capture capture, ProcessType processType)
        {
            if (capture == null || _cascadeFrontDefault == null)
                return null;

            var imageFrame = capture.QueryFrame().ToImage<Bgr, byte>();

            if (imageFrame == null)
                return null;

            var grayframe = imageFrame.Convert<Gray, byte>();
            
            try
            {
                // Detect the face
                if (processType == ProcessType.Both || processType == ProcessType.Front)
                {
                    if (_processedFrames == 0)
                    {
                        var facesDefault = _cascadeFrontDefault.DetectMultiScale(grayframe, Properties.Settings.Default.ScaleFactorFront, 10, Size.Empty);

                        _lastDetectedPositionsFront.Clear();
                        _lastDetectedPositionsFront.AddRange(facesDefault);
                        foreach (var face in facesDefault)
                        {
                            imageFrame.Draw(face, new Bgr(Color.BlueViolet), 4);
                        }
                    }
                    else
                    {
                        foreach (var face in _lastDetectedPositionsFront)
                        {
                            imageFrame.Draw(face, new Bgr(Color.BlueViolet), 4);
                        }
                    }
                }

                if (processType == ProcessType.Both || processType == ProcessType.Profile)
                {
                    if (_processedFrames == 0)
                    {
                        var facesProfile = _cascadeProfileFace.DetectMultiScale(grayframe, Properties.Settings.Default.ScaleFactorProfile, 10, Size.Empty);

                        _lastDetectedPositionsProfile.Clear();
                        _lastDetectedPositionsProfile.AddRange(facesProfile);
                        foreach (var face in facesProfile)
                        {
                            imageFrame.Draw(face, new Bgr(Color.Aqua), 4);
                        }
                    }
                    else
                    {
                        foreach (var face in _lastDetectedPositionsProfile)
                        {
                            imageFrame.Draw(face, new Bgr(Color.Aqua), 4);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not process image: " + ex);
                return null;
            }

            _processedFrames ++;
            if (_processedFrames > 1)
                _processedFrames = 0;

            return imageFrame.Bitmap;
        }

        /// <summary>
        /// Makes a snapshot and gets the snippets where a face was detected.
        /// </summary>
        /// <param name="capture">The capture.</param>
        /// <param name="processType">Type of the process.</param>
        /// <returns>A <see cref="List{PreviewImage}"/> containing all captured images.</returns>
        public List<PreviewImage> GetDetectedSnippets(Capture capture, ProcessType processType)
        {
            var mat = capture?.QueryFrame();

            var imageList = new List<PreviewImage>();
            if (mat == null)
                return imageList;

            var imageframe = mat.ToImage<Bgr, byte>();
            var grayframe = imageframe.Convert<Gray, byte>();
            
            Rectangle[] faces = null;

            try
            {
                switch (processType)
                {
                    case ProcessType.Front:
                        {
                            faces = _cascadeFrontDefault.DetectMultiScale(grayframe, 1.25, 10, Size.Empty);

                        }
                        break;

                    case ProcessType.Profile:
                        {
                            faces = _cascadeProfileFace.DetectMultiScale(grayframe, 1.25, 10, Size.Empty);
                        }
                        break;

                    default:
                        {
                            return imageList;
                        }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not process snapshot: " + ex);
                return imageList;
            }


            foreach (var face in faces)
            {
                grayframe.ROI = face;
                var detectedGrayframe = grayframe.Copy();
                grayframe.ROI = Rectangle.Empty;

                imageframe.ROI = face;
                var detectedImage = imageframe.Copy();
                imageframe.ROI = Rectangle.Empty;

                imageList.Add(new PreviewImage(detectedImage, detectedGrayframe));
            }

            return imageList;
        }

        /// <summary>
        /// Creates a capture.
        /// </summary>
        /// <param name="selection">The camera selection.</param>
        /// <returns>The created <see cref="Capture"/></returns>
        public Capture CreateCapture(int selection)
        {
            if (selection > -1)
            {
                try
                {
                    return new Capture(selection);
                }
                catch (Exception)
                {
                    Debug.WriteLine("Couldn't read from camera input: " + selection);
                }
            }

            return null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            _cascadeProfileFace.Dispose();
            _cascadeFrontDefault.Dispose();
        }
        #endregion
    }
}
