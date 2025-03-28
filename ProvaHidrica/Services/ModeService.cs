﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ProvaHidrica.Components;
using ProvaHidrica.Interfaces;
using ProvaHidrica.Utils;

namespace ProvaHidrica.Services
{
    public class ModeService : IModeService
    {
        private readonly DispatcherTimer _buttonPressTimer;
        private bool _isButtonPressed;
        private readonly MainApplication MainApplication;
        private readonly LogService LogService;
        public bool IsAutomatic = false;
        public bool IsMaintenance = false;

        public ModeService(MainApplication mainApplication, LogService logService)
        {
            MainApplication = mainApplication;
            LogService = logService;
            _buttonPressTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _buttonPressTimer.Tick += ButtonPressTimer_Tick;
        }

        public void SetMode()
        {
            if (MainApplication == null)
                return;

            if (!IsAutomatic)
            {
                MainApplication.ModeButton.Foreground = Brushes.LightBlue;
                MainApplication.ModeButton.ToolTip = "Modo manual";
                MainApplication.ModeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Hand;
                MainApplication.VPInput.IsEnabled = true;
                MainApplication.CISInput.IsEnabled = true;
                MainApplication.ChassisInput.IsEnabled = true;
                IsMaintenance = false;
                return;
            }
            try
            {
                MainApplication.ModeButton.Foreground = Brushes.LightGreen;
                MainApplication.ModeButton.ToolTip = "Modo automático";
                MainApplication.ModeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Automatic;
                MainApplication.VPInput.IsEnabled = false;
                MainApplication.CISInput.IsEnabled = false;
                MainApplication.ChassisInput.IsEnabled = false;
                IsMaintenance = false;
            }
            catch (Exception)
            {
                IsAutomatic = false;

                if (!IsMaintenance)
                    SetMode();

                return;
            }
        }

        public async void SwitchMode(object sender, RoutedEventArgs e)
        {
            if (!Auth.UserHasPermission("OM"))
            {
                ErrorMessage.Show("Usuário não tem permissão para alterar o modo de operação.");
                return;
            }
            if (!IsMaintenance)
                IsAutomatic = !IsAutomatic;

            SetMode();
            await LogService.LogSysSwitchedMode(IsAutomatic ? "Automático" : "Manual");
        }

        public void ModeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isButtonPressed = true;
            _buttonPressTimer.Start();
        }

        public void ModeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isButtonPressed = false;
            _buttonPressTimer.Stop();
        }

        private void ButtonPressTimer_Tick(object? sender, EventArgs e)
        {
            if (_isButtonPressed)
            {
                _buttonPressTimer.Stop();
                EnterMaintenanceMode();
            }
        }

        private async void EnterMaintenanceMode()
        {
            if (MainApplication == null)
                return;

            MainApplication.ModeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Tools;
            MainApplication.ModeButton.Foreground = Brushes.Orange;
            MainApplication.ModeButton.ToolTip = "Modo de manutenção";
            MainApplication.VPInput.IsEnabled = true;
            MainApplication.CISInput.IsEnabled = true;
            IsAutomatic = false;
            IsMaintenance = true;

            MessageBox.Show(
                "Modo de manutenção ativado. Logs e registros de operações não serão salvos.",
                "Aviso!"
            );

            await LogService.LogSysSwitchedMode("Manutenção");
        }
    }
}
