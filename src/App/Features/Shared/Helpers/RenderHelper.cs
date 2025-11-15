using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Features.Shared.Helpers;

public static class RenderHelper
{
    public static RazorComponentResult RenderMainLayout(HttpContext context, RenderFragment bodyFragment,
        string pageTitle, User? user = null)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        var layoutFragment =
            AppFragments.RenderMainLayout(bodyFragment, isAuthenticated, context.Request.Path, user);

        return AppFragments.RenderApp(layoutFragment, pageTitle).ToComponentResult();
    }

    public static RazorComponentResult ToComponentResult(this RenderFragment fragment)
    {
        return new RazorComponentResult<FragmentComponent>(new {Fragment = fragment});
    }
}