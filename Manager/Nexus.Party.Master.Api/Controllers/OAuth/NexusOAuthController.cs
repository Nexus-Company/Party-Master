using Microsoft.EntityFrameworkCore;
using Nexus.OAuth.Libary;
using Nexus.OAuth.Libary.Models;
using Nexus.Party.Master.Dal;
using Nexus.Party.Master.Dal.Models.Accounts;
using Nexus.Stock.Domain.Helpers;
using System.Net;
using Account = Nexus.Party.Master.Dal.Models.Accounts.Account;

namespace Nexus.Stock.Api.Controllers;

[AllowAnonymous]
[ApiController,Route("nexus")]
public class OAuthController : BaseController
{
    private const double MaxAge = 60 * 60;
    private readonly Application oauthApplication;
    private readonly string[] scopes; 
    public const string RedirectUri = "https://localhost:7191/nexus/callback";
    public OAuthController(IConfiguration config, AuthenticationContext ctx, Application app)
        : base(config, ctx)
    {
        oauthApplication = app;
        scopes = new string[] { "openid", "offiline_access" };
    }

    [HttpGet("Authorize")]
    [ProducesResponseType((int)HttpStatusCode.Redirect)]
    public IActionResult Authorize()
       => Redirect(oauthApplication.GenerateAuthorizeUrl(scopes, redirect_uri: RedirectUri).ToString());

    [HttpGet("Callback")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CallbackAsync(string? code, string? state, bool web = true)
    {
        if (string.IsNullOrEmpty(code))
            return BadRequest();

        AccessToken accessToken;

        try
        {
            accessToken = await oauthApplication.GetAccessTokenAsync(code!, scopes, RedirectUri);
        }
        catch (Exception)
        {
            return Unauthorized();
        }

        var oauthAccount = await oauthApplication.GetAccountAsync(accessToken);

        // Temporary null value Error Correction
        oauthAccount.ShortName = string.Join(' ', oauthAccount.Name.Split(' ').Take(2));

        Account? account = await (from acc in authCtx.Accounts
                                  where acc.NexusId == oauthAccount.Id.ToString()
                                  select acc).FirstOrDefaultAsync();

        if (account is null)
        {
            account = new()
            {
                Email = oauthAccount.Email,
                Name = oauthAccount.Name,
                NexusId = oauthAccount.Id.ToString(),
                PictureUrl = oauthAccount.ProfileImageUrl().ToString(),
                ShortName = oauthAccount.Name
            };
            await authCtx.Accounts.AddAsync(account);
        }
        else {
            account.Name = oauthAccount.Name;
            account.Email = oauthAccount.Email;
            account.PictureUrl = oauthAccount.ProfileImageUrl().ToString();
        }

        await authCtx.SaveChangesAsync();

        var clientJWT = await oauthApplication.GetClientJWTAsync(code!, new string[] { "openid" }, MaxAge);

        string? refresh = "none"; //GeneralHelpers.GenerateToken(256);
        Authentication auth = new()
        {
            AccountId = account.Id,
            Token = clientJWT.Token.Id,
            Refresh = BCrypt.Net.BCrypt.HashPassword(refresh),
            Generated = DateTime.UtcNow,
            MaxAge = MaxAge
        };

        await authCtx.Authentications.AddAsync(auth);
        await authCtx.SaveChangesAsync();

        if (web)
        {
            HttpContext.Response.Cookies.Append(AuthenticationHelper.AuthKey, $"{clientJWT.Type} {clientJWT.Token.RawData}", new CookieOptions()
            {
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return Redirect(Config.WebUrl);
        }

        return StatusCode((int)HttpStatusCode.NotImplemented);
    }

    [HttpPost("Refresh")]
    [RequireAuthentication]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RefreshAsync()
    {
        throw new NotImplementedException();
    }
}