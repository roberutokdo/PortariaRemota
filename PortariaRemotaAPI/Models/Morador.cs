using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortariaRemotaAPI.Models
{
    public class Morador
    {
        public int MoradorId { get; set; }
        public string Nome { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime DataNascimento { get; set; }
        public string Telefone { get; set; }
        public string CPF { get; set; }
        public string EMail { get; set; }
        
        public virtual Apartamento Apartamento { get; set; }
    }
}
