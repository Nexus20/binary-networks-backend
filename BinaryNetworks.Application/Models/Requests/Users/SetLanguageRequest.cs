using System.ComponentModel.DataAnnotations;

namespace BinaryNetworks.Application.Models.Requests.Users;

public class SetLanguageRequest
{
    [Required] public string NewLanguage { get; set; } = null!;
}