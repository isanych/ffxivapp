﻿// FFXIVAPP.Client
// SettingsHelper.cs
// 
// © 2013 Ryan Wilson

using SmartAssembly.Attributes;

namespace FFXIVAPP.Client.Helpers
{
    [DoNotObfuscate]
    internal static partial class SettingsHelper
    {
        /// <summary>
        /// </summary>
        public static void Save(bool isUpdating)
        {
            if (isUpdating)
            {
            }
            Client.Save();
            Application.Save();
            Parse.Save();
        }

        /// <summary>
        /// </summary>
        public static void Default()
        {
            Client.Default();
        }
    }
}
