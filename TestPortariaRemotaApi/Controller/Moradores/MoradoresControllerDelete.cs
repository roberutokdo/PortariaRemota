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
    [CollectionDefinition("MoradoresDelete", DisableParallelization = true)]
    public class MoradoresControllerDelete
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
                CPF = "98998998989",
                DataNascimento = DateTime.Now.AddDays(-100),
                EMail = "98998998989@kiper.com",
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
        public async Task Morador_DeleteWithWrongID()
        {
            KiperContext Context = GetContext();

            _MoradoresControleer = new MoradoresController(Context);
            await ForceAddInContext(Context);

            int moradorID = 99;
            var result = await _MoradoresControleer.DeleteMorador(moradorID) as NotFoundObjectResult;
            var jsonSerialize = JsonConvert.SerializeObject(result.Value);
            var jsonResult = JsonConvert.DeserializeObject<CallStatus>(jsonSerialize);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Erro ao excluir o Morador.".ToUpper(), jsonResult.Message.ToUpper());
            Assert.True(jsonResult.Error);
        }

        [Fact]
        public async Task Morador_DeleteWithCorrectID()
        {
            KiperContext Context = GetContext();

            _MoradoresControleer = new MoradoresController(Context);
            await ForceAddInContext(Context);

            int moradorID = 1;
            var result = await _MoradoresControleer.DeleteMorador(moradorID) as OkResult;
            Assert.Equal(200, result.StatusCode);
        }
    }
}
