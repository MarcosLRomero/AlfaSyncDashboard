namespace AlfaSyncDashboard.Models;

public sealed class TpvInfo
{
    public bool Selected { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Sucursal { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;
    public string DbName { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string EstadoConexion { get; set; } = "Pendiente";
    public string UltimaSincronizacion { get; set; } = "-";
    public string EstadoActual { get; set; } = "Listo";
    public string ScriptSet { get; set; } = "DEFAULT";

    public string BuildLocalConnectionString()
        => $"Server={Server};Database={DbName};User Id={Usuario};Password={Password};TrustServerCertificate=True;Encrypt=False;";
}
