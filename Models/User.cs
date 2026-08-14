using System.ComponentModel.DataAnnotations;

namespace DocumentationDemo.Models;

public sealed class User
{
    public int Id { get; set; }

    public required string Nome { get; set; }

    public required string Sobrenome { get; set; }
}

public sealed class UserInput
{
    [Required]
    public required string Nome { get; set; }

    [Required]
    public required string Sobrenome { get; set; }
}
