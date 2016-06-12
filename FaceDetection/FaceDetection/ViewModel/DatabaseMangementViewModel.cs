using FaceDetection.Model.Recognition;
using FaceDetection.ViewModel.Helpers;
using GalaSoft.MvvmLight;

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
            RefreshData();
            Faces = new AsyncObservableCollection<Face>();
        }
        #endregion

        #region Methods
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
