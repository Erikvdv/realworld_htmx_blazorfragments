using Carter;
using Htmx;
using MiniValidation;
using RealworldBlazorHtmx.App.Components.Shared;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Pages;

public class RegisterRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var path = app.MapGroup("register");
        path.MapGet("/", GetRegister);
        path.MapPost("/", SubmitRegisterForm);
    }
    
    
    private static IResult GetRegister(HttpContext context)
    {
        var user = context.GetUser();
        if (user != null)
            return Results.Redirect("/");

        var fragment = RegisterFragments.RenderRegister;
        return RenderHelper.RenderMainLayout(context, fragment, "RegisterComponent - Conduit");
    }
    
    private record RegisterFormInput(string Email, string Username, string Password);

    private static async Task<IResult> SubmitRegisterForm(RegisterFormInput input,
        IConduitApiClient client, HttpContext context)
    {
        MiniValidator.TryValidate(input, out var validationErrors); //todo: fix this later

        try
        {
            var user = await client.RegisterUserAsync(new NewUser(input.Username, input.Email, input.Password));
            await AuthenticationHelper.LoginUser(context, user);
            context.Response.Htmx(h =>
            {
                h.Redirect("/");
                h.WithTrigger("UserLoggedIn");
            });
            return Results.Ok();
        }
        catch (ApiException apiException)
        {
            return RegisterFragments.RenderRegisterForm(apiException.ErrorList).ToComponentResult();
        }
    }
}