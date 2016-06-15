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

        public double ScaleFactorFront
        {
            get { return Properties.Settings.Default.ScaleFactorFront; }
            set
            {
                RaisePropertyChanged(nameof(ScaleFactorFront));
                Properties.Settings.Default.ScaleFactorFront = value;
            }
        }

        public double ScaleFactorProfile
        {
            get { return Properties.Settings.Default.ScaleFactorProfile; }
            set
            {
                RaisePropertyChanged(nameof(ScaleFactorProfile));
                Properties.Settings.Default.ScaleFactorProfile = value;
            }
        }

        public int MinNeighbours
        {
            get { return Properties.Settings.Default.MinNeighbours; }
            set
            {
                RaisePropertyChanged(nameof(MinNeighbours));
                Properties.Settings.Default.MinNeighbours = value;
            }
        }
        #endregion

        #region Construction
        public MainViewModel()
        {
            if(IsInDesignMode)
                return;

            RecognitionData = new RecognitionData();
        }
        #endregion

        #region Methods
        
        #endregion
    }
}
