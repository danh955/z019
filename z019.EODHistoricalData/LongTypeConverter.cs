namespace z019.EodHistoricalData;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Logging;

internal class LongTypeConverter : ITypeConverter
{
    private readonly ILogger? logger;

    public LongTypeConverter(ILogger? logger = null)
    {
        this.logger = logger;
    }

    public object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        return text switch
        {
            _ when long.TryParse(text, out long value) => value,
            _ when decimal.TryParse(text, out decimal valueDecimal) => (long)valueDecimal,
            _ => NotConverted()
        };

        long NotConverted()
        {
            if (logger != null)
            {
                string name = row.HeaderRecord == null ? string.Empty : $"Name: {row.HeaderRecord[memberMapData.Index]}, ";
                logger.LogWarning("Unable to convert to long integer.  {Name} Data: {text}, Row: {Row}, Column: {Column}\n RawRecord: {RawRecord})",
                    name, text, row.Context.Parser.Row - 1, memberMapData.Index.ToString(), row.Context.Parser.RawRecord);
            }
            return 0;
        }
    }

    public string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        return value == null ? string.Empty : value.ToString();
    }
}