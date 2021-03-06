﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleNetworkScanner
{
    public static class Settings
    {
        /* PROTOCOL FOR ADDING NEW SETTINGS IN THE FILE
         * 
         * 1.) Each key in the settings needs to be added in the InValidSetting(string)'s array (it has to be unique).
         * 2.) SETTINGS_COUNT property in this file needs to be increased by two per new setting (one is the key, one is the value).
         * 3.) Add the new setting pair by writer in FormMain => CheckLastSaveExit
         * 
         */

        public const string SETTINGS_PATH = "settings.sns";
        public const uint SETTINGS_COUNT = 16;
        private static Dictionary<string, string> SETTINGS_LIST;

        public static void InitSettings()
        {
            SETTINGS_LIST = new Dictionary<string, string>();
            using (StreamReader reader = new StreamReader(SETTINGS_PATH))
            {
                while (!reader.EndOfStream)
                {
                    string key = reader.ReadWord();
                    if (key == String.Empty) break;
                    SETTINGS_LIST.Add(key, reader.ReadLine().Trim());
                }
            }

            if (SETTINGS_LIST.ContainsKey("LAST_SAVE") && !File.Exists(SETTINGS_LIST["LAST_SAVE"])) SETTINGS_LIST.Remove("LAST_SAVE");
        }
        public static bool IsValidSetting(string s) {
            bool b = new[] {"WIN_SIZE", "LAST_SAVE", "DNS_COUNT", "PING_TIMEOUT", "TCP_TIMEOUT", "BRIEF_PORTS"}.Contains(s.ToUpper());
            return b || s.ToUpper().Substring(0, 11) == "DNS_RECORD_";
        }

        public static bool HasKey(string s) { return SETTINGS_LIST.ContainsKey(s); }

        public static string GetSetting(string key) { return SETTINGS_LIST[key]; ;  }

        public static void SetSetting(string key, string value) {
            if (SETTINGS_LIST.ContainsKey(key)) SETTINGS_LIST[key] = value;
            else SETTINGS_LIST.Add(key, value);
        }

        public static void SaveSettings()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(SETTINGS_PATH))
                {
                    foreach (var item in SETTINGS_LIST) writer.WriteLine($"{item.Key} {item.Value}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save settings file!");
                MessageBox.Show(ex.ToString());
            }
        }

        public static IPAddress[] GetSettingsDnsAddresses()
        {
            List<IPAddress> dns = new List<IPAddress>();
            foreach (var record in SETTINGS_LIST) {
                if(record.Key.Length > 11 && record.Key.Substring(0, 11) == "DNS_RECORD_") dns.Add(IPAddress.Parse(record.Value));
            }
            return dns.ToArray();
        }

        public static int[] GetBriefPorts() {
            List<int> ports = new List<int>();
            string toSplit = SETTINGS_LIST["BRIEF_PORTS"].Trim();
            string number = string.Empty;

            for (int i = 0; i < toSplit.Length; i++) {
                
                if (toSplit[i] >= 48 && toSplit[i] <= 57) {     //Split string by ','
                    number += toSplit[i];   //Add if number
                }
                else {
                    ports.Add(int.Parse(number));
                    number = string.Empty;
                }
            }

            ports.Add(int.Parse(number));   //Add last port
            return ports.ToArray();
        }
    }
}
