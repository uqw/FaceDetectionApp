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
            Faces = new AsyncObservableCollection<Face>();
            InitializeMessageHandlers();
        }
        #endregion

        #region Methods
        private void InitializeMessageHandlers()
        {
            Messenger.Default.Register<TabSelectionChangedMessage>(this,
            (message) =>
            {
                if (message.Index == 1)
                {
                    Faces = new AsyncObservableCollection<Face>(RecognitionData.AllFaces);
                }
            });
        }
        #endregion
    }
}
