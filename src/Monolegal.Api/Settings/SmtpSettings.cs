namespace Monolegal.Api.Settings;

// Se llena con la sección "Smtp" de appsettings.json
public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Puerto { get; set; }

    // false para Mailpit en local; true para Gmail u otro servidor real
    public bool UsarTls { get; set; }

    // Vacíos cuando el servidor no pide autenticación (Mailpit)
    public string Usuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string RemitenteEmail { get; set; } = string.Empty;
    public string RemitenteNombre { get; set; } = string.Empty;
}
