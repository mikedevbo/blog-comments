namespace Bc.Web.Dto

open System.ComponentModel.DataAnnotations

//// TODO: move to resource file
type Comment() =
    
    [<Required(ErrorMessage = "Nazwa użytkownika nie może być pusta.")>]
    [<StringLength(50, ErrorMessage = "Nazwa użytkownika nie może mieć więcej niż 20 znaków.")>]
    member val UserName:string = null with get,set
    
    [<EmailAddress(ErrorMessage = "Nieprawidłowy format adresu e-mail.")>]
    member val UserEmail:string = null with get, set
    
    [<StringLength(50, ErrorMessage = "Nazwa Website nie może mieć więcej niż 30 znaków.")>]
    member val UserWebsite:string = null with get, set
    
    [<Required(ErrorMessage = "File name cannot be empty.")>]
    member val ArticleFileName:string = null with get, set
    
    [<Required(ErrorMessage = "Komentarz nie może być pusty.")>]
    [<StringLength(4000, ErrorMessage = "Komentarz nie może mieć więcej niż 4000 znaków.")>]
    member val UserComment:string = null with get, set
    
    member val UserPhone:string = null with get, set