using FaceDetection.Model.Recognition;
using FaceDetection.ViewModel.Helpers;
using FaceDetection.ViewModel.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace FaceDetection.ViewModel
{
    internal class DatabaseMangementViewModel: ViewModelBase
    {
        #region Fields
        private AsyncObservableCollection<Face> _faces;
        #endregion

        #region Properties
        public AsyncObservableCollection<Face> Faces
        {
            get { return _faces; }
            set
            {
                _faces = value;
                RaisePropertyChanged(nameof(Faces));
            }
        }
        #endregion

        #region Construction
        public DatabaseMangementViewModel()
        {
            if(!IsInDesignMode)
                RefreshData();

            Faces = new AsyncObservableCollection<Face>();
            InitialzeMessageListeners();
        }
        #endregion

        #region Methods
        private void InitialzeMessageListeners()
        {
            Messenger.Default.Register<FaceAddedMessage>(this,
                (message) =>
                {
                   Faces.Add(message.Face); 
                });
        }

        private async void RefreshData()
        {
            var data = await MainViewModel.RecognitionData.GetAllFaces();

            if(data == null)
                return;

            Faces.Clear();
            foreach (var face in data)
            {
                Faces.Add(face);
            }
        }
        #endregion
    }
}
