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

namespace TestPortariaRemotaApi.Controller.Moradores
{
    [CollectionDefinition("MoradoresGetOne", DisableParallelization = true)]
    public class MoradoresControllerGetOne
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
                CPF = "76776776767",
                DataNascimento = DateTime.Now.AddDays(-100),
                EMail = "76776776767@kiper.com",
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
        public async Task Morador_GetOneNotFound()
        {
            KiperContext Context = GetContext();

            _MoradoresControleer = new MoradoresController(Context);
            await ForceAddInContext(Context);

            int moradorID = 15;
            var result = await _MoradoresControleer.GetMorador(moradorID) as NotFoundObjectResult;
            var jsonSerialize = JsonConvert.SerializeObject(result.Value);
            var jsonResult = JsonConvert.DeserializeObject<CallStatus>(jsonSerialize);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Morador não encontrado".ToUpper(), jsonResult.Message.ToUpper());
            Assert.True(jsonResult.Error);
        }

        [Fact]
        public async Task Morador_GetOneFound()
        {
            KiperContext Context = GetContext();

            _MoradoresControleer = new MoradoresController(Context);
            await ForceAddInContext(Context);

            int moradorID = Context.Moradores.FirstOrDefault(x=>x.CPF.Equals("76776776767")).MoradorId;

            Apartamento apartamento = new Apartamento
            {
                Bloco = "A",
                Numero = 100
            };

            Morador morador = new Morador
            {
                Apartamento = apartamento,
                CPF = "76776776767",
                DataNascimento = DateTime.Now.AddDays(-100),
                EMail = "76776776767@kiper.com",
                Nome = "Kiper",
                Telefone = "(33) 3344-4578"
            };

            var result = await _MoradoresControleer.GetMorador(moradorID) as OkObjectResult;
            Assert.IsType<Morador>(result.Value);
            var returnedMrd = (Morador)result.Value;
            Assert.Equal(morador.CPF, returnedMrd.CPF);
            Assert.Equal(morador.DataNascimento.Date, returnedMrd.DataNascimento.Date);
            Assert.Equal(morador.EMail, returnedMrd.EMail);
            Assert.Equal(morador.Nome, returnedMrd.Nome);
            Assert.Equal(morador.Telefone, returnedMrd.Telefone);
            Assert.Equal(morador.Apartamento.Bloco, returnedMrd.Apartamento.Bloco);
            Assert.Equal(morador.Apartamento.Numero, returnedMrd.Apartamento.Numero);
            Assert.Equal(200, result.StatusCode);
        }
    }
}
