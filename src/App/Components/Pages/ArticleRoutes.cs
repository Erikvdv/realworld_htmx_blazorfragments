using Carter;
using Htmx;
using Microsoft.AspNetCore.Http.HttpResults;
using RealworldBlazorHtmx.App.Components.Shared;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Pages;

public class ArticleRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var path = app.MapGroup("article");
        path.MapGet("/{slug}", GetArticle);
        path.MapDelete("/{slug}", DeleteArticle);
        path.MapPost("/{slug}/favorite", FavoriteArticle);
        path.MapDelete("/{slug}/favorite", UnFavoriteArticle);
        path.MapGet("/{slug}/comments", GetComments);
        path.MapPost("/{slug}/comments", AddComment);
        path.MapDelete("/{slug}/comments/{commentId}", DeleteComment);
        path.MapPost("/followauthor/{authorUsername}", FollowAuthor);
        path.MapDelete("/followauthor/{authorUsername}", UnfollowAuthor);
    }
    

    private static async Task<RazorComponentResult> GetArticle(HttpContext context, string slug,
        IConduitApiClient client)
    {
        
        var user = context.GetUser();

        var article = await client.GetArticleAsync(slug, user?.Token);
        var bodyFragment = ArticleFragments.RenderArticle((user != null, article, user));
        
        return RenderHelper.RenderMainLayout(context, bodyFragment, "Home - Conduit", user);
    }
    
    private static async Task<IResult> DeleteArticle(HttpContext context, string slug,
        IConduitApiClient client)
    {
        var user = context.GetUser();

        await client.DeleteArticleAsync(slug, user?.Token);

        context.Response.Htmx(h =>
        {
            h.Redirect("/");
        });
        return Results.Ok();
    }
    
    private static async Task<IResult> FavoriteArticle(HttpContext context, string slug, IConduitApiClient client)
    {
        var user = context.GetUser();
        if (user == null)
        {
            context.Response.Htmx(h =>
            {
                h.Redirect("/login");
            });
            return Results.Unauthorized();
        }
        
        var article = await client.FavoriteArticleAsync(slug, user.Token);
        return ArticleFragments.RenderArticle((true, article, user)).ToComponentResult();
        
    }
    
    private static async Task<IResult> UnFavoriteArticle(HttpContext context, string slug, IConduitApiClient client)
    {
        var user = context.GetUser();

        if (user == null) return Results.Forbid();
        var article = await client.UnfavoriteArticleAsync(slug, user.Token);

        return ArticleFragments.RenderArticle((true, article, user)).ToComponentResult();

    }


    
    private static async Task<IResult> GetComments(HttpContext context, string slug, IConduitApiClient client)
    {
        var user = context.GetUser();
        if (user == null) return Results.Forbid();
        
        var comments = await client.GetArticleCommentsAsync(slug, user.Token);

        return ArticleFragments.RenderComments((slug, comments, user)).ToComponentResult();
    }
    
    public record NewComment(string Comment);
    private static async Task<IResult> AddComment(NewComment newComment, HttpContext context, string slug,
        IConduitApiClient client)
    {
        var user = context.GetUser();
        if (user == null) return Results.Forbid();
        
        var comment = await client.AddCommentAsync(slug, newComment.Comment, user.Token);
        var comments = await client.GetArticleCommentsAsync(slug, user.Token);
        return ArticleFragments.RenderComments((slug, comments, user)).ToComponentResult();
    }
    
    private static async Task<IResult> DeleteComment(int commentId, HttpContext context, string slug,
        IConduitApiClient client)
    {
        var user = context.GetUser();
        if (user == null) return Results.Forbid();
        
        await client.DeleteCommentAsync(slug, commentId, user.Token);
        var comments = await client.GetArticleCommentsAsync(slug, user.Token);

        return ArticleFragments.RenderComments((slug, comments, user)).ToComponentResult();
    }
    
    private static async Task<IResult> FollowAuthor(HttpContext context, string authorUsername, IConduitApiClient client)
    {
        var user = context.GetUser();
        if (user == null)
        {
            context.Response.Htmx(h =>
            {
                h.Redirect("/login");
            });
            return Results.Unauthorized();
        }
        
        var profile = await client.FollowProfileAsync(authorUsername, user.Token);
        return ArticleFragments.RenderArticleAuthor(profile).ToComponentResult();
    }
    
    private static async Task<IResult> UnfollowAuthor(HttpContext context, string authorUsername, IConduitApiClient client)
    {
        var user = context.GetUser();
        if (user == null)
        {
            return Results.Unauthorized();
        }
        
        var profile = await client.UnFollowProfileAsync(authorUsername, user.Token);
        return ArticleFragments.RenderArticleAuthor(profile).ToComponentResult();
    }
}