namespace RealworldBlazorHtmx.App.ServiceClient;

public interface IConduitApiClient
{
    Task<ArticleList> GetArticleListAsync(ArticlesQuery articlesQuery, string? token,
        CancellationToken cancellationToken = default);

    Task<ArticleList> GetArticleFeedAsync(ArticlesQuery articleListFilter, string token,
        CancellationToken cancellationToken = default);

    Task<Article> GetArticleAsync(string slug, string? token, CancellationToken cancellationToken = default);
    Task<Article> CreateArticleAsync(NewArticle article, string token, CancellationToken cancellationToken = default);
    Task<Article> UpdateArticleAsync(string slug, UpdateArticle article, string token, CancellationToken cancellationToken = default);
    Task DeleteArticleAsync(string slug, string? token, CancellationToken cancellationToken = default);
    
    Task<Article> FavoriteArticleAsync(string slug, string token, CancellationToken cancellationToken = default);
    Task<Article> UnfavoriteArticleAsync(string slug, string token, CancellationToken cancellationToken = default);

    Task<List<Comment>> GetArticleCommentsAsync(string slug, string? token,
        CancellationToken cancellationToken = default);
    
    Task<Comment> AddCommentAsync(string slug, string comment, string token,
        CancellationToken cancellationToken = default);
    Task DeleteCommentAsync(string slug, int commentId, string token,
        CancellationToken cancellationToken = default);

    Task<string[]> GetTagListAsync(CancellationToken cancellationToken = default);

    Task<Profile> GetProfileAsync(string username, string? token, CancellationToken cancellationToken = default);
    Task<Profile> FollowProfileAsync(string username, string token, CancellationToken cancellationToken = default);
    Task<Profile> UnFollowProfileAsync(string username, string token, CancellationToken cancellationToken = default);

    Task<User> LoginAsync(Login login, CancellationToken cancellationToken = default);
    Task<User> LoginWithTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<User> GetUserAsync(string token, CancellationToken cancellationToken = default);
    Task<User> UpdateUserAsync(User user, string token, CancellationToken cancellationToken = default);

    Task<User> RegisterUserAsync(NewUser user, CancellationToken cancellationToken = default);
}