namespace z019.Web.Site.Components.Pages.More;

using MudBlazor;
using z019.Storage.SqlStorage;

public static class QueryableExtension
{
    internal static IQueryable<Exchange> ExchangeTableStateOrderBy(this IQueryable<Exchange> source, TableState state)
    {
        return state.SortLabel switch
        {
            "Id" => source.OrderByDirection(state.SortDirection, o => o.Id),
            "Name" => source.OrderByDirection(state.SortDirection, o => o.Name),
            "Code" => source.OrderByDirection(state.SortDirection, o => o.Code),
            "OperatingMIC" => source.OrderByDirection(state.SortDirection, o => o.OperatingMIC),
            "Country" => source.OrderByDirection(state.SortDirection, o => o.Country),
            "Currency" => source.OrderByDirection(state.SortDirection, o => o.Currency),
            "CountryISO2" => source.OrderByDirection(state.SortDirection, o => o.CountryISO2),
            "CountryISO3" => source.OrderByDirection(state.SortDirection, o => o.CountryISO3),
            _ => source,
        };
    }
}