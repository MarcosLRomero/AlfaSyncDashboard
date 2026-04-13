namespace AlfaSyncDashboard.Models;

public sealed class AnalysisResult
{
    public int MissingArticles { get; set; }
    public int CostDifferences { get; set; }
    public int MissingPriceCab { get; set; }
    public int MissingPrices { get; set; }
    public int PriceDifferences { get; set; }

    public override string ToString()
        => $"Art. faltantes: {MissingArticles} | Dif. costos: {CostDifferences} | Cab. faltantes: {MissingPriceCab} | Precios faltantes: {MissingPrices} | Dif. precios: {PriceDifferences}";
}
