using FaceDetection.Model.Recognition;
using FaceDetection.ViewModel.Helpers;
using FaceDetection.ViewModel.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace FaceDetection.ViewModel 
{
    class UserMangementViewModel : ViewModelBase
    {
        #region Fields
        private AsyncObservableCollection<User> _users;
        #endregion

        #region Properties
        public AsyncObservableCollection<User> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                RaisePropertyChanged(nameof(Users));
            }
        }
        #endregion

        #region Construction
        public UserMangementViewModel()
        {

            if (IsInDesignMode)
                return;
            Users = new AsyncObservableCollection<User>();
            InitializeMessageHandlers();
        }
        #endregion

        #region Methods
        private void InitializeMessageHandlers()
        {
            Messenger.Default.Register<TabSelectionChangedMessage>(this,
            (message) =>
            {
                if (message.Index == 2)
                {
                    Users = new AsyncObservableCollection<User>(RecognitionData.AllUsers);
                }
            });
        }
        #endregion
    }
}
