﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.HackChecklist.SystemChecker
{
    public static class RegistryChecker
    {
        public static string GetLocalRegistryValue(string subKey, string valueKey)
        {
            return GetLocalRegistryValue(subKey, valueKey, GetLocalRegistryKey());
        }

        public static IEnumerable<string> GetLocalRegistryValues(string subKey, string valueKey)
        {            
            return GetLocalRegistryValues(subKey, valueKey, GetLocalRegistryKey(RegistryView.Registry64))
                .Union(GetLocalRegistryValues(subKey, valueKey, GetLocalRegistryKey(RegistryView.Registry32)));
        }

        private static string GetLocalRegistryValue(string subKey, string valueKey, RegistryKey localKey)
        {
            if (string.IsNullOrEmpty(subKey) || string.IsNullOrEmpty(valueKey)) return null;

            try
            {                
                return localKey?.OpenSubKey(subKey).GetValue(valueKey).ToString() ?? null;
            }
            catch
            {
                return null;
            }
        }

        private static RegistryKey GetLocalRegistryKey()
        {
            return (Environment.Is64BitOperatingSystem)
                ? GetLocalRegistryKey(RegistryView.Registry64)
                : GetLocalRegistryKey(RegistryView.Registry32);
        }

        private static RegistryKey GetLocalRegistryKey(RegistryView view)
        {
            return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
        }

        private static IEnumerable<string> GetLocalRegistryValues(string subKey, string valueKey, RegistryKey localKey)
        {
            var searchResult = new List<string>();
            if (localKey == null) return searchResult;

            try
            {
                string value;
                var key = localKey.OpenSubKey(subKey);
                if (key != null)
                {
                    foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                    {
                        value = subkey.GetValue(valueKey) as string;
                        if (value != null)
                        {
                            searchResult.Add(value);
                        }
                    }
                    key.Close();
                }
                return searchResult;
            }
            catch
            {
                return searchResult;
            }
        }
    }
}