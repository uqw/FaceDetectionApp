using System;
using System.Threading.Tasks;
using FaceDetection.Model.Recognition;
using FaceDetection.Model.Updater;
using GalaSoft.MvvmLight;
using FaceDetection.ViewModel.Messages;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;

namespace FaceDetection.ViewModel
{
    internal sealed class MainViewModel: ViewModelBase
    {
        #region Fields
        private int _selectedTab;
        private readonly UpdateHandler _updateHandler;
        private ProgressDialogController _progressController;
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

        public IDialogCoordinator DialogCoordinator { get; set; }

        public static bool IsUpdating { get; private set; }
        #endregion

        #region Construction
        public MainViewModel()
        {
            _updateHandler = new UpdateHandler();
            CheckForUpdates();
        }
        #endregion

        #region Methods

        private async void CheckForUpdates()
        {
            await Task.Delay(3000);

            while (DialogCoordinator == null)
            {
                await Task.Delay(25);
            }

            if (await _updateHandler.IsUpdateAvailable()) 
            {
                var result = await
                    DialogCoordinator.ShowMessageAsync(this, "Update available", $"An update is available.{Environment.NewLine}{Environment.NewLine}Current installed version: {_updateHandler.LocaVersion}.{Environment.NewLine}Available version: {_updateHandler.RemoteVersion}{Environment.NewLine}{Environment.NewLine}Do you want to install this update now?",
                        MessageDialogStyle.AffirmativeAndNegative);

                if (result == MessageDialogResult.Affirmative)
                {
                    // Todo: implement downloader

                    _progressController = await DialogCoordinator.ShowProgressAsync(this, "Test", "test", true);
                    _progressController.Canceled += (sender, args) =>
                    {
                        _progressController.CloseAsync();
                    };
                }
            }
        }
        #endregion
    }
}
