using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AlfaSyncDashboard.Models;
using Microsoft.Data.SqlClient;

namespace AlfaSyncDashboard.Services;

public sealed class SyncStatisticsService
{
    private const string ClientCodeKey = "FTP_CODIGOCTA";
    private static readonly HttpClient HttpClient = new();
    private readonly AppSettings _settings;
    private string? _resolvedClientId;

    public SyncStatisticsService(AppSettings settings)
    {
        _settings = settings;
    }

    public async Task ReportStageAsync(
        TpvInfo tpv,
        string stageName,
        string fileName,
        int sequence,
        bool executedByService,
        DateTime startedAt,
        DateTime finishedAt,
        Exception? exception,
        int? updatedCount,
        int? insertedCount,
        Action<string>? appendLog = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.SyncStatistics.ApiUrl))
        {
            appendLog?.Invoke($"[SyncStatistics] No se envia a la API para {tpv.Descripcion}/{stageName}: ApiUrl vacia.");
            return;
        }

        var clientId = await ResolveClientIdAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(clientId))
        {
            appendLog?.Invoke("No se envio control de sincronizacion: no se pudo resolver Id cliente API.");
            return;
        }

        var payload = new SyncStatisticPayload
        {
            IdCliente = clientId,
            Fecha = startedAt.ToString("dd-MM-yyyy HH:mm:ss"),
            Secuencia = sequence,
            NombrePC = Environment.MachineName,
            ServidorSQL = Limit(tpv.Server, 250),
            Usuario = Limit(BuildExecutionUser(tpv, executedByService), 150),
            BaseDatos = Limit(BuildDatabaseLabel(tpv.DbName), 150),
            NroError = GetErrorNumber(exception),
            MensajeError = Limit(exception?.Message ?? string.Empty, 255),
            Proceso = Limit(BuildProcessLabel(stageName, tpv.Descripcion, exception, updatedCount, insertedCount), 250),
            FhHsFinProceso = finishedAt.ToString("dd-MM-yyyy HH:mm:ss"),
            Archivo = Limit(fileName, 200)
        };

        var payloadJson = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
        appendLog?.Invoke($"[SyncStatistics] Payload para {tpv.Descripcion}/{stageName}:{Environment.NewLine}{payloadJson}");
        Debug.WriteLine($"[SyncStatistics] Payload para {tpv.Descripcion}/{stageName}:{Environment.NewLine}{payloadJson}");

        try
        {
            using var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
            using var response = await HttpClient.PostAsync(_settings.SyncStatistics.ApiUrl.Trim(), content, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                appendLog?.Invoke($"[SyncStatistics] Envio OK para {tpv.Descripcion}/{stageName}. HTTP {(int)response.StatusCode}.");
                return;
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            appendLog?.Invoke($"No se pudo enviar control de sincronizacion para {tpv.Descripcion}/{stageName}. HTTP {(int)response.StatusCode}: {body}");
        }
        catch (Exception ex)
        {
            appendLog?.Invoke($"No se pudo enviar control de sincronizacion para {tpv.Descripcion}/{stageName}: {ex.Message}");
        }
    }

    public async Task<string> ResolveClientIdAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(_resolvedClientId))
            return _resolvedClientId;

        try
        {
            await using var cn = new SqlConnection(_settings.CentralConnectionString);
            await cn.OpenAsync(cancellationToken);
            await using var cmd = new SqlCommand(@"
SELECT TOP 1 VALOR
FROM NW_ESTADISTICAS.dbo.TA_CONFIGURACION
WHERE CLAVE = @Clave;", cn)
            {
                CommandTimeout = _settings.CommandTimeoutSeconds
            };
            cmd.Parameters.AddWithValue("@Clave", ClientCodeKey);

            var value = Convert.ToString(await cmd.ExecuteScalarAsync(cancellationToken))?.Trim();
            if (!string.IsNullOrWhiteSpace(value))
            {
                _resolvedClientId = value;
                _settings.SyncStatistics.IdCliente = value;
                return value;
            }
        }
        catch
        {
        }

        _resolvedClientId = _settings.SyncStatistics.IdCliente.Trim();
        return _resolvedClientId;
    }

    private string BuildDatabaseLabel(string localDatabase)
    {
        var centralDatabase = "CENTRAL";

        try
        {
            var builder = new SqlConnectionStringBuilder(_settings.CentralConnectionString);
            if (!string.IsNullOrWhiteSpace(builder.InitialCatalog))
                centralDatabase = builder.InitialCatalog;
        }
        catch
        {
        }

        return $"{centralDatabase} + {localDatabase}";
    }

    private static string BuildExecutionUser(TpvInfo tpv, bool executedByService)
    {
        var dbUser = string.IsNullOrWhiteSpace(tpv.Usuario) ? "dbuser" : tpv.Usuario.Trim();
        var origin = executedByService ? "Servicio" : "Manual";
        return $"{dbUser} {origin}";
    }

    private static string BuildProcessLabel(string stageName, string localDescription, Exception? exception, int? updatedCount, int? insertedCount)
    {
        var process = $"Sincronizacion de {stageName} - {localDescription}";
        if (exception is not null || updatedCount is null || insertedCount is null)
            return process;

        return $"{process} - {updatedCount.Value} actualizados, {insertedCount.Value} altas";
    }

    private static int GetErrorNumber(Exception? exception)
    {
        if (exception is null)
            return 0;

        if (exception is SqlException sqlException)
            return sqlException.Number;

        if (exception is OperationCanceledException)
            return -1;

        return exception.HResult != 0 ? exception.HResult : 1;
    }

    private static string Limit(string value, int maxLength)
    {
        return value.Length <= maxLength
            ? value
            : value[..maxLength];
    }
}
