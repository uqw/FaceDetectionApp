using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using DirectShowLib;
using Emgu.CV;
using Emgu.CV.Structure;

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
        private CascadeClassifier _cascadeProfileFace;
        private CascadeClassifier _cascadeFrontDefault;
        #endregion

        #region Construction        
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraHandler"/> class.
        /// </summary>
        public CameraHandler()
        {
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

        private void InitializeFaceDetection()
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

            // Detect the face
            if (processType == ProcessType.Both || processType == ProcessType.Front)
            {
                var facesDefault = _cascadeFrontDefault.DetectMultiScale(grayframe, 1.25, 10, Size.Empty);
                foreach (var face in facesDefault)
                {
                    imageFrame.Draw(face, new Bgr(Color.BlueViolet), 4);
                }
            }

            if (processType == ProcessType.Both || processType == ProcessType.Profile)
            {
                var facesProfile = _cascadeProfileFace.DetectMultiScale(grayframe, 1.25, 10, Size.Empty);
                foreach (var face in facesProfile)
                {
                    imageFrame.Draw(face, new Bgr(Color.Aqua));
                }
            }

            return imageFrame.Bitmap;
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