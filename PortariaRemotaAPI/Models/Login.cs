using System.ComponentModel.DataAnnotations;

namespace PortariaRemotaAPI.Models
{
    public class Login
    {
        public int LoginId { get; set; }
        [Required]
        public string User { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Pass { get; set; }
    }
}
