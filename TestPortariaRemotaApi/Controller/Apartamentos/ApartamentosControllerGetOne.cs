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
    [CollectionDefinition("ApartamentosGetOne", DisableParallelization = true)]
    public class ApartamentosControllerGetOne
    {
        private ApartamentosController _ApartamentosController;

        private async Task ForceAddInContext(KiperContext Context)
        {
            Apartamento apartamento = new Apartamento
            {
                Bloco = "C",
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
        public async Task Apartamento_GetOneNotFound()
        {
            KiperContext Context = GetContext();

            _ApartamentosController = new ApartamentosController(Context);
            await ForceAddInContext(Context);

            int apartamentoID = 100;
            var result = await _ApartamentosController.GetApartamento(apartamentoID) as NotFoundObjectResult;
            var jsonSerialize = JsonConvert.SerializeObject(result.Value);
            var jsonResult = JsonConvert.DeserializeObject<CallStatus>(jsonSerialize);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Apartamento não encontrado".ToUpper(), jsonResult.Message.ToUpper());
            Assert.True(jsonResult.Error);
        }

        [Fact]
        public async Task Apartamento_GetOneFound()
        {
            KiperContext Context = GetContext();

            _ApartamentosController = new ApartamentosController(Context);
            await ForceAddInContext(Context);

            Apartamento apartamento = new Apartamento
            {
                Bloco = "C",
                Numero = 100
            };

            int apartamentoID = Context.Apartamentos.FirstOrDefault(x => x.Bloco.Equals(apartamento.Bloco) && x.Numero == apartamento.Numero).ApartamentoId;

            var result = await _ApartamentosController.GetApartamento(apartamentoID) as OkObjectResult;
            Assert.IsType<Apartamento>(result.Value);
            var returnedApto = (Apartamento)result.Value;
            Assert.Equal(apartamento.Bloco, returnedApto.Bloco);
            Assert.Equal(apartamento.Numero, returnedApto.Numero);
            Assert.Equal(200, result.StatusCode);
        }
    }
}
