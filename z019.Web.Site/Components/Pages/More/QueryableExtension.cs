namespace z019.Web.Site.Components.Pages.More;

using System.Linq;
using MudBlazor;
using System.Linq.Expressions;
using z019.Storage.SqlStorage;

public static class QueryableExtension
{
    internal static IQueryable<Exchange> OrderBy(this IQueryable<Exchange> source, GridState<Exchange> state)
    {
        foreach (var item in state.SortDefinitions)
        {
            source = item.SortBy switch
            {
                "Id" => source.OrderBy(item.Descending, o => o.Id),
                "Name" => source.OrderBy(item.Descending, o => o.Name),
                "Code" => source.OrderBy(item.Descending, o => o.Code),
                "OperatingMIC" => source.OrderBy(item.Descending, o => o.OperatingMIC),
                "Country" => source.OrderBy(item.Descending, o => o.Country),
                "Currency" => source.OrderBy(item.Descending, o => o.Currency),
                "CountryISO2" => source.OrderBy(item.Descending, o => o.CountryISO2),
                "CountryISO3" => source.OrderBy(item.Descending, o => o.CountryISO3),
                _ => source
            };
        }

        return source;
    }

    private static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source, bool descending, Expression<Func<TSource, TKey>> keySelector)
    {
        return descending
            ? source.OrderByDescending(keySelector)
            : source.OrderBy(keySelector);
    }
}