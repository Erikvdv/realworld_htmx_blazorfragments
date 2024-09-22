using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace RealworldBlazorHtmx.App.ServiceClient;

public record ArticlesQuery(string? Tag, string? Author, string? Favorited, int Limit = 20, int Offset = 0);

public enum FeedType
{
    Global,
    Private
}

public class Login
{
    [Required] public required string Email { get; set; }

    [Required] public required string Password { get; set; }
}

public class User
{
    public required string Bio { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Email { get; set; }
    public int Id { get; set; }
    public required string Image { get; set; }
    public string? Password { get; set; }
    public required string Token { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required string Username { get; set; }
}

public record NewUser(string Username, string Email, string Password);

public class Profile
{
    public required string Username { get; set; }
    public string? Bio { get; set; }
    public required string Image { get; set; }
    public bool Following { get; set; }
}

public class NewArticle
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Body { get; set; } = "";
    public HashSet<string> TagList { get; set; } = [];
}

public class Comment
{
    public int Id { get; set; }

    public string Body { get; set; } = "";

    public required Profile Author { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

public class ArticleListFilter : ICloneable
{
    public FeedType FeedType { get; set; }
    public string? Tag { get; set; }
    public string? Author { get; set; }
    public string? Favorited { get; set; }
    public int Limit { get; set; } = 10;
    public int Offset { get; set; }

    public object Clone()
    {
        var serialized = JsonSerializer.Serialize(this);
        return JsonSerializer.Deserialize<ArticleListFilter>(serialized) ?? throw new InvalidOperationException();
    }
}

public class ArticleList
{
    public List<Article> Articles { get; set; } = [];

    public int ArticlesCount { get; set; }
}

public class Article
{
    public required string Slug { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public string Body { get; set; } = "";

    public required string[] TagList { get; set; }

    public bool Favorited { get; set; }
    public int FavoritesCount { get; set; }
    public required Profile Author { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

public record UpdateArticle(string Title, string Description, string Body, List<string> TagList);

public class TagsResponse
{
    public required string[] Tags { get; set; }
}

public class LoginRequest
{
    public Login? User { get; set; }
}

public class LoginResponse
{
    public required User User { get; set; }
}

public class ArticleResponse
{
    public required Article Article { get; set; }
}

public class UpdateArticleResponse
{
    public required UpdateArticle Article { get; set; }
}

public class NewArticleRequest
{
    public NewArticle? Article { get; set; }
}

public class CommentResponse
{
    public required Comment Comment { get; set; }
}

public class CommentsResponse
{
    public required List<Comment> Comments { get; set; }
}

public class ProfileResponse
{
    public required Profile Profile { get; set; }
}

public class UserResponse
{
    public required User User { get; set; }
}

public class UserUpdateRequest
{
    public User? User { get; set; }
}

public record NewUserRequest(NewUser User);

public record NewComment(string Body);
public record NewCommentRequest(NewComment Comment);

public class ErrorResponse
{
    public required Dictionary<string, string[]> Errors { get; set; }
}