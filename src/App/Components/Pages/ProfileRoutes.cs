using Carter;
using Htmx;
using Microsoft.AspNetCore.Http.HttpResults;
using RealworldBlazorHtmx.App.Components.Shared;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Pages;

public class ProfileRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var path = app.MapGroup("profile");
        path.MapGet("/", GetProfile);
        path.MapPost("/{profileName}/follow", FollowProfile);
        path.MapDelete("/{profileName}/follow", UnfollowProfile);
    }

    private static async Task<RazorComponentResult> GetProfile(HttpContext context, [AsParameters] ArticlesFilter filter,
        IConduitApiClient client)
    {
        var user = context.GetUser();

        var profileName = filter.Author ?? filter.Favorited;

        var profile = await client.GetProfileAsync(profileName, user?.Token);
      
        var fragment = ProfileFragments.RenderProfile((user is not null, profile, filter,
                profileName == user?.Username));
       

        return RenderHelper.RenderMainLayout(context, fragment, "Home - Conduit", user);
    }
    
    
    private static async Task<IResult> FollowProfile(HttpContext context, string profileName,
        IConduitApiClient client)
    {
        var user = context.GetUser();

        if (user is null)
        {
            context.Response.Htmx(h => h.Redirect("/login"));
            return Results.Unauthorized();
        }
        
        var profile = await client.FollowProfileAsync(profileName, user.Token);
        
        return ProfileFragments.RenderProfileFollowing(profile).ToComponentResult();
        
    }
    
    private static async Task<IResult> UnfollowProfile(HttpContext context, string profileName,
        IConduitApiClient client)
    {
        var user = context.GetUser();

        if (user is null) 
            return Results.Unauthorized();
        
        var profile = await client.UnFollowProfileAsync(profileName, user.Token);
        
        return ProfileFragments.RenderProfileFollowing(profile).ToComponentResult();
    }
}