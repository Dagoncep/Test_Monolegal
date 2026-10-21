namespace Monolegal.Api.Settings;

// Se llena con la sección "Mongo" de appsettings.json
public class MongoSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}
