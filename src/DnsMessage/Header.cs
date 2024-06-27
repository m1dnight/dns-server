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
        // https://datatracker.ietf.org/doc/html/rfc1035#section-2.3.2
        // the 0th bit is the most significant bit
        // 0 1 2 3 4 5 6 7
        // +-+-+-+-+-+-+-+-+
        // |1 0 1 0 1 0 1 0|
        // +-+-+-+-+-+-+-+-+


        // 1  1  1  1  1  1
        // 0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        // |                      ID                       |
        // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        // |QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
        // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        // |                    QDCOUNT                    |
        // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        // |                    ANCOUNT                    |
        // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        // |                    NSCOUNT                    |
        // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        // |                    ARCOUNT                    |
        // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        var bytes = new byte[12];

        bytes[0] = (byte)(Identifier >> 0);
        bytes[1] = (byte)(Identifier >> 8);
        /*
         *   0   1   2   3   4   5   6   7
         *   23  22  21  20  19  18  17  16
         * +---+---+---+---+---+---+---+---+
         * |QR | Opcode        |AA |TC |RD |
         * +---+---+---+---+---+---+---+---+
         */
        bytes[2] = (byte)((uint)(IsResponse ? 1 << 7 : 0)
                          | (OperationCode << 3)
                          | (uint)(AuthoritativeAnswer ? 1 << 2 : 0)
                          | (uint)(Truncated ? 1 << 1 : 0)
                          | (uint)(RecursionDesired ? 1 : 0));

        /*
         *   0   1   2   3   4   5   6   7
         *   31  30  29  28  27  26  25  24
         * +---+---+---+---+---+---+---+---+
         * |RA |Z          |RCODE          |
         * +---+---+---+---+---+---+---+---+
         */

        bytes[3] = (byte)((uint)(RecursionAvailable ? 1 << 7 : 0)
                          | (Reserved << 4)
                          | ResponseCode);

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