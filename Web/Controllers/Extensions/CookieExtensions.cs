using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

public static class CookieExtensions
{
    public static string GetCookie(HttpRequest request, string key)
    {
        try
        {
            return request.Cookies[key];
        }
        catch (KeyNotFoundException)
        {
            return "0";
        }
    }

    public static void SetCookie(HttpResponse response, string key, string value, int? expireTime = null)
    {
        CookieOptions option = new CookieOptions();

        if (expireTime.HasValue)
        {
            option.Expires = DateTime.Now.AddMinutes(expireTime.Value);

            response.Cookies.Append(key, value, option);
        }
        else
        {
            response.Cookies.Append(key, value);
        }
    }

    public static void RemoveCookie(HttpResponse response, string key)
    {
        response.Cookies.Delete(key);
    }
}