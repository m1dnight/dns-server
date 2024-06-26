namespace codecrafters_dns_server;

public class Util
{
    public static string ToHexString(byte[] bytes)
    {
        return string.Join(" ", bytes.ToList().Select(b => b.ToString("X2")));
    }


    public static void PrintHex(byte[] bytes, string label)
    {
        var str = ToHexString(bytes);
        Console.WriteLine($"{label,32}: {str}");
    }
}