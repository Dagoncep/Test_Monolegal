namespace Monolegal.Api.Models;

public class Cliente
{
    // NIT o cédula del cliente; es el _id del documento en MongoDB
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
