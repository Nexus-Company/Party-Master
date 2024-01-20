using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Microsoft.EntityFrameworkCore;
using Nexus.Party.Master.Dal.Models.Accounts;
using System.Net;
using System.Numerics;

namespace Nexus.Party.Master.Api.OAuth.Controllers;

[AllowAnonymous]
[Route("Google")]
public class GoogleOAuthController : OAuthController
{
    private const string ConfigKey = "Google";
    public const string RedirectUri = "https://localhost:7191/google/callback";

    public GoogleOAuthController(IConfiguration config)
       : base(config, ConfigKey)
    {

    }

    [HttpGet("Authorize")]
    public IActionResult GoogleLogin()
        => Redirect(GoogleAuthHelper.GetAuthorizationUri(ClientId, RedirectUri));


    [HttpGet("CallBack")]
    public async Task<IActionResult> GoogleCallbackAsync(string code, string authUser, bool web = true)
    {
        Account? account;

        try
        {
            var accessToken = await GoogleAuthHelper.GetAccessToken(ClientId, Secret, code, RedirectUri, authUser);

            var requestUrl = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + accessToken.AccessToken;
            var response = await new HttpClient().GetAsync(requestUrl);
            var responseContent = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<GoogleAccountResponse>(responseContent);

            string gId = obj.Id.ToString();

            account = await (from acc in authCtx.Accounts
                             where acc.GoogleId == gId
                             select acc).FirstOrDefaultAsync();

            if (account == null)
            {
                account = new Account()
                {
                    GoogleId = gId,
                    Name = obj.Name,
                    PictureUrl = obj.Picture,
                    Email = obj.Email
                };

                await authCtx.Accounts.AddAsync(account);
            }
            else
            {
                account.Name = obj.Name;
                account.PictureUrl = obj.Picture;
                account.Email = obj.Email;
            }

            await authCtx.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return RedirectToAction("Authorize");
        }

        Authentication auth = new(AuthenticationHelper.GenerateToken(96), account.Id);

        await authCtx.Authentications.AddAsync(auth);

        await authCtx.SaveChangesAsync();

        if (web)
        {
            HttpContext.Response.Cookies.Append(AuthenticationHelper.AuthKey, auth.Token, new CookieOptions()
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
}

public class GoogleAccountResponse
{
    public BigInteger Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Picture { get; set; }
}

public static class GoogleAuthHelper
{
    private static readonly string[] Scopes = { "openid", "profile", "email" };

    public static string GetAuthorizationUri(string clientId, string redirectUri)
    {
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets { ClientId = clientId },
            Scopes = Scopes,
            DataStore = new FileDataStore("Tokens")
        });

        return flow.CreateAuthorizationCodeRequest(redirectUri).Build().ToString();
    }

    public static async Task<TokenResponse> GetAccessToken(string clientId, string clientSecret, string code, string redirectUri, string authUser)
    {
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret },
            Scopes = Scopes,
            DataStore = new FileDataStore("Tokens")
        });

        var token = await flow.ExchangeCodeForTokenAsync(authUser, code, redirectUri, CancellationToken.None);
        return token;
    }
}