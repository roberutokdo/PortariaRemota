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
    [CollectionDefinition("MoradoresPost", DisableParallelization = true)]
    public class MoradoresControllerPost
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
                CPF = "65665665656",
                DataNascimento = DateTime.Now.AddDays(-100),
                EMail = "65665665656@kiper.com",
                Nome = "Kiper",
                Telefone = "(33) 3344-4578"
            };

            if (Context.Moradores.Include(apto=>apto.Apartamento).FirstOrDefault(x => x.CPF.Equals(morador.CPF) && x.EMail.Equals(morador.EMail) &&  x.Apartamento != null) != null)
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
        public async Task Morador_PostWithValidValues()
        {
            KiperContext Context = GetContext();
            _MoradoresControleer = new MoradoresController(Context);
            await ForceAddInContext(Context);

            Apartamento apartamento = new Apartamento
            {
                Bloco = "B",
                Numero = 101
            };

            Morador morador = new Morador
            {
                Apartamento = apartamento,
                CPF = "23499087043",
                DataNascimento = DateTime.Now.AddDays(-500),
                EMail = "morador@portaria.com",
                Nome = "João",
                Telefone = "(33) 3344-6755"
            };

            var result = await _MoradoresControleer.PostMorador(morador) as CreatedAtActionResult;
            Assert.IsType<Morador>(result.Value);
            var returnedMrd = (Morador)result.Value;
            Assert.Equal(morador.CPF, returnedMrd.CPF);
            Assert.Equal(morador.DataNascimento, returnedMrd.DataNascimento);
            Assert.Equal(morador.EMail, returnedMrd.EMail);
            Assert.Equal(morador.Nome, returnedMrd.Nome);
            Assert.Equal(morador.Telefone, returnedMrd.Telefone);
            Assert.NotNull(morador.Apartamento);
            Assert.Equal(201, result.StatusCode);
        }

        [Fact]
        public async Task Morador_PostWithoutApto()
        {
            KiperContext Context = GetContext();
            _MoradoresControleer = new MoradoresController(Context);
            await ForceAddInContext(Context);

            Morador morador = new Morador
            {
                CPF = "23499087043",
                DataNascimento = DateTime.Now.AddDays(-500),
                EMail = "morador@portaria.com",
                Nome = "João",
                Telefone = "(33) 3344-6755"
            };

            var result = await _MoradoresControleer.PostMorador(morador) as NotFoundObjectResult;
            var jsonSerialize = JsonConvert.SerializeObject(result.Value);
            var jsonResult = JsonConvert.DeserializeObject<CallStatus>(jsonSerialize);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Erro ao tentar incluir o Morador João".ToUpper(), jsonResult.Message.ToUpper());
            Assert.True(jsonResult.Error);
        }

        [Fact]
        public async Task Morador_PostWithDuplicateMorador()
        {
            KiperContext Context = GetContext();
            _MoradoresControleer = new MoradoresController(Context);
            await ForceAddInContext(Context);

            Apartamento apartamento = new Apartamento
            {
                Bloco = "C",
                Numero = 255
            };

            await Context.Apartamentos.AddAsync(apartamento);

            Morador morador = new Morador
            {
                Apartamento = apartamento,
                CPF = "472034729323",
                DataNascimento = DateTime.Now.AddDays(-100),
                EMail = "840923840923@kiper.com",
                Nome = "Kiper",
                Telefone = "(33) 3344-4578"
            };

            await Context.Moradores.AddAsync(morador);
            await Context.SaveChangesAsync();

            var result = await _MoradoresControleer.PostMorador(morador) as NotFoundObjectResult;
            var jsonSerialize = JsonConvert.SerializeObject(result.Value);
            var jsonResult = JsonConvert.DeserializeObject<CallStatus>(jsonSerialize);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Erro ao tentar incluir o Morador Kiper. Já existe um morador com mesma Data de Nascimento e CPF.".ToUpper(), jsonResult.Message.ToUpper());
            Assert.True(jsonResult.Error);
        }
    }
}
