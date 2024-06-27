using System.Collections;

namespace codecrafters_dns_server;

public class Util
{
    public static string ToHexString(byte[] bytes)
    {
        return string.Join(" ", bytes.ToList().Select(b => b.ToString("X2")));
    }

    public static string ToBinaryString(byte[] bytes)
    {
        return string.Join(" ", bytes.ToList().Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
    }


    public static void PrintHex(byte[] bytes, string label)
    {
        var str = ToHexString(bytes);
        Console.WriteLine($"{label,32}: {str}");
    }


    public static void PrintHex(BitArray bits, string label)
    {
        var bytes = new byte[bits.Length / 8];
        bits.CopyTo(bytes, 0);
        var str = ToHexString(bytes);
        Console.WriteLine($"{label,32}: {str}");
    }

    public static void PrintBinary(BitArray bits, string label)
    {
        var bytes = new byte[bits.Length / 8];
        bits.CopyTo(bytes, 0);
        var str = ToBinaryString(bytes);
        Console.WriteLine($"{label,32}: {str}");
    }

    public static void PrintBinary(byte[] bytes, string label)
    {
        var str = ToBinaryString(bytes);
        Console.WriteLine($"{label,32}: {str}");
    }
}