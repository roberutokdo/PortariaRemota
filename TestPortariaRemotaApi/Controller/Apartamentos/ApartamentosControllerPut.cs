using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PortariaRemotaAPI.Controllers;
using PortariaRemotaAPI.Models;
using PortariaRemotaAPI.Models.DataContext;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TestPortariaRemotaApi.Controller.Apartamentos
{
    [CollectionDefinition("ApartamentosPut", DisableParallelization = true)]
    public class ApartamentosControllerPut
    {
        private ApartamentosController _ApartamentosController;

        private async Task ForceAddInContext(KiperContext Context)
        {
            Apartamento apartamento = new Apartamento
            {
                Bloco = "D",
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
        public async Task Apartamento_PutWithValidValues()
        {
            KiperContext Context = GetContext();
            _ApartamentosController = new ApartamentosController(Context);
            await ForceAddInContext(Context);

            var apto = Context.Apartamentos.FirstOrDefault();
            var result = await _ApartamentosController.PutApartamento(apto.ApartamentoId, new Apartamento { Bloco = "C", Numero = 200, ApartamentoId = apto.ApartamentoId }) as NoContentResult;
            Assert.Equal(204, result.StatusCode);
        }

        [Fact]
        public async Task Apartamento_PutWithNoValidValues()
        {
            KiperContext Context = GetContext();
            _ApartamentosController = new ApartamentosController(Context);
            await ForceAddInContext(Context);

            Apartamento apartamento = new Apartamento
            {
                ApartamentoId = 10,
                Bloco = "C",
                Numero = 200
            };

            var result = await _ApartamentosController.PutApartamento(1, apartamento) as BadRequestResult;
            Assert.Equal(400, result.StatusCode);
        }
    }
}
