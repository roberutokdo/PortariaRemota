using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;
using WebPortariaRemota.Models.WebApiContext;

namespace WebPortariaRemota.Models
{
    [Serializable]
    public class MoradorViewModel
    {
        public int MoradorId { get; set; }
        [Required]
        [Display(Name = "Nome")]
        public string Nome { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }
        [Required]
        [Display(Name = "Telefone")]
        [DataType(DataType.PhoneNumber)]
        public string Telefone { get; set; }
        [Required]
        [Display(Name = "CPF")]
        public string CPF { get; set; }
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string EMail { get; set; }

        public SelectList Apartamentos { get; set; }
        public string SelectApartamento { get; set; }

        public virtual Apartamento Apartamento { get; set; }
    }
}
