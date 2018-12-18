using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebPortariaRemota.Models.WebApiContext;

namespace WebPortariaRemota.Models
{
    [Serializable]
    public class ApartamentoViewModel
    {
        public int ApartamentoId { get; set; }
        [Required]
        [Display(Name = "Bloco")]
        public string Bloco { get; set; }
        [Required]
        [Display(Name = "Número")]
        public int Numero { get; set; }

        public virtual IList<Morador> Moradores { get; set; }
    }
}
