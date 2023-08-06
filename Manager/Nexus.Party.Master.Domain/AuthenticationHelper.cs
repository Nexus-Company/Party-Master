using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexus.Party.Master.Dal;
using Nexus.Party.Master.Dal.Models.Accounts;
using static Nexus.Tools.Validations.Middlewares.Authentication.AuthenticationMidddleware;

namespace Nexus.Party.Master.Domain;

public class AuthenticationHelper
{
    public const string AuthKey = "auth_to";
    public readonly AuthenticationContext authCtx;

    public AuthenticationHelper(AuthenticationContext ctx)
    {
        authCtx = ctx;
    }

    /// <summary>
    /// Api Authentication Handler Implementation
    /// </summary>
    /// <param name="ctx">Http Context</param>
    /// <returns>Authentication Validation result with contains one valid authentication.</returns>
    public async Task<AuthenticationResult> ValidAuthenticationAsync(HttpContext ctx)
    {
        AuthenticationResult result = new(false, false);
        Authentication? auth = await GetAuthenticationAsync(authCtx, ctx);

        if (auth != null)
            return new(true, true);

        return result;
    }

    private static async Task<Authentication?> GetAuthenticationAsync(AuthenticationContext dbCtx, HttpContext ctx)
    {
        Authentication? auth = null;

        string cookie = ctx.Request.Cookies[AuthKey];

        if (string.IsNullOrEmpty(cookie))
        {
            cookie = ctx.Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;

            try
            {
                cookie = cookie.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(cookie))
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        auth = await (from aut in dbCtx.Authentications
                      where aut.Token == cookie
                      select aut).FirstOrDefaultAsync();


        return auth;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbCtx"></param>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public static async Task<Account?> TryGetAccount(AuthenticationContext dbCtx, HttpContext ctx)
    {
        Authentication? auth = await GetAuthenticationAsync(dbCtx, ctx);
        Account? account = null;

        if (auth == null)
            return account;

        account = await (from acc in dbCtx.Accounts
                         where acc.Id == auth.AccountId
                         select acc).FirstOrDefaultAsync();

        return null;
    }

    /// <summary>
    /// Generate Tokens with specific length
    /// </summary>
    /// <param name="size">Token Size</param>
    /// <param name="lower">Use lowercase characters.</param>
    /// <param name="upper">Use uppercase characters.</param>
    /// <returns>New token with size value.</returns>
    public static string GenerateToken(int size, bool upper = true, bool lower = true)
    {
        // ASCII characters rangers
        byte[] lowers = "a{"u8.ToArray();
        // Upercase latters
        byte[] uppers = "A["u8.ToArray();
        // ASCII numbers
        byte[] numbers = "0:"u8.ToArray();

        Random random = new();
        string result = string.Empty;

        for (int i = 0; i < size; i++)
        {
            int type = random.Next(0, lower ? 3 : 2);

            byte[] possibles = type switch
            {
                1 => upper ? uppers : numbers,
                2 => lowers,
                _ => numbers
            };

            int selected = random.Next(possibles[0], possibles[1]);
            char character = (char)selected;

            result += character;
        }

        return result;
    }
}
