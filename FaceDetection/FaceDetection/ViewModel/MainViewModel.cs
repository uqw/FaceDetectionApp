using FaceDetection.Model.Recognition;
using GalaSoft.MvvmLight;
using FaceDetection.ViewModel.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace FaceDetection.ViewModel
{
    internal sealed class MainViewModel: ViewModelBase
    {
        #region Fields
        private int _selectedTab;
        public static RecognitionData RecognitionData;
        #endregion

        #region Properties
        public int SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                if (_selectedTab != value)
                {
                    _selectedTab = value;
                    RaisePropertyChanged(nameof(SelectedTab));
                    Messenger.Default.Send(new TabSelectionChangedMessage(value));
                }
            }
        }
        #endregion

        #region Construction
        public MainViewModel()
        {
            RecognitionData = new RecognitionData();
        }
        #endregion

        #region Methods
        
        #endregion
    }
}
