using System.Collections;
using System.Text;

namespace codecrafters_dns_server.DnsMessages;

public class Answer
{
    public string Name { get; set; }
    public uint Type { get; set; }
    public uint Class { get; set; }
    public uint TTL { get; set; }
    public uint DataLength { get; set; }
    public byte[] Data { get; set; }

    public byte[] ToBytes()
    {
        /*
         * 0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
         * +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         * |                                               |
         * /                                               /
         * /                      NAME                     /
         * |                                               |
         * +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         * |                      TYPE                     |
         * +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         * |                     CLASS                     |
         * +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         * |                      TTL                      |
         * |                                               |
         * +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         * |                   RDLENGTH                    |
         * +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|
         * /                     RDATA                     /
         * /                                               /
         * +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         */
        return [];
    }

    public static List<Answer> ParseMany(BitArray bits, uint answerCount)
    {
        var bytes = new byte[bits.Length / 8];
        bits.CopyTo(bytes, 0);

        var answers = new List<Answer>();

        var start = 12;
        var end = -1;

        for (var ai = 0; ai < answerCount; ai++)
        {
            // find the end of the labels 
            var nullByte = -1;
            for (var i = start; i < bytes.Length; i++)
                if (bytes[i] == 0x00)
                {
                    nullByte = i;
                    break;
                }

            // add 8 bytes for type, class and ttl
            var rdLengthIndex = nullByte + 8 + 1;

            // read the length of the rdata 
            uint rdLength = BitConverter.ToUInt16(bytes[rdLengthIndex..(rdLengthIndex + 2)].Reverse().ToArray());

            end = rdLengthIndex + (int)rdLength;

            var answer = Parse(bytes, start, end);
            answers.Add(answer);
            start = end;
        }

        return answers;
    }

    private static Answer Parse(byte[] bytes, int start, int end)
    {
        var name = "";
        uint type = 0;
        uint clazz = 0;
        uint ttl = 0;
        uint rdLength = 0;
        byte[] rData = [];

        // parse the labels
        var nullByte = -1;
        List<string> labels = new();
        for (var i = start; i < end; i++)
        {
            var labelLength = bytes[i];
            if (labelLength == 0x00)
            {
                nullByte = i;
                break;
            }

            var label = Encoding.UTF8.GetString(bytes, i + 1, labelLength);
            labels.Add(label);
            i += labelLength;
        }

        name = string.Join(".", labels);

        // parse the type, class, and ttl 
        type = BitConverter.ToUInt16(bytes[(nullByte + 1)..(nullByte + 3)].Reverse().ToArray());
        clazz = BitConverter.ToUInt16(bytes[(nullByte + 4)..(nullByte + 6)].Reverse().ToArray());
        ttl = BitConverter.ToUInt32(bytes[(nullByte + 7)..(nullByte + 11)].Reverse().ToArray());
        rdLength = BitConverter.ToUInt16(bytes[(nullByte + 12)..(nullByte + 14)].Reverse().ToArray());

        // parse the rdata 
        var x = nullByte + 15;
        var y = nullByte + 15 + (int)rdLength;
        rData = bytes[x..y];


        return new Answer
        {
            Name = name,
            Type = type,
            Class = clazz,
            TTL = ttl,
            DataLength = rdLength,
            Data = rData
        };
    }
}