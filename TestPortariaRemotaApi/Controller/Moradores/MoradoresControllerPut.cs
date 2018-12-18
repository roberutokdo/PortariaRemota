using Microsoft.AspNetCore.Mvc;
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
    [CollectionDefinition("MoradoresPut", DisableParallelization = true)]
    public class MoradoresControllerPut
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
                CPF = "54554554545",
                DataNascimento = DateTime.Now.AddDays(-100),
                EMail = "54554554545@kiper.com",
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
        public async Task Morador_PutWithValidValues()
        {
            KiperContext Context = GetContext();
            _MoradoresControleer = new MoradoresController(Context);
            await ForceAddInContext(Context);

            Apartamento apartamento = new Apartamento
            {
                ApartamentoId = 3,
                Bloco = "A",
                Numero = 100
            };

            Morador morador = new Morador
            {
                MoradorId = 2,
                Apartamento = apartamento,
                CPF = "99988877765",
                DataNascimento = DateTime.Now.AddDays(-45),
                EMail = "testesModificado@kiper.com",
                Nome = "Kiper_TESTE",
                Telefone = "(33) 3344-9999"
            };

            var result = await _MoradoresControleer.PutMorador(2, morador) as NoContentResult;
            Assert.Equal(204, result.StatusCode);
        }

        [Fact]
        public async Task Morador_PutWithNoValidValues()
        {
            KiperContext Context = GetContext();
            _MoradoresControleer = new MoradoresController(Context);
            await ForceAddInContext(Context);

            Apartamento apartamento = new Apartamento
            {
                ApartamentoId = 10,
                Bloco = "C",
                Numero = 200
            };

            Morador morador = new Morador
            {
                MoradorId = 10,
                Apartamento = apartamento,
                CPF = "99988877765",
                DataNascimento = DateTime.Now.AddDays(-45),
                EMail = "testesModificado@kiper.com",
                Nome = "Kiper_TESTE",
                Telefone = "(33) 3344-9999"
            };

            var result = await _MoradoresControleer.PutMorador(1, morador) as BadRequestResult;
            Assert.Equal(400, result.StatusCode);
        }
    }
}
