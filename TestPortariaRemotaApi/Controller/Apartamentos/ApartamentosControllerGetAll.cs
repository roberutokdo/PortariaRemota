using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
    [CollectionDefinition("ApartamentosGetAll", DisableParallelization = true)]
    public class ApartamentosControllerGetAll
    {
        private ApartamentosController _ApartamentosController;

        private async Task ForceAddInContext(KiperContext Context)
        {
            Apartamento apartamento = new Apartamento
            {
                Bloco = "B",
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
        public async Task Apartamento_GetAllReturnList()
        {
            KiperContext Context = GetContext();

            _ApartamentosController = new ApartamentosController(Context);
            await ForceAddInContext(Context);
            var result = _ApartamentosController.GetApartamentos() as IEnumerable<Apartamento>;

            Assert.NotEmpty(result);
        }
    }
}
