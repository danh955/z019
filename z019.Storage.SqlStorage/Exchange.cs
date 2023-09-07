namespace z019.Storage.SqlStorage;

public class Exchange
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string OperatingMIC { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string CountryISO2 { get; set; } = string.Empty;
    public string CountryISO3 { get; set; } = string.Empty;
}