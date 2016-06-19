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
        public bool DialogCoordinatorRegistered { get; set; }
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

            while (!DialogCoordinatorRegistered)
            {
                await Task.Delay(25);
            }

            if (await _updateHandler.IsUpdateAvailable())
            {
                IsUpdating = true;
                var result = await
                    DialogCoordinator.ShowMessageAsync(this, "Update available", $"An update is available.{Environment.NewLine}{Environment.NewLine}Current installed version: {_updateHandler.LocaVersion}.{Environment.NewLine}Available version: {_updateHandler.RemoteVersion}{Environment.NewLine}{Environment.NewLine}Do you want to install this update now?",
                        MessageDialogStyle.AffirmativeAndNegative);

                if (result == MessageDialogResult.Affirmative)
                {
                    _updateHandler.UpdateDownloadProgressChanged += (sender, args) =>
                    {
                        _progressController.SetProgress(args.ProgressPercentage / 100.0);
                        _progressController.SetMessage($"Downloading...{Environment.NewLine}Received: {args.MegabytesReceived} MB{Environment.NewLine}Total Size: {args.MegabytesToReceive} MB");
                    };

                    _updateHandler.UpdateDownloadCompleted += async (sender, args) =>
                    {
                        if (args.Aborted)
                        {
                            _progressController?.SetMessage("Failed to download update");
                        }
                        else
                        {
                            _progressController?.SetMessage("Completed!");
                        }

                        await Task.Delay(3000);
                        try
                        {
                            await _progressController?.CloseAsync();
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                        
                        IsUpdating = false;
                    };

                    var task = _updateHandler.DownloadUpdate();

                    _progressController = await DialogCoordinator.ShowProgressAsync(this, "Downloading update", "", true);
                    _progressController.Canceled += (sender, args) =>
                    {
                        _progressController.CloseAsync();
                        _updateHandler.CancelDownload();
                        IsUpdating = false;
                    };
                }
                else
                {
                    IsUpdating = false;
                }

            }
        }
        #endregion
    }
}
