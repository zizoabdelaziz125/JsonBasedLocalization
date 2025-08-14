using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace JsonBasedLocalization.Web;

public class JsonStringLocalizer : IStringLocalizer
{

    private readonly JsonSerializer _jsonSerializer = new();

    public LocalizedString this[string name] 
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value, resourceNotFound: string.IsNullOrEmpty(value));
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var acctualValue = this[name];
            return acctualValue.ResourceNotFound
                ? acctualValue
                : new LocalizedString(name, string.Format(acctualValue.Value, arguments), false);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {

        var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";

        using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using StreamReader streamReader = new(fileStream);
        using JsonTextReader reader = new(streamReader);

        while (reader.Read())
        {
            if(reader.TokenType != JsonToken.PropertyName)
                continue;

            var key = reader.Value as string;
            var value = _jsonSerializer.Deserialize<string>(reader);
            yield return new LocalizedString(key, value ?? string.Empty, resourceNotFound: string.IsNullOrEmpty(value));
        }

    }

    private string GetString(string key)
    {
        if (string.IsNullOrEmpty(key))
            return string.Empty;

        var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
        var fullFilePath = Path.GetFullPath(filePath);
        if (File.Exists(fullFilePath))
        {
            return GetValueFromJSON(key, fullFilePath);
        }

        return string.Empty;
    }

    private string GetValueFromJSON(string propertyName, string filePath)
    {
        if(string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(propertyName))
            return string.Empty;

        using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using StreamReader streamReader = new(fileStream);
        using JsonTextReader reader = new(streamReader);

        while (reader.Read())
        {
            if(reader.TokenType == JsonToken.PropertyName && reader.Value as string == propertyName)
            {
                reader.Read(); // Move to the value token
                return _jsonSerializer.Deserialize<string>(reader) ?? string.Empty;
            }
        }

        return string.Empty;

    }
}
