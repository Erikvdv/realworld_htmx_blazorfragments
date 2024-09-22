using Carter;
using Htmx;
using Microsoft.AspNetCore.Http.HttpResults;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Shared;

public class ArticlesRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var path = app.MapGroup("articles");
        path.MapGet("/list", GetArticleList);
    }

    private static async Task<RazorComponentResult> GetArticleList(HttpContext context,
        IConduitApiClient client, [AsParameters] ArticlesFilter filter)
    {
        var user = context.GetUser();
        try
        {
            var articles = filter.MyFeed == true && user is not null
                ? await client.GetArticleFeedAsync(
                    new ArticlesQuery(filter.Tag, filter.Author, filter.Favorited, 10, (filter.Page - 1) * 10), user.Token
                )
                : await client.GetArticleListAsync(
                    new ArticlesQuery(
                        filter.Tag, filter.Author, filter.Favorited,
                        (filter.Author is not null || filter.Favorited is not null) ? 5 : 10,
                        (filter.Page - 1) * ((filter.Author is not null || filter.Favorited is not null) ? 5 : 10)
                    ), user?.Token
                );
        
            context.Response.Htmx(h => { h.ReplaceUrl(filter.ToQueryString()); });
            return ArticlesFragments.RenderArticleList((articles, filter)).ToComponentResult();
        } catch (Exception e)
        {
            return SharedFragments.Render500Error.ToComponentResult();
        }
        
    }
}