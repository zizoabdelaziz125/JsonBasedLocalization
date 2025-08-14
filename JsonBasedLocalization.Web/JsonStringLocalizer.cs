using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace JsonBasedLocalization.Web;

public class JsonStringLocalizer : IStringLocalizer
{
    private readonly JsonSerializer _jsonSerializer = new();
    public LocalizedString this[string name] => throw new NotImplementedException();

    public LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        throw new NotImplementedException();
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
