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

        var start = 12;
        var end = -1;
        for (var qi = 0; qi < questionCount; qi++)
            // if the message is compressed, the name is a pointer to the original name
            // if ((bytes[start] & 0xC0) == 0xC0)
            // {
            //     // skip the pointer
            //     start += 2;
            //     questions.Add(Parse(bytes, start, start + 4));
            //     start += 4;
            //     continue;
            // }
            // else
            // {
                // find end of question (4 bytes after the null byte)
                for (var i = start; i < bytes.Length; i++)
                    if (bytes[i] == 0x00)
                    {
                        end = i + 4;
                        break;
                    }
            // }

        questions.Add(Parse(bytes, start, end));

        // set offsets for the next question
        start = end + 1;
        end = -1;

        return questions;
    }

    private static Question Parse(byte[] bytes, int start, int end)
    {
        var name = "";
        uint type = 0;
        uint clazz = 0;


        // parse the labels
        List<string> labels = new();

        // if the message is compressed, the name is a pointer to the original name
        if ((bytes[start] & 0xC0) == 0xC0)
        {
        }

        for (var i = start; i < end; i++)
        {
            var labelLength = bytes[i];
            if (labelLength == 0x00) break;

            var label = Encoding.UTF8.GetString(bytes, i + 1, labelLength);
            labels.Add(label);
            i += labelLength;
        }

        name = string.Join(".", labels);

        // parse the type and class
        type = BitConverter.ToUInt16(bytes[^4..^2].Reverse().ToArray());
        clazz = BitConverter.ToUInt16(bytes[^2..].Reverse().ToArray());


        return new Question(name, type, clazz);
    }
}