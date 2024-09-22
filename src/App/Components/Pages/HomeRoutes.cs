using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using RealworldBlazorHtmx.App.Components.Shared;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Pages;

public class HomeRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", GetHome);
        app.MapGet("/home/articleslist", GetHomeArticlesList);
        app.MapGet("/home/tags", GetTags);
    }

    private static RazorComponentResult GetHome(HttpContext context, [AsParameters] ArticlesFilter filter)
    {
        var user = context.GetUser();

        if (user is not null && filter.MyFeed is null && filter.Tag is null)
        {
            filter = filter with { MyFeed = true };
        }
        
        var fragment = HomeFragments.RenderHome((user != null, filter));
        
        return RenderHelper.RenderMainLayout(context, fragment, "Home - Conduit", user);
    }

    private static async Task<RazorComponentResult> GetHomeArticlesList(HttpContext context,
        IConduitApiClient client, [AsParameters] ArticlesFilter filter)
    {
        var articles = await client.GetArticleListAsync(new ArticlesQuery(null, null, null), null);

        return ArticlesFragments.RenderArticleList((articles, filter)).ToComponentResult();
    }

    private static async Task<RazorComponentResult> GetTags(HttpContext context, IConduitApiClient client)
    {
        var tags = await client.GetTagListAsync();
        context.Response.Headers.CacheControl = $"max-age={TimeSpan.FromSeconds(60).TotalSeconds}";
        return HomeFragments.RenderTags(tags.ToList()).ToComponentResult();
    }
}