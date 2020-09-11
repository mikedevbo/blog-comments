using System.ComponentModel.DataAnnotations;

namespace Bc.Web.Models
{
    //// TODO: move to resource file
    public class Comment
    {
        [Required(ErrorMessage = "Nazwa użytkownika nie może być pusta.")]
        [StringLength(50, ErrorMessage = "Nazwa użytkownika nie może mieć więcej niż 20 znaków.")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu e-mail.")]
        public string UserEmail { get; set; }

        [StringLength(50, ErrorMessage = "Nazwa Website nie może mieć więcej niż 30 znaków.")]
        public string UserWebsite { get; set; }

        [Required(ErrorMessage = "File name cannot be empty.")]
        public string ArticleFileName { get; set; }

        [Required(ErrorMessage = "Komentarz nie może być pusty.")]
        [StringLength(4000, ErrorMessage = "Komentarz nie może mieć więcej niż 4000 znaków.")]
        public string UserComment { get; set; }

        public string UserPhone { get; set; }
    }
}