using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using PortariaRemotaAPI.Controllers;
using PortariaRemotaAPI.Models;
using PortariaRemotaAPI.Models.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestPortariaRemotaApi.Context;
using Xunit;

namespace TestPortariaRemotaApi.Controller.Apartamentos
{
    [CollectionDefinition("ApartamentosDelete", DisableParallelization = true)]
    public class ApartamentosControllerDelete
    {
        private ApartamentosController _ApartamentosController;

        private async Task ForceAddInContext(KiperContext Context)
        {
            Apartamento apartamento = new Apartamento
            {
                Bloco = "A",
                Numero = 100
            };

            if (Context.Apartamentos.FirstOrDefault(x => x.Bloco.Equals(apartamento.Bloco) && x.Numero == apartamento.Numero) != null)
                return;

            await Context.Apartamentos.AddAsync(apartamento);
            await Context.SaveChangesAsync();
        }

        private KiperContext GetContext()
        {
            DbContextOptions<KiperContext> dbOptions;
            var dbOptionsBuilder = new DbContextOptionsBuilder<KiperContext>();

            dbOptionsBuilder.UseInMemoryDatabase().ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            dbOptions = dbOptionsBuilder.Options;
            return new KiperContext(dbOptions);
        }

        [Fact]
        public async Task Apartamento_DeleteNotFound()
        {
            KiperContext Context = GetContext();

            _ApartamentosController = new ApartamentosController(Context);
            await ForceAddInContext(Context);

            int apartamentoID = 100;
            var result = await _ApartamentosController.DeleteApartamento(apartamentoID) as NotFoundObjectResult;
            var jsonSerialize = JsonConvert.SerializeObject(result.Value);
            var jsonResult = JsonConvert.DeserializeObject<CallStatus>(jsonSerialize);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Erro ao excluir o Apartamento e seus moradores.".ToUpper(), jsonResult.Message.ToUpper());
            Assert.True(jsonResult.Error);
        }

        [Fact]
        public async Task Apartamento_DeleteOk()
        {
            KiperContext Context = GetContext();

            _ApartamentosController = new ApartamentosController(Context);
            await ForceAddInContext(Context);

            int apartamentoID = 1;
            var result = await _ApartamentosController.DeleteApartamento(apartamentoID) as OkResult;
            Assert.Equal(200, result.StatusCode);
        }
    }
}
