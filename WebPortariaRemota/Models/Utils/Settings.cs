using Newtonsoft.Json;
using System;
using System.IO;

namespace WebPortariaRemota.Models.Utils
{
    public class Settings
    {
        private Settings()
        {
            if (_SettingsJSON is null)
            {
                var json = File.ReadAllText("appsettings.json");
                _SettingsJSON = JsonConvert.DeserializeObject<SettingsJSON>(json);
            }
        }

        private SettingsJSON _SettingsJSON;

        private static Settings _Settings;
        public static Settings Instance
        {
            get
            {
                if (_Settings == null)
                    _Settings = new Settings();

                return _Settings;
            }
        }

        public string GetUrlApi()
        {
            return _SettingsJSON is null ? String.Empty : _SettingsJSON.WebApi.PortariaRemotaApi;
        }

        public string GetEncryptedKey()
        {
            return _SettingsJSON is null ? String.Empty : _SettingsJSON.EncryptedKey.Key;
        }
    }


    public class SettingsJSON
    {
        public Webapi WebApi { get; set; }
        public EncryptedKey EncryptedKey { get; set; }
    }

    public class Webapi
    {
        public string PortariaRemotaApi { get; set; }
    }

    public class EncryptedKey
    {
        public string Key { get; set; }
    }

}
