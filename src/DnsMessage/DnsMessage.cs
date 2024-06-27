using System.Collections;
using codecrafters_dns_server;

public class DnsMessage : IToBytes
{
    public DnsMessage(Header header)
    {
        Header = header;
    }

    public Header Header { get; set; }

    // public Question[] Questions { get; set; }
    // public ResourceRecord[] Answers { get; set; }
    // public ResourceRecord[] Authority { get; set; }
    // public ResourceRecord[] Additional { get; set; }
    public byte[] ToBytes()
    {
        var bytes = new List<byte>();
        bytes.AddRange(Header.ToBytes());

        return bytes.ToArray();
    }

    public byte[] ToBytesBigEndian()
    {
        var bytes = ToBytes();

        // swap the first two bytes to big endian 
        (bytes[0], bytes[1]) = (bytes[1], bytes[0]);
        (bytes[4], bytes[5]) = (bytes[5], bytes[4]);
        (bytes[6], bytes[7]) = (bytes[7], bytes[6]);
        (bytes[8], bytes[9]) = (bytes[9], bytes[8]);
        (bytes[10], bytes[11]) = (bytes[11], bytes[10]);

        return bytes;
    }
}