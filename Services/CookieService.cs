using Microsoft.AspNetCore.Http;

namespace CsdbMigration.Services;

public class CookieService(IHttpContextAccessor httpContextAccessor)
{
    public void SetCookie(string key, string value, int expireInDays)
    {
        var option = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(expireInDays)
        };

        httpContextAccessor.HttpContext?.Response.Cookies.Append(key, value, option);
    }

    public string GetCookie(string key) => httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(key, out string? value) == true ? value : string.Empty;
}