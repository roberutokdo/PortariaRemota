using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using WebPortariaRemota.Models;
using WebPortariaRemota.Models.WebApiContext;

namespace WebPortariaRemota.Controllers
{
    public class ManagementController : Controller
    {
        private readonly Apartamento Apartamento = new Apartamento();
        private IEnumerable<Apartamento> Apartamentos = new List<Apartamento>();
        private readonly Morador Morador = new Morador();

        [Authorize]
        public async Task<IActionResult> Index(string mySearch)
        {
            var list = await this.Apartamento.GetApartamentos(mySearch);
            IList<ApartamentoViewModel> viewModel = new List<ApartamentoViewModel>();

            foreach(Apartamento apto in list)
            {
                viewModel.Add(new ApartamentoViewModel
                {
                    ApartamentoId = apto.ApartamentoId,
                    Bloco = apto.Bloco,
                    Numero = apto.Numero,
                    Moradores = apto.Moradores
                });
            }
            return View(viewModel);
        }
        
        [Authorize]
        public IActionResult CreateApartamento()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateApartamento(ApartamentoViewModel apto)
        {
            if (!ModelState.IsValid)
                return View();

            Apartamento apartamento = new Apartamento()
            {
                Bloco = apto.Bloco,
                Numero = apto.Numero
            };

            //Asp.Net Core ainda não suporta tipos complexos no TempData.
            TempData["Apartamento"] = JsonConvert.SerializeObject(apartamento);
            return RedirectToAction("CreateMorador", "Management");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateMorador()
        {
            MoradorViewModel morador = await GetSelectAptoList(false);

            //Se entrar significa que o usuário vem da sessão de cadastro do Apartamento.
            //Ação necessária para garantir que o Apartamento somente seja incluído junto de um morador.
            if (TempData["Apartamento"] != null)
            {
                string jsonApartamento = TempData["Apartamento"] as string;
                byte[] jsonData = Encoding.ASCII.GetBytes(jsonApartamento);
                HttpContext.Session.Set("Apartamento", jsonData);
                morador.Apartamento = JsonConvert.DeserializeObject<Apartamento>(TempData["Apartamento"] as string);
            }
            else
            {
                //para garantir que se o usuário iniciar um novo cadastro, não busque algo da sessão.
                HttpContext.Session.Remove("Apartamento");
            }
            return View(morador);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMorador(MoradorViewModel morador)
        {
            if (!ModelState.IsValid)
            {
                MoradorViewModel errorMoradorViewModel = await GetSelectAptoList(false);
                return View(errorMoradorViewModel);
            }

            //Busca o apartamento ainda não cadastrado na base que vem desde a View CreateApartamento
            HttpContext.Session.TryGetValue("Apartamento", out byte[] jsonData);
            if (jsonData != null)
                morador.Apartamento = JsonConvert.DeserializeObject<Apartamento>(Encoding.ASCII.GetString(jsonData));
            else if (string.IsNullOrEmpty(morador.SelectApartamento))
            {
                MoradorViewModel errorMoradorViewModel = await GetSelectAptoList(false);
                ViewBag.Message = "É obrigatório o vínculo de um apartamento ao morador";
                return View(errorMoradorViewModel);
            }

            Morador mrd = new Morador()
            {
                CPF = morador.CPF,
                DataNascimento = morador.DataNascimento,
                EMail = morador.EMail,
                Nome = morador.Nome,
                Telefone = morador.Telefone,
                Apartamento = await GetApartamentoFromView(morador)
            };

            var response = await this.Morador.PostMorador(mrd);

            if (response.Error)
                ViewBag.Message = response.Message;
            else
                ViewBag.Status = "Morador incluído com sucesso!";

            morador.Apartamentos = await GetDropDownItemsApartamentos(true);
            return View(morador);
        }

        [Authorize]
        public async Task<IActionResult> IndexMoradores(string mySearch)
        {
            var list = await this.Morador.GetMoradores(mySearch);
            IList<MoradorViewModel> viewModel = new List<MoradorViewModel>();

            foreach (Morador mrd in list)
            {
                viewModel.Add(new MoradorViewModel
                {
                    Apartamento = mrd.Apartamento,
                    CPF = mrd.CPF,
                    DataNascimento = mrd.DataNascimento,
                    EMail = mrd.EMail,
                    MoradorId = mrd.MoradorId,
                    Nome = mrd.Nome,
                    Telefone = mrd.Telefone
                });
            }
            return View(viewModel);
        }

        private async Task<Apartamento> GetApartamentoFromView(MoradorViewModel morador)
        {
            this.Apartamentos = await FillApartamentosList(false);
            return string.IsNullOrEmpty(morador.SelectApartamento) ? morador.Apartamento : this.Apartamentos.First(x => x.ApartamentoId.Equals(Convert.ToInt32(morador.SelectApartamento)));
        }

        private async Task<IEnumerable<Apartamento>> FillApartamentosList(bool force)
        {
            if(force)
                return await this.Apartamento.GetApartamentos(null);
            if (TempData["ListaApartamentos"] is null)
                return await this.Apartamento.GetApartamentos(null);
            else
                return JsonConvert.DeserializeObject<IEnumerable<Apartamento>>(TempData["ListaApartamentos"] as string);
        }

        private async Task<MoradorViewModel> GetSelectAptoList(bool force)
        {
            MoradorViewModel morador = new MoradorViewModel();
            morador.Apartamentos = await GetDropDownItemsApartamentos(force);
            TempData["ListaApartamentos"] = JsonConvert.SerializeObject(this.Apartamentos);
            return morador;
        }

        private async Task<SelectList> GetDropDownItemsApartamentos(bool force)
        {
            this.Apartamentos = await FillApartamentosList(force);
            Dictionary<int, string> selectApartamentos = new Dictionary<int, string>();
            foreach (Apartamento apto in this.Apartamentos)
            {
                selectApartamentos.Add(apto.ApartamentoId, $"Bloco: {apto.Bloco} - Número: {apto.Numero}");
            }

            return new SelectList(selectApartamentos, "Key", "Value");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteApartamento(int id)
        {
            if (!ModelState.IsValid)
                return View();

            await this.Apartamento.DeleteApartamento(id);

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditApartamento(int id)
        {
            if (!ModelState.IsValid)
                return View();

            var apartamento = await this.Apartamento.GetApartamento(id);

            if (apartamento is null)
            {
                ViewBag.Message = "Ocorreu um erro ao tentar buscar os dados do Apartamento para edição.";
                return View();
            }

            ApartamentoViewModel result = new ApartamentoViewModel
            {
                ApartamentoId = apartamento.ApartamentoId,
                Bloco = apartamento.Bloco,
                Numero = apartamento.Numero
            };

            return View(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditApartamento(ApartamentoViewModel view)
        {
            if (!ModelState.IsValid)
                return View(view);

            Apartamento apto = new Apartamento
            {
                ApartamentoId = view.ApartamentoId,
                Bloco = view.Bloco,
                Numero = view.Numero
            };
            var response = await this.Apartamento.PutApartamento(apto);

            if (response.Error)
                ViewBag.Message = response.Message;
            else
                ViewBag.Status = "Apartamento atualizado com sucesso.";

            return View(view);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteMorador(int id)
        {
            if (!ModelState.IsValid)
                return View();

            await this.Morador.DeleteMorador(id);

            return RedirectToAction("IndexMoradores");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditMorador(int id)
        {
            if (!ModelState.IsValid)
            {
                MoradorViewModel view = await GetSelectAptoList(false);
                return View(view);
            }

            var morador = await this.Morador.GetMorador(id);

            if (morador is null)
            {
                ViewBag.Message = "Ocorreu um erro ao tentar buscar os dados do Morador para edição.";
                return View();
            }

            MoradorViewModel result = new MoradorViewModel
            {
                Apartamento = morador.Apartamento,
                CPF = morador.CPF,
                DataNascimento = morador.DataNascimento,
                EMail = morador.EMail,
                MoradorId = morador.MoradorId,
                Nome = morador.Nome,
                Telefone = morador.Telefone,
                Apartamentos = await GetDropDownItemsApartamentos(true)
            };

            //Selecionando o apartamento no DropDown automaticamente.
            result.SelectApartamento = result.Apartamento.ApartamentoId.ToString();

            return View(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditMorador(MoradorViewModel view)
        {
            view.Apartamentos = await GetDropDownItemsApartamentos(false);

            if (!ModelState.IsValid)
                return View(view);

            Morador mrd = new Morador
            {
                Apartamento = await this.Apartamento.GetApartamento(Convert.ToInt32(view.SelectApartamento)),
                CPF = view.CPF,
                DataNascimento = view.DataNascimento,
                EMail = view.EMail,
                MoradorId = view.MoradorId,
                Nome = view.Nome,
                Telefone = view.Telefone
            };

            var response = await this.Morador.PutMorador(mrd);

            if (response.Error)
                ViewBag.Message = response.Message;
            else
                ViewBag.Status = "Morador atualizado com sucesso.";

            return View(view);
        }
    }
}