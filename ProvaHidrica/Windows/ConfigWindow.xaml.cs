﻿using System.Windows;
using ProvaHidrica.Components;

namespace ProvaHidrica.Windows
{
    /// <summary>
    /// Lógica interna para ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();

            Topmost = true;
            Footer footer = new();
            Footer.Children.Add(footer);
        }

        private void LoadChildren(object? sender, RoutedEventArgs? e)
        {
            switch (TcData.SelectedIndex)
            {
                case 0:
                    PlcConfig plcConfig = new();
                    Plc.Children.Clear();
                    Plc.Children.Add(plcConfig);
                    break;
                case 1:
                    CodeBarsReaderConfig codeBarsReaderConfig = new();
                    CodeBarsReader.Children.Clear();
                    CodeBarsReader.Children.Add(codeBarsReaderConfig);
                    break;
                case 2:
                    DatabaseConfig databaseConfig = new();
                    Database.Children.Clear();
                    Database.Children.Add(databaseConfig);
                    break;
                default:
                    break;
            }
        }
    }
}
