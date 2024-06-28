namespace codecrafters_dns_server.DnsMessages;

public class Answer
{
    public string Name { get; set; }
    public uint Type { get; set; }
    public uint Class { get; set; }
    public uint TTL { get; set; }
    public ushort DataLength { get; set; }
    public byte[] Data { get; set; }
}