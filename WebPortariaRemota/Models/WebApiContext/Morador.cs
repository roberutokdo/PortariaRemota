using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebPortariaRemota.Models.WebApiContext
{
    public class Morador
    {
        private PortariaRemotaApiContext context = PortariaRemotaApiContext.Instance;
        public int MoradorId { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Telefone { get; set; }
        public string CPF { get; set; }
        public string EMail { get; set; }
        
        public virtual Apartamento Apartamento { get; set; }

        public async Task<CallStatus> PostMorador(Morador morador)
        {
            var response = await context.ClientApi.PostAsJsonAsync($"api/Moradores", morador);

            if (!response.IsSuccessStatusCode)
            {
                //Verifica se encontrou erros vindos da Fluent Validation da API.
                return await GetDataResult(response);
            }

            var result = JsonConvert.DeserializeObject<CallStatus>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<CallStatus> PutMorador(Morador morador)
        {
            HttpResponseMessage response = await context.ClientApi.PutAsJsonAsync(
                $"api/Moradores/{morador.MoradorId}", morador);

            if(!response.IsSuccessStatusCode)
            {
                //Verifica se encontrou erros vindos da Fluent Validation da API.
                return await GetDataResult(response);
            }

            var result = JsonConvert.DeserializeObject<CallStatus>(await response.Content.ReadAsStringAsync());
            //Se result é nulo, mostra que os dados foram atualizados com sucesso. Cria apenas manualmente para envio da mensagem para a View.
            return (result is null ? result = new CallStatus { Error = false } : result);
        }

        public async Task<IEnumerable<Morador>> GetMoradores(string searchText)
        {
            HttpResponseMessage response = await context.ClientApi.GetAsync(
                $"api/Moradores");
            response.EnsureSuccessStatusCode();

            IEnumerable<Morador> list = await response.Content.ReadAsAsync<IEnumerable<Morador>>();

            if (!string.IsNullOrEmpty(searchText))
                list = SearchOnList(list, searchText);

            return list;
        }

        public async Task<Morador> GetMorador(int id)
        {
            HttpResponseMessage response = await context.ClientApi.GetAsync(
                $"api/Moradores/{id}");
            response.EnsureSuccessStatusCode();

            Morador morador = await response.Content.ReadAsAsync<Morador>();
            return morador;
        }

        public async Task<CallStatus> DeleteMorador(int id)
        {
            HttpResponseMessage response = await context.ClientApi.DeleteAsync(
                $"api/Moradores/{id}");

            var result = JsonConvert.DeserializeObject<CallStatus>(await response.Content.ReadAsStringAsync());
            return result;
        }

        private async Task<CallStatus> GetDataResult(HttpResponseMessage response)
        {
            string jsonResult = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(jsonResult))
            {
                string message = string.Empty;
                dynamic jsonParse = JObject.Parse(jsonResult);
                if (jsonParse.Nome != null)
                    message += jsonParse.Nome + Environment.NewLine;
                if (jsonParse.DataNascimento != null)
                    message += jsonParse.DataNascimento + Environment.NewLine;
                if (jsonParse.Telefone != null)
                    message += jsonParse.Telefone + Environment.NewLine;
                if (jsonParse.CPF != null)
                    message += jsonParse.CPF + Environment.NewLine;
                if (jsonParse.EMail != null)
                    message += jsonParse.EMail + Environment.NewLine;
                if (jsonParse.message != null)
                    message += jsonParse.message + Environment.NewLine;
                if (jsonParse.error != null)
                    message += jsonParse.error + Environment.NewLine;

                return new CallStatus { Message = message, Error = true };
            }

            return null;
        }

        private IEnumerable<Morador> SearchOnList(IEnumerable<Morador> list, string searchText)
        {
            return ((IList<Morador>)list).Where(mrd => mrd.Nome.ToUpper().Contains(searchText.Trim().ToUpper()) ||
                mrd.Telefone.ToUpper().Contains(searchText.Trim().ToUpper()) ||
                mrd.CPF.ToUpper().Contains(searchText.Trim().ToUpper()) ||
                mrd.DataNascimento.Date.ToString().ToUpper().Contains(searchText.Trim().ToUpper()) ||
                mrd.EMail.ToUpper().Contains(searchText.Trim().ToUpper()));
        }
    }
}
