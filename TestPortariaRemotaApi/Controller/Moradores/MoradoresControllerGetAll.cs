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
using Xunit;

namespace TestPortariaRemotaApi.Controller.Moradores
{
    [CollectionDefinition("MoradoresGetAll", DisableParallelization = true)]
    public class MoradoresControllerGetAll
    {
        private MoradoresController _MoradoresControleer;

        private async Task ForceAddInContext(KiperContext Context)
        {
            Apartamento apartamento = new Apartamento
            {
                Bloco = "A",
                Numero = 100
            };

            Morador morador = new Morador
            {
                Apartamento = apartamento,
                CPF = "87887887878",
                DataNascimento = DateTime.Now.AddDays(-100),
                EMail = "87887887878@kiper.com",
                Nome = "Kiper",
                Telefone = "(33) 3344-4578"
            };

            if (Context.Moradores.FirstOrDefault(x => x.CPF.Equals(morador.CPF) && x.EMail.Equals(morador.EMail)) != null)
                return;

            await Context.Moradores.AddAsync(morador);
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
        public async Task Morador_GetAllReturnList()
        {
            KiperContext Context = GetContext();

            _MoradoresControleer = new MoradoresController(Context);
            await ForceAddInContext(Context);
            var result = _MoradoresControleer.GetMoradores() as IEnumerable<Morador>;

            Assert.NotEmpty(result);
        }
    }
}
