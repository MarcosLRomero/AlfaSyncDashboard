using AlfaSyncDashboard.Models;
using Microsoft.Data.SqlClient;

namespace AlfaSyncDashboard.Services;

public sealed class AnalysisService
{
    private readonly AppSettings _settings;

    public AnalysisService(AppSettings settings)
    {
        _settings = settings;
    }

    public async Task<AnalysisResult> AnalyzeAsync(TpvInfo tpv, CancellationToken cancellationToken = default)
    {
        var centralArticles = await LoadArticleCostsAsync(_settings.CentralConnectionString, cancellationToken);
        var localArticles = await LoadArticleCostsAsync(tpv.BuildLocalConnectionString(), cancellationToken);

        var centralPriceCab = await LoadPriceCabKeysAsync(_settings.CentralConnectionString, cancellationToken);
        var localPriceCab = await LoadPriceCabKeysAsync(tpv.BuildLocalConnectionString(), cancellationToken);

        var centralPrices = await LoadPricesAsync(_settings.CentralConnectionString, cancellationToken);
        var localPrices = await LoadPricesAsync(tpv.BuildLocalConnectionString(), cancellationToken);

        var result = new AnalysisResult
        {
            MissingArticles = centralArticles.Keys.Count(k => !localArticles.ContainsKey(k)),
            CostDifferences = centralArticles.Count(kvp => localArticles.TryGetValue(kvp.Key, out var local) && local != kvp.Value),
            MissingPriceCab = centralPriceCab.Count(k => !localPriceCab.Contains(k)),
            MissingPrices = centralPrices.Keys.Count(k => !localPrices.ContainsKey(k)),
            PriceDifferences = centralPrices.Count(kvp => localPrices.TryGetValue(kvp.Key, out var local) && local != kvp.Value),
        };

        return result;
    }

    public async Task<bool> TestLocalConnectionAsync(TpvInfo tpv, CancellationToken cancellationToken = default)
    {
        await using var cn = new SqlConnection(tpv.BuildLocalConnectionString());
        await cn.OpenAsync(cancellationToken);
        return cn.State == System.Data.ConnectionState.Open;
    }

    private static async Task<Dictionary<string, decimal>> LoadArticleCostsAsync(string connectionString, CancellationToken ct)
    {
        const string sql = "SELECT IDARTICULO, CAST(COSTO AS decimal(18,4)) AS COSTO FROM dbo.V_MA_ARTICULOS;";
        var dict = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        await using var cn = new SqlConnection(connectionString);
        await cn.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, cn) { CommandTimeout = 0 };
        await using var dr = await cmd.ExecuteReaderAsync(ct);
        while (await dr.ReadAsync(ct))
        {
            var key = dr[0]?.ToString()?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(key))
                dict[key] = dr.IsDBNull(1) ? 0m : dr.GetDecimal(1);
        }
        return dict;
    }

    private static async Task<HashSet<string>> LoadPriceCabKeysAsync(string connectionString, CancellationToken ct)
    {
        const string sql = "SELECT IdLista, TipoLista FROM dbo.V_MA_PRECIOSCAB;";
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        await using var cn = new SqlConnection(connectionString);
        await cn.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, cn) { CommandTimeout = 0 };
        await using var dr = await cmd.ExecuteReaderAsync(ct);
        while (await dr.ReadAsync(ct))
        {
            var key = $"{dr[0]?.ToString()?.Trim()}|{dr[1]?.ToString()?.Trim()}";
            set.Add(key);
        }
        return set;
    }

    private static async Task<Dictionary<string, string>> LoadPricesAsync(string connectionString, CancellationToken ct)
    {
        const string sql = @"
SELECT IdLista, IdArticulo, TipoLista,
       CAST(ISNULL(COSTO,0) AS varchar(50)) + '|' +
       CAST(ISNULL(Precio1,0) AS varchar(50)) + '|' +
       CAST(ISNULL(Precio2,0) AS varchar(50)) + '|' +
       CAST(ISNULL(Precio3,0) AS varchar(50)) + '|' +
       CAST(ISNULL(Precio4,0) AS varchar(50)) + '|' +
       CAST(ISNULL(Precio5,0) AS varchar(50)) AS Firma
FROM dbo.V_MA_PRECIOS;";
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        await using var cn = new SqlConnection(connectionString);
        await cn.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, cn) { CommandTimeout = 0 };
        await using var dr = await cmd.ExecuteReaderAsync(ct);
        while (await dr.ReadAsync(ct))
        {
            var key = $"{dr[0]?.ToString()?.Trim()}|{dr[1]?.ToString()?.Trim()}|{dr[2]?.ToString()?.Trim()}";
            var signature = dr[3]?.ToString() ?? string.Empty;
            dict[key] = signature;
        }
        return dict;
    }
}
