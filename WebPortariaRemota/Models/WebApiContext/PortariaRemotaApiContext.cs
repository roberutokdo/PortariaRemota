using System;
using System.Net.Http;
using System.Net.Http.Headers;
using WebPortariaRemota.Models.Utils;

namespace WebPortariaRemota.Models.WebApiContext
{
    public class PortariaRemotaApiContext
    {
        public  HttpClient ClientApi;
        static Settings SettingsInstance = Settings.Instance;

        private PortariaRemotaApiContext() { ConfigureClientApi(); }
        private static PortariaRemotaApiContext _Instance;
        public static PortariaRemotaApiContext Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new PortariaRemotaApiContext();

                return _Instance;
            }
        }

        private void ConfigureClientApi()
        {
            string apiUrl = SettingsInstance.GetUrlApi();

            if (ClientApi != null) return;
            if (string.IsNullOrWhiteSpace(apiUrl)) return;

            ClientApi = new HttpClient(new HttpClientHandler { UseProxy = false })
            {
                BaseAddress = new Uri(apiUrl)
            };

            ClientApi.DefaultRequestHeaders.Accept.Clear();
            ClientApi.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
