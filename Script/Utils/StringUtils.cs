namespace Beyondourborders.Script.Utils;

public class StringUtils
{
    public static string GetSimpleType(object? obj)
    {
        if (obj == null)
            return "null";
        return obj.GetType().ToString().Split(".")[^1];
    }

    public static string SsToHhMm(int seconds) {
        var totalMinutes = seconds / 60;
        var hours = totalMinutes / 60;
        var minutes = totalMinutes - hours;
        return $"{hours:D2}:{totalMinutes:D2}";
    }
}