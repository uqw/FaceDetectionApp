using FaceDetection.Model.Recognition;
using FaceDetection.ViewModel.Helpers;
using FaceDetection.ViewModel.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Users = new AsyncObservableCollection<User>();

            if (IsInDesignMode)
                return;
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
                    Users = new AsyncObservableCollection<User>(MainViewModel.RecognitionData.AllUsers);
                }
            });
        }
        #endregion
    }
}
