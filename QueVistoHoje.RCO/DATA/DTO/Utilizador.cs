using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace RCLAPI.DTO;

// Add profile data for application users by adding properties to the ApplicationUser class
public class Utilizador
{
    public string? Nome { get; set; }
    public string? Apelido { get; set; }
    public string? EMail { get; set; }
    public string? Password { get; set; }
    public long? NIF { get; set; }
    public string? Rua { get; set; }
    public string? Localidade1 { get; set; }
    public string? Localidade2 { get; set; }
    public string? Pais { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }
    public byte[]? Fotografia { get; set; }
    public string? UrlImagem { get; set; }
}
