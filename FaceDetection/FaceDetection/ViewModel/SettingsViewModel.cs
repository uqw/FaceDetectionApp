﻿using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FaceDetection.ViewModel
{
    /// <summary>
    /// The settings view model
    /// </summary>
    public class SettingsViewModel: ViewModelBase
    {
        #region Fields
        private bool _restartButtonShown;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the scale factor front.
        /// </summary>
        /// <value>
        /// The scale factor front.
        /// </value>
        public double ScaleFactorFront
        {
            get { return Properties.Settings.Default.ScaleFactorFront; }
            set
            {
                RaisePropertyChanged(nameof(ScaleFactorFront));
                Properties.Settings.Default.ScaleFactorFront = value;
            }
        }

        /// <summary>
        /// Gets or sets the scale factor profile.
        /// </summary>
        /// <value>
        /// The scale factor profile.
        /// </value>
        public double ScaleFactorProfile
        {
            get { return Properties.Settings.Default.ScaleFactorProfile; }
            set
            {
                RaisePropertyChanged(nameof(ScaleFactorProfile));
                Properties.Settings.Default.ScaleFactorProfile = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum neighbours.
        /// </summary>
        /// <value>
        /// The minimum neighbours.
        /// </value>
        public int MinNeighbours
        {
            get { return Properties.Settings.Default.MinNeighbours; }
            set
            {
                RaisePropertyChanged(nameof(MinNeighbours));
                Properties.Settings.Default.MinNeighbours = value;
            }
        }

        /// <summary>
        /// Gets or sets the execution delay.
        /// </summary>
        /// <value>
        /// The execution delay.
        /// </value>
        public int ExecutionDelay
        {
            get { return Properties.Settings.Default.ExecutionDelay; }
            set
            {
                Properties.Settings.Default.ExecutionDelay = value;
                RaisePropertyChanged(nameof(ExecutionDelay));
                RestartButtonShown = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the restart button is shown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the restart button is shown otherwise, <c>false</c>.
        /// </value>
        public bool RestartButtonShown
        {
            get { return _restartButtonShown; }
            set
            {
                _restartButtonShown = value;
                RaisePropertyChanged(nameof(RestartButtonShown));
            }
        }

        /// <summary>
        /// Gets the restart command.
        /// </summary>
        /// <value>
        /// The restart command.
        /// </value>
        public RelayCommand RestartCommand => new RelayCommand(Restart);
        #endregion

        private void Restart()
        {
            App.RestartApp();
        }
    }
}