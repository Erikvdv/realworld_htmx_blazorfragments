using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Options;

namespace RealworldBlazorHtmx.App.ServiceClient;

public class ConduitApiClient : IConduitApiClient
{
    private readonly HttpClient _httpClient;

    private readonly ILogger<ConduitApiClient> _logger;
    private readonly ConduitClientSettings _settings;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new() {PropertyNameCaseInsensitive = true};

    public ConduitApiClient(
        HttpClient httpClient,
        IOptions<ConduitClientSettings> settings,
        ILogger<ConduitApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };
        _httpClient.BaseAddress = new Uri(_settings.BaseAddress);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public Task<ArticleList> GetArticleListAsync(ArticlesQuery articlesQuery, string? token,
        CancellationToken cancellationToken = default)
    {
        var querystring = GetQueryString(articlesQuery);
        var httpRequest =
            new HttpRequestMessage(HttpMethod.Get, new Uri($"api/articles?{querystring}", UriKind.Relative));
        httpRequest.Headers.Add("Accept", "application/json");
        if (token != null) httpRequest.Headers.Add("Authorization", $"Token {token}");

        return HandleRequest<ArticleList>(httpRequest, cancellationToken);
    }

    public Task<ArticleList> GetArticleFeedAsync(ArticlesQuery articlesQuery, string token,
        CancellationToken cancellationToken = default)
    {
        var querystring = GetQueryString(articlesQuery);
        var httpRequest =
            new HttpRequestMessage(HttpMethod.Get, new Uri($"api/articles/feed?{querystring}", UriKind.Relative));
        httpRequest.Headers
            .Add("Authorization", $"Token {token}");


        return HandleRequest<ArticleList>(httpRequest, cancellationToken);
    }

    public async Task<Article> GetArticleAsync(string slug, string? token,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri($"api/articles/{slug}", UriKind.Relative));
        if (token != null) httpRequest.Headers.Add("Authorization", $"Token {token}");

        var response = await HandleRequest<ArticleResponse>(httpRequest, cancellationToken);
        return response.Article;
    }

    public async Task<Article> CreateArticleAsync(NewArticle article, string token,
        CancellationToken cancellationToken = default)
    {
        var body = new NewArticleRequest {Article = article};
        var requestBody = JsonSerializer.Serialize(body, _jsonSerializerOptions);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri("api/articles", UriKind.Relative))
        {
            Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
        };
        httpRequest.Headers.Add("Authorization", $"Token {token}");

        var response = await HandleRequest<ArticleResponse>(httpRequest, cancellationToken);
        return response.Article;
    }

    public async Task<Article> UpdateArticleAsync(string slug, UpdateArticle article, string token,
        CancellationToken cancellationToken = default)
    {
        var body = new UpdateArticleResponse {Article = article};
        var requestBody = JsonSerializer.Serialize(body, _jsonSerializerOptions);
        var httpRequest =
            new HttpRequestMessage(HttpMethod.Put, new Uri($"api/articles/{slug}", UriKind.Relative))
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };
        httpRequest.Headers.Add("Authorization", $"Token {token}");

        var response = await HandleRequest<ArticleResponse>(httpRequest, cancellationToken);
        return response.Article;
    }

    public async Task DeleteArticleAsync(string slug, string? token, CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, new Uri($"api/articles/{slug}", UriKind.Relative));
        httpRequest.Headers.Add("Authorization", $"Token {token}");
        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
    }

    public async Task<Article> FavoriteArticleAsync(string slug, string token, CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri($"api/articles/{slug}/favorite", UriKind.Relative));
        httpRequest.Headers.Add("Authorization", $"Token {token}");

        var response = await HandleRequest<ArticleResponse>(httpRequest, cancellationToken);
        return response.Article;
    }

    public async Task<Article> UnfavoriteArticleAsync(string slug, string token, CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, new Uri($"api/articles/{slug}/favorite", UriKind.Relative));
        httpRequest.Headers.Add("Authorization", $"Token {token}");

        var response = await HandleRequest<ArticleResponse>(httpRequest, cancellationToken);
        return response.Article;
    }

    public async Task<List<Comment>> GetArticleCommentsAsync(string slug, string? token,
        CancellationToken cancellationToken = default)
    {
        var httpRequest =
            new HttpRequestMessage(HttpMethod.Get, new Uri($"api/articles/{slug}/comments", UriKind.Relative));
        if (token != null) httpRequest.Headers.Add("Authorization", $"Token {token}");

        var response = await HandleRequest<CommentsResponse>(httpRequest, cancellationToken);
        return response.Comments;
    }

    public async Task<Comment> AddCommentAsync(string slug, string comment, string token, CancellationToken cancellationToken = default)
    {

        var newCommentRequest = new NewCommentRequest(new NewComment(comment));
        var requestBody = JsonSerializer.Serialize(newCommentRequest, _jsonSerializerOptions);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri($"api/articles/{slug}/comments", UriKind.Relative))
        {
            Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
        };
        httpRequest.Headers.Add("Authorization", $"Token {token}");

        var response = await HandleRequest<CommentResponse>(httpRequest, cancellationToken);
        return response.Comment;
    }

    public async Task DeleteCommentAsync(string slug, int commentId, string token, CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, new Uri($"api/articles/{slug}/comments/{commentId}", UriKind.Relative));
        httpRequest.Headers.Add("Authorization", $"Token {token}");
        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
    }

    public async Task<string[]> GetTagListAsync(CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("api/tags", UriKind.Relative));
        
        var response = await HandleRequest<TagsResponse>(httpRequest, cancellationToken);
        return response.Tags;
    }

    public async Task<Profile> GetProfileAsync(string username, string? token,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri($"api/profiles/{username}", UriKind.Relative));
        if (token != null) httpRequest.Headers.Add("Authorization", $"Token {token}");

        var response = await HandleRequest<ProfileResponse>(httpRequest, cancellationToken);
        return response.Profile;
    }

    public async Task<User> GetUserAsync(string token, CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("api/user", UriKind.Relative));
        httpRequest.Headers.Add("Authorization", $"Token {token}");


        var response = await HandleRequest<UserResponse>(httpRequest, cancellationToken);
        return response.User;
    }

    public async Task<User> UpdateUserAsync(User user, string token, CancellationToken cancellationToken = default)
    {
        var updateUserRequest = new UserUpdateRequest {User = user};
        var requestBody = JsonSerializer.Serialize(updateUserRequest, _jsonSerializerOptions);
        var httpRequest = new HttpRequestMessage(HttpMethod.Put, new Uri("api/user", UriKind.Relative))
        {
            Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
        };

        httpRequest.Headers.Add("Authorization", $"Token {token}");


        var response = await HandleRequest<UserResponse>(httpRequest, cancellationToken);
        return response.User;
    }

    public async Task<User> RegisterUserAsync(NewUser user, CancellationToken cancellationToken = default)
    {
        var newUserRequest = new NewUserRequest(user);
        var requestBody = JsonSerializer.Serialize(newUserRequest, _jsonSerializerOptions);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri("api/users", UriKind.Relative))
        {
            Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
        };

        var response = await HandleRequest<UserResponse>(httpRequest, cancellationToken);
        return response.User;
    }

    public async Task<Profile> FollowProfileAsync(string username, string token,
        CancellationToken cancellationToken = default)
    {
        var httpRequest =
            new HttpRequestMessage(HttpMethod.Post, new Uri($"api/profiles/{username}/follow", UriKind.Relative));

        httpRequest.Headers.Add("Authorization", $"Token {token}");

        var response = await HandleRequest<ProfileResponse>(httpRequest, cancellationToken);
        return response.Profile;
    }

    public async Task<Profile> UnFollowProfileAsync(string username, string token,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete,
            new Uri($"api/profiles/{username}/follow", UriKind.Relative));

        httpRequest.Headers.Add("Authorization", $"Token {token}");

        var response = await HandleRequest<ProfileResponse>(httpRequest, cancellationToken);
        return response.Profile;
    }

    public async Task<User> LoginAsync(Login login, CancellationToken cancellationToken = default)
    {
        var loginRequest = new LoginRequest {User = login};
        var requestBody = JsonSerializer.Serialize(loginRequest, _jsonSerializerOptions);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri("api/users/login", UriKind.Relative))
        {
            Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
        };
        var response = await HandleRequest<LoginResponse>(httpRequest, cancellationToken);
        return response.User;
    }

    public async Task<User> LoginWithTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("api/user", UriKind.Relative));
        httpRequest.Headers.Add("authorization", $"Token {token}");
        var response = await HandleRequest<LoginResponse>(httpRequest, cancellationToken);
        return response.User;
    }

    private async Task<T> HandleRequest<T>(HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseBody = await GetResponseBody(response);

        if (response.IsSuccessStatusCode)
            return JsonSerializer.Deserialize<T>(responseBody, _jsonSerializerOptions) ?? throw new InvalidOperationException();

        _logger.LogError("Error executing request to \'{RequestRequestUri}\', HTTP {ResponseStatusCode}: {ResponseBody}", request.RequestUri, (int) response.StatusCode, responseBody);
        if (response.StatusCode == HttpStatusCode.UnprocessableEntity ||
            response.StatusCode == HttpStatusCode.Forbidden)
        {
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, _jsonSerializerOptions);

            throw new ApiException(errorResponse.Errors);
        }

        throw new Exception(
            $"Error executing request to '{request.RequestUri}', HTTP {(int) response.StatusCode}: {responseBody}"
        );
    }

    private static async Task<string> GetResponseBody(HttpResponseMessage response)
    {
        return await response.Content.ReadAsStringAsync();
    }

    private static string GetQueryString(object obj)
    {
        var properties = from p in obj.GetType().GetProperties()
            where p.GetValue(obj, null) != null
            select p.Name.ToLower() + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

        return string.Join("&", properties.ToArray());
    }
}