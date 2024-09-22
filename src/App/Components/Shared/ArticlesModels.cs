namespace RealworldBlazorHtmx.App.Components.Shared;

public record ArticlesFilter(string? Tag, string? Author, string? Favorited, bool? MyFeed, int Page = 1)
{
    public string ToQueryString()
    {
        var parameters = new Dictionary<string, string?>
        {
            {"page", Page.ToString()},
            {"tag", Tag},
            {"author", Author},
            {"favorited", Favorited},
            {"myfeed", MyFeed.HasValue ? MyFeed.ToString() : null}
        };

        return "?" + string.Join("&",
            parameters.Where(p => !string.IsNullOrEmpty(p.Value)).Select(p => $"{p.Key}={p.Value}"));
    }
}