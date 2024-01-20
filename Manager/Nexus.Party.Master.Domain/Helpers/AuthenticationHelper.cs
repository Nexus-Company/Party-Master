using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using Nexus.Party.Master.Dal;
using Nexus.OAuth.Libary.Models;
using Account = Nexus.Party.Master.Dal.Models.Accounts.Account;
using Nexus.Tools.Validations.Middlewares.Authentication;
using Nexus.OAuth.Libary;

namespace Nexus.Stock.Domain.Helpers;

public sealed class AuthenticationHelper : IAuthenticationContextFactory
{
    public const string AuthKey = "auth_to";
    public const string AuthorizationHeader = "Authorization";

    #region IAuthenticationContextFactory

    private Account? _acc;
    private IEnumerable<string> _scopes = Array.Empty<string>();
    private JwtSecurityToken? _token;
    public Account? Account
        => _acc;

    public IEnumerable<string> Scopes
        => _scopes;

    public JwtSecurityToken? Token
        => _token;

    void IAuthenticationContextFactory.SetThis(JwtSecurityToken token, Account? acc, IEnumerable<string> scopes)
    {
        _token = token;
        _acc = acc;
        _scopes = scopes;
    }
    #endregion

    public static async Task<AuthenticationResult> CheckAuthenticationAsync(HttpContext ctx, AuthenticationContext db, Application app)
    {
        AuthenticationResult invalid = new(false, false);
        string[] header;

        if (!ctx.Request.Headers.TryGetValue(AuthorizationHeader, out var value))
        {
            if(ctx.Request.Cookies.TryGetValue(AuthKey, out string cookie))
                header = cookie.Split(' ');
            else
                return invalid;
        }
        else
            header = value.ToString().Split(' ');

        if (header.Length < 2 ||
            string.IsNullOrWhiteSpace(header[0]) ||
            string.IsNullOrWhiteSpace(header[1]))
            return invalid;

        if (!Enum.TryParse(typeof(TokenType), header[0]!, out object? typeObj))
            return invalid;

        TokenType type = (TokenType)typeObj!;

        if (type != TokenType.Bearer)
            return invalid;

        try
        {
            JwtSecurityToken token = new(header[1]);

            if (DateTimeOffset.FromUnixTimeSeconds(token.Payload.Exp ?? 1).ToUniversalTime() < DateTime.UtcNow ||
                !app.VerifySignature(token))
                return invalid;

            _ = Guid.TryParse(token.Payload["account_id"].ToString(), out Guid accId);

            Account account = (await (from acc in db.Accounts
                                      where acc.Id == accId
                                      select acc).FirstOrDefaultAsync())!;

            string[] scopes = token.Payload["scopes"].ToString()!.Split(' ');

            ctx.RequestServices
                .GetRequiredService<IAuthenticationContextFactory>()
                .SetThis(token, account, scopes);

            return new AuthenticationResult(true, true, scopes);
        }
        catch (Exception ex)
        {
            return invalid;
        }
    }
}

public interface IAuthenticationContextFactory
{
    /// <summary>
    /// Client Authenticated Account
    /// </summary>
    public Account? Account { get; }

    /// <summary>
    /// Client Authorized Scopes
    /// </summary>
    public IEnumerable<string> Scopes { get; }

    /// <summary>
    /// JWT Authorization Token.
    /// </summary>
    public JwtSecurityToken? Token { get; }
    internal void SetThis(JwtSecurityToken? token, Account? acc, IEnumerable<string> scopes);
}