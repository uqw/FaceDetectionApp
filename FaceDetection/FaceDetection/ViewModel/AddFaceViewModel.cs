using System;
using System.Threading.Tasks;
using FaceDetection.Model;
using FaceDetection.Model.Recognition;
using FaceDetection.ViewModel.Helpers;
using FaceDetection.ViewModel.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace FaceDetection.ViewModel
{
    internal class AddFaceViewModel: ViewModelBase
    {
        #region Fields
        private AsyncObservableCollection<PreviewImage> _previewImages;
        private readonly CameraHandler _cameraHandler;
        private int _selectedImage;
        private bool _previewHeaderVisible;
        private string _resultText;
        private string _usernameText;
        #endregion

        #region Properties
        public AsyncObservableCollection<PreviewImage> PreviewImages
        {
            get { return _previewImages; }

            set
            {
                _previewImages = value;
                RaisePropertyChanged(nameof(PreviewImages));
            }
        }

        public int SelectedImage
        {
            get { return _selectedImage; }

            set
            {
                _selectedImage = value;
                RaisePropertyChanged(nameof(SelectedImage));
            }
        }

        public bool PreviewHeaderVisible
        {
            get { return _previewHeaderVisible; }
            set
            {
                _previewHeaderVisible = value;
                RaisePropertyChanged(nameof(PreviewHeaderVisible));
            }
        }

        public string UsernameText
        {
            get { return _usernameText; }

            set
            {
                _usernameText = value;
                RaisePropertyChanged(nameof(UsernameText));
            }
        }

        public string ResultText
        {
            get { return _resultText; }
            set
            {
                _resultText = value;
                RaisePropertyChanged(nameof(ResultText));
            }
        }

        public RelayCommand TakeSnapshotCommand => new RelayCommand(TakeSnapshot);
        public RelayCommand AddFaceCommand => new RelayCommand(AddFace);
        #endregion

        public AddFaceViewModel()
        {
            _cameraHandler = new CameraHandler();
            PreviewImages = new AsyncObservableCollection<PreviewImage>();

            if (IsInDesignMode)
            {
                PreviewHeaderVisible = true;
            }
        }

        private void TakeSnapshot()
        {
            var previewImages = _cameraHandler.GetDetectedSnippets(CameraViewModel.Capture, CameraHandler.ProcessType.Front);
            
            PreviewImages.Clear();
            foreach (var image in previewImages)
            {
                PreviewImages.Add(image);
            }

            PreviewHeaderVisible = true;
            SelectedImage = 0;
        }

        private async void AddFace()
        {
            if(SelectedImage != -1)
                return;

            var selectedImage = PreviewImages[SelectedImage];

            var addedFaceData = await MainViewModel.RecognitionData.InsertFace(selectedImage.Original, selectedImage.Grayframe, UsernameText);

            if (addedFaceData.FaceId != -1)
            {
                /* Messenger.Default.Send(new FaceAddedMessage(
                    new Face(selectedImage.Original.Bytes, selectedImage.Grayframe.Bytes, (int)addedFaceData.FaceId, UsernameText, (int)addedFaceData.UserId, selectedImage.Original.Width, selectedImage.Original.Height)
                    ));
                */

                ResultText = $"Success: ID {addedFaceData.FaceId}";
                PreviewImages.Clear();
                PreviewHeaderVisible = false;
                UsernameText = "";
            }
            else
            {
                ResultText = "Error!";
            }

            await Task.Delay(3000);
            ResultText = "";
        }
    }
}
