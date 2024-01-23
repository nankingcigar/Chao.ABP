using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;

namespace System;

public static class StringExtensions
{
    public static TEntity GetDataFromJsonFile<TEntity>(this string jsonFilePath)
    {
        var entitesText = File.ReadAllText(jsonFilePath);
        var entity = JsonSerializer.Deserialize<TEntity>(entitesText, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        return entity;
    }

    public static string GetFileName(this string str)
    {
        var invalidFileNameChars = Path.GetInvalidFileNameChars();
        foreach (var invalidFileNameChar in invalidFileNameChars)
        {
            str = str.Replace(invalidFileNameChar, '-');
        }
        return str;
    }

    public static string? GetFirstImageLink(this string str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return null;
        }
        if (Regex.IsMatch(str, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase) == false)
        {
            return null;
        }
        return Regex.Match(str, "<img.+?src=[\"" +
            ".0'](.+?)[\"'].+?>", RegexOptions.IgnoreCase).Groups[1].Value;
    }

    public static string HtmlDecode(this string str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return str;
        }
        return HttpUtility.HtmlDecode(str);
    }

    public static string Name(this string str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return str;
        }
        return str.Replace(str.SurName(), string.Empty).Trim();
    }

    public static string RemoveHtmlTag(this string str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return str;
        }
        if (Regex.IsMatch(str, @"<[^>]*>") == false)
        {
            return str;
        }
        return Regex.Replace(str, @"<[^>]*>", string.Empty).Trim();
    }

    public static string SurName(this string str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return str;
        }
        if (str.Contains(" ") == true)
        {
            return str.Split(" ", StringSplitOptions.RemoveEmptyEntries).Last();
        }
        else if (str.Contains("·") == true)
        {
            return str.Split("·", StringSplitOptions.RemoveEmptyEntries).Last();
        }
        else
        {
            return str.First().ToString();
        }
    }
}