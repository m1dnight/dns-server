namespace codecrafters_dns_server;

public class Question
{
    public string Name { get; set; }
    public uint Type { get; set; }
    public uint Class { get; set; }

    public byte[] ToBytes()
    {
        // query name                            type   class
        // -----------------------------------  -----  -----
        // HEX    06 67 6f 6f 67 6c 65 03 63 6f 6d 00  00 01  00 01
        // ASCII      g  o  o  g  l  e     c  o  m

        var bytes = new List<byte>();
        var labels = Name.Split('.');
        foreach (var label in labels)
        {
            bytes.Add((byte)label.Length);
            bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(label));
        }

        bytes.Add(0);

        bytes.AddRange(BitConverter.GetBytes((ushort)Type).Reverse());
        bytes.AddRange(BitConverter.GetBytes((ushort)Class).Reverse());

        return bytes.ToArray();
    }
}