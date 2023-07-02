using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace AMS.Utils;

public class IntListToStringValueConverter : ValueConverter<IEnumerable<int>, string>
{
    public IntListToStringValueConverter() : base(le => ListToString(le), (s => StringToList(s)))
    {

    }
    public static string ListToString(IEnumerable<int> value)
    {
        if (value == null || !value.Any())
        {
            return "";
        }

        return string.Join(",", value);
    }

    public static IEnumerable<int> StringToList(string value)
    {
        if (value == null || value == string.Empty)
        {
            return null;
        }

        return value.Split(',').Select(i => Convert.ToInt32(i)); ;

    }
}