using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebPortariaRemota.Models.WebApiContext
{
    public class Apartamento
    {
        private PortariaRemotaApiContext context = PortariaRemotaApiContext.Instance;

        public int ApartamentoId { get; set; }
        public string Bloco { get; set; }
        public int Numero { get; set; }

        public virtual IList<Morador> Moradores { get; set; }

        public async Task<CallStatus> PutApartamento(Apartamento apartamento)
        {
            HttpResponseMessage response = await context.ClientApi.PutAsJsonAsync(
                $"api/Apartamentos/{apartamento.ApartamentoId}", apartamento);

            var result = JsonConvert.DeserializeObject<CallStatus>(await response.Content.ReadAsStringAsync());
            //Se result é nulo, mostra que os dados foram atualizados com sucesso. Cria apenas manualmente para envio da mensagem para a View.
            return (result is null ? result = new CallStatus { Error = false } : result);
        }

        public async Task<IEnumerable<Apartamento>> GetApartamentos(string searchText)
        {
            HttpResponseMessage response = await context.ClientApi.GetAsync(
                $"api/Apartamentos");
            response.EnsureSuccessStatusCode();

            IEnumerable<Apartamento> list = await response.Content.ReadAsAsync<IEnumerable<Apartamento>>();

            if (!string.IsNullOrEmpty(searchText))
                list = SearchOnList(list, searchText);

            return list;
        }

        public async Task<Apartamento> GetApartamento(int id)
        {
            HttpResponseMessage response = await context.ClientApi.GetAsync(
                $"api/Apartamentos/{id}");
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();

                Apartamento apartamento = await response.Content.ReadAsAsync<Apartamento>();
                return apartamento;
            }

            return null;
        }

        public async Task<CallStatus> DeleteApartamento(int id)
        {
            HttpResponseMessage response = await context.ClientApi.DeleteAsync(
                $"api/Apartamentos/{id}");

            var result = JsonConvert.DeserializeObject<CallStatus>(await response.Content.ReadAsStringAsync());
            return result;
        }

        private IEnumerable<Apartamento> SearchOnList(IEnumerable<Apartamento> list, string searchText)
        {
            return ((IList<Apartamento>)list).Where(apto => apto.Bloco.ToUpper().Contains(searchText.Trim().ToUpper()) || apto.Numero.ToString().Contains(searchText.Trim().ToUpper()));
        }
    }
}
