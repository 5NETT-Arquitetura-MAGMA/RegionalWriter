using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Contacts")]
    public class Contact : EntityBase
    {
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Range(100000000, 999999999, ErrorMessage = "O telefone deve ter 9 digitos.")]
        public int Telefone { get; set; }

        [Required(ErrorMessage = "O DDD é obrigatório.")]
        [Range(01, 99, ErrorMessage = "O DDD deve ter 2 digitos.")]
        public int DDD { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O estado é obrigatório.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "O Estado deve conter exatos 2 caracteres. Exemplo: SP")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "A cidade é obrigatório.")]
        public string Cidade { get; set; }
    }
}