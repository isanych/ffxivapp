﻿// FFXIVAPP.Client
// PluginInfoBox.xaml.cs
// 
// © 2013 Ryan Wilson

using System;
using MahApps.Metro.Controls;
using NLog;

namespace FFXIVAPP.Client.Controls
{
    /// <summary>
    ///     Interaction logic for PluginInfoBox.xaml
    /// </summary>
    public partial class PluginInfoBox
    {
        #region Logger

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        public PluginInfoBox()
        {
            InitializeComponent();
        }

        private void ToggleSwitchOnCheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var name = ((ToggleSwitch) sender).Tag.ToString();
                Constants.Application.EnabledPlugins[name] = ((ToggleSwitch) sender).IsChecked ?? true;
            }
            catch (Exception ex)
            {
            }
        }
    }
}