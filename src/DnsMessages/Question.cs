using System.Collections;
using System.Text;

namespace codecrafters_dns_server.DnsMessages;

public class Question(string name, uint type, uint clazz)
{
    public string Name { get; set; } = name;
    public uint Type { get; set; } = type;
    public uint Class { get; set; } = clazz;

    public byte[] ToBytes()
    {
        var bytes = new List<byte>();


        /*
         *
         * query name                                  type   class
         * ------------------------------------------  -----  -----
         * HEX    06 67 6f 6f 67 6c 65 03 63 6f 6d 00  00 01  00 01
         * ASCII      g  o  o  g  l  e     c  o  m
         *
         * +---+---+---+---+---+---+---+
         * | VARIABLE  | 2 BTS | 2 BTS |
         * +---+---+---+---+---+---+---+
         * |NAME       | TYPE  | CLASS |
         * +---+---+---+---+---+---+---+
         */


        // split name into labels and prefix with byte for their length
        var labels = Name.Split('.');
        foreach (var label in labels)
        {
            bytes.Add((byte)label.Length);
            // add the bytes of the label. 
            bytes.AddRange(Encoding.UTF8.GetBytes(label));
        }

        bytes.Add(0x00);


        // type and class in big endian 
        bytes.AddRange(BitConverter.GetBytes((ushort)Type).Reverse());
        bytes.AddRange(BitConverter.GetBytes((ushort)Class).Reverse());

        return bytes.ToArray();
    }


    public static List<Question> ParseMany(BitArray bits, uint questionCount)
    {
        var bytes = new byte[bits.Length / 8];
        bits.CopyTo(bytes, 0);

        var questions = new List<Question>();

        var questionStart = 12;
        for (var qi = 0; qi < questionCount; qi++)
        {
            var question = Parse(bytes, ref questionStart);
            questions.Add(question);
        }

        return questions;
    }

    private static Question Parse(byte[] bytes, ref int start)
    {
        var name = ParseName(bytes, ref start);

        // parse the type and class
        var type = BitConverter.ToUInt16(bytes[start..(start + 2)].Reverse().ToArray());
        var clazz = BitConverter.ToUInt16(bytes[(start + 2)..(start + 4)].Reverse().ToArray());

        // bump the pointer for the next question
        start += 4;


        return new Question(name, type, clazz);
    }

    private static string ParseName(byte[] bytes, ref int start)
    {
        // parse labels until a pointer, or until 0x00
        var labels = new List<string>();

        while (bytes[start] != 0x00 && (bytes[start] & 0xC0) != 0xC0)
        {
            // parse a single label 
            var label = ParseLabel(bytes, ref start);
            labels.Add(label);
        }

        // if the end is a pointer, parse the pointer too.
        if ((bytes[start] & 0xC0) == 0xC0)
        {
            var pointerBytes = new[] { bytes[start + 1], bytes[start] };
            var labelPosition = BitConverter.ToUInt16(pointerBytes) & 0x3FFF;
            var label = ParseName(bytes, ref labelPosition);
            labels.Add(label);
            start += 2;
        }
        else
        {
            // move pointer past null byte of labels 
            start += 1;
        }

        return string.Join(".", labels);
    }

    private static string ParseLabel(byte[] bytes, ref int offset)
    {
        var labelLength = bytes[offset];
        var label = Encoding.UTF8.GetString(bytes, offset + 1, labelLength);
        offset += labelLength + 1;
        return label;
    }

    private static string ParseCompressedLabel(byte[] bytes, ref int offset)
    {
        var pointerBytes = new byte[] { bytes[offset + 1], bytes[offset] };
        var labelPosition = BitConverter.ToUInt16(pointerBytes) & 0x3FFF;
        var label = ParseLabel(bytes, ref labelPosition);
        offset += 2;
        return label;
    }
}