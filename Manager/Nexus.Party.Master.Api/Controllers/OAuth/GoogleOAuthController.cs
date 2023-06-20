using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using System.Net;

namespace Nexus.Party.Master.Api.OAuth.Controllers;

[Route("Google")]
public class GoogleOAuthController : OAuthController
{
    private const string ConfigKey = "Google";
    public const string RedirectUri = "https://localhost:44383/google/callback";

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
        var accessToken = await GoogleAuthHelper.GetAccessToken(ClientId, Secret, code, RedirectUri, authUser);

        var requestUrl = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + accessToken.AccessToken;
        var response = await new HttpClient().GetAsync(requestUrl);
        var responseContent = await response.Content.ReadAsStringAsync();
        var obj = JsonConvert.DeserializeObject<GoogleAccountResponse>(responseContent);



        // Extrair os dados da conta do JSON de resposta
        // O formato do JSON de resposta pode variar dependendo da versão da API do Google

        if (web)
            return Redirect(Url.Action("Index", "Home", null));

        return StatusCode((int)HttpStatusCode.NotImplemented);
    }
}

public class GoogleAccountResponse
{
    public long Id { get; set; }
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