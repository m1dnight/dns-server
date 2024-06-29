using System.Collections;
using System.Text;

namespace codecrafters_dns_server.DnsMessages;

public class Answer(string name, uint type, uint clazz, uint ttl, uint dataLength, byte[] data)
{
    public string Name { get; set; } = name;
    public uint Type { get; set; } = type;
    public uint Class { get; set; } = clazz;
    public uint TTL { get; set; } = ttl;
    public uint DataLength { get; set; } = dataLength;
    public byte[] Data { get; set; } = data;

    public byte[] ToBytes()
    {
        var bytes = new List<byte>();

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

        bytes.AddRange(BitConverter.GetBytes(TTL).Reverse());
        bytes.AddRange(BitConverter.GetBytes((ushort)DataLength).Reverse());
        bytes.AddRange(Data);
        return bytes.ToArray();
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
            var temp = bytes[rdLengthIndex..(rdLengthIndex + 2)];
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
        var typeS = nullByte + 1;
        var typeE = nullByte + 3;
        var tmp = bytes[(nullByte + 1)..(nullByte + 3)];
        type = BitConverter.ToUInt16(bytes[(nullByte + 1)..(nullByte + 3)].Reverse().ToArray());
        var clazzS = nullByte + 3;
        var clazzE = nullByte + 5;
        clazz = BitConverter.ToUInt16(bytes[(nullByte + 3)..(nullByte + 5)].Reverse().ToArray());

        var ttlS = nullByte + 5;
        var ttlE = nullByte + 9;
        ttl = BitConverter.ToUInt32(bytes[(nullByte + 5)..(nullByte + 9)].Reverse().ToArray());

        var rdLengthS = nullByte + 9;
        var rdLengthE = nullByte + 11;
        rdLength = BitConverter.ToUInt16(bytes[(nullByte + 9)..(nullByte + 11)].Reverse().ToArray());

        // parse the rdata 
        var x = nullByte + 11;
        var y = nullByte + 11 + (int)rdLength;
        rData = bytes[x..y];


        return new Answer(name, type, clazz, ttl, rdLength, rData);
    }
}