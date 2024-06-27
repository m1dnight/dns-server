namespace codecrafters_dns_server;

public class Header : IToBytes
{
    public uint Identifier { get; set; }
    public bool IsResponse { get; set; }
    public uint OperationCode { get; set; }

    public bool AuthoritativeAnswer { get; set; }

    public bool Truncated { get; set; }

    public bool RecursionDesired { get; set; }
    public bool RecursionAvailable { get; set; }
    public uint Reserved { get; set; }

    public uint ResponseCode { get; set; }

    public uint QuestionCount { get; set; }
    public uint AnswerCount { get; set; }
    public uint AuthorityCount { get; set; }
    public uint AdditionalCount { get; set; }

    public byte[] ToBytes()
    {
        var bytes = new byte[12];
        bytes[0] = (byte)(Identifier >> 0);
        bytes[1] = (byte)(Identifier >> 8);

        bytes[2] = (byte)((uint)(IsResponse ? 1 : 0)
                          | (OperationCode << 1)
                          | (uint)(AuthoritativeAnswer ? 1 << 5 : 0)
                          | (uint)(Truncated ? 1 << 6 : 0)
                          | (uint)(RecursionDesired ? 1 << 7 : 0));

        bytes[3] = (byte)((uint)(RecursionAvailable ? 1 : 0)
                          | (Reserved << 1)
                          | (ResponseCode << 4));

        // 32
        bytes[4] = (byte)(QuestionCount >> 0);
        bytes[5] = (byte)(QuestionCount >> 8);
        // 48
        bytes[6] = (byte)(AnswerCount >> 0);
        bytes[7] = (byte)(AnswerCount >> 8);
        // 64 
        bytes[8] = (byte)(AuthorityCount >> 0);
        bytes[9] = (byte)(AuthorityCount >> 8);
        // 80
        bytes[10] = (byte)(AdditionalCount >> 0);
        bytes[11] = (byte)(AdditionalCount >> 8);
        return bytes;
    }

    public byte[] ToBytesBigEndian()
    {
        throw new NotImplementedException();
    }
}