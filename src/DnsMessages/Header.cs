using System.Collections;

namespace codecrafters_dns_server.DnsMessages;

public class Header
{
    public uint Identifier { get; set; }
    public bool QueryResponseIndicator { get; set; }
    public uint OperationCode { get; set; }

    public bool AuthoritativeAnswer { get; set; }

    public bool Truncation { get; set; }

    public bool RecursionDesired { get; set; }
    public bool RecursionAvailable { get; set; }
    public uint Reserved { get; set; }

    public uint ResponseCode { get; set; }

    public uint QuestionCount { get; set; }
    public uint AnswerRecordCount { get; set; }
    public uint AuthorityRecordCount { get; set; }
    public uint AdditionalRecordCount { get; set; }

    public byte[] ToBytes()
    {
        var bytes = new byte[12];
        /*
         * https://datatracker.ietf.org/doc/html/rfc1035#section-2.3.2
         * the 0th bit is the most significant bit
         *    0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
         *  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         *  |                      ID                       |
         *  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         *  |QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
         *  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         *  |                    QDCOUNT                    |
         *  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         *  |                    ANCOUNT                    |
         *  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         *  |                    NSCOUNT                    |
         *  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         *  |                    ARCOUNT                    |
         *  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         *
         */

        /*
         *   0   1   2   3   4   5   6   7
         *   15  14  13  12  11  10   9   8
         * +---+---+---+---+---+---+---+---+
         * |IDENTIFIER                     |
         * +---+---+---+---+---+---+---+---+
         */
        bytes[0] = (byte)(Identifier >> 8);
        bytes[1] = (byte)(Identifier >> 0);

        /*
         *   0   1   2   3   4   5   6   7
         *   23  22  21  20  19  18  17  16
         * +---+---+---+---+---+---+---+---+
         * |QR | Opcode        |AA |TC |RD |
         * +---+---+---+---+---+---+---+---+
         */
        bytes[2] = (byte)((uint)(QueryResponseIndicator ? 1 << 7 : 0)
                          | (OperationCode << 3)
                          | (uint)(AuthoritativeAnswer ? 1 << 2 : 0)
                          | (uint)(Truncation ? 1 << 1 : 0)
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

        /*
         *   0   1   2   3   4   5   6   7
         *   39  38  37  36  35  34  33  32
         * +---+---+---+---+---+---+---+---+
         * |QDCOUNT                        |
         * +---+---+---+---+---+---+---+---+
         */
        bytes[4] = (byte)(QuestionCount >> 8);
        bytes[5] = (byte)(QuestionCount >> 0);

        /*
         *   0   1   2   3   4   5   6   7
         *   47  46  45  44  43  42  41  40
         * +---+---+---+---+---+---+---+---+
         * |ANCOUNT                        |
         * +---+---+---+---+---+---+---+---+
         */
        bytes[6] = (byte)(AnswerRecordCount >> 8);
        bytes[7] = (byte)(AnswerRecordCount >> 0);

        /*
         *   0   1   2   3   4   5   6   7
         *   55  54  53  52  51  50  49  48
         * +---+---+---+---+---+---+---+---+
         * |NSCOUNT                        |
         * +---+---+---+---+---+---+---+---+
         */
        bytes[8] = (byte)(AuthorityRecordCount >> 8);
        bytes[9] = (byte)(AuthorityRecordCount >> 0);

        /*
         *   0   1   2   3   4   5   6   7
         *   63  62  61 60  59  58  57  56
         * +---+---+---+---+---+---+---+---+
         * |ARCOUNT                        |
         * +---+---+---+---+---+---+---+---+
         */
        bytes[10] = (byte)(AdditionalRecordCount >> 8);
        bytes[11] = (byte)(AdditionalRecordCount >> 0);
        return bytes;
    }

    public static Header Parse(BitArray bits)
    {
        // Parse the header
        var header = new Header
        {
            Identifier = ParseIdentifier(bits),
            QueryResponseIndicator = ParseIsResponse(bits),
            OperationCode = ParseOperationCode(bits),
            AuthoritativeAnswer = ParseAuthorativeAnswer(bits),
            Truncation = ParseTruncated(bits),
            RecursionDesired = ParseRecursionDesired(bits),
            RecursionAvailable = ParseRecursionAvailable(bits),
            Reserved = ParseReserved(bits),
            ResponseCode = ParseResponseCode(bits),
            QuestionCount = ParseQuestionCount(bits),
            AnswerRecordCount = ParseAnswerCount(bits),
            AuthorityRecordCount = ParseAuthorityCount(bits),
            AdditionalRecordCount = ParseAdditionalCount(bits)
        };
        return header;
    }

    private static uint ParseIdentifier(BitArray bits)
    {
        // identifier is big endian
        var identifierBits = new BitArray(16);
        for (var i = 0; i < 8; i++) identifierBits[8 + i] = bits[i];
        for (var i = 0; i < 8; i++) identifierBits[i] = bits[8 + i];

        var identifierBytes = new byte[2];
        identifierBits.CopyTo(identifierBytes, 0);
        return BitConverter.ToUInt16(identifierBytes);
    }

    private static bool ParseIsResponse(BitArray bits)
    {
        return bits[23];
    }

    private static uint ParseOperationCode(BitArray bits)
    {
        var opcodeBits = new BitArray(4);
        for (var i = 0; i < 4; i++) opcodeBits[i] = bits[19 + i];

        var opcodeBytes = new byte[2];
        opcodeBits.CopyTo(opcodeBytes, 0);
        return BitConverter.ToUInt16(opcodeBytes);
    }

    private static bool ParseAuthorativeAnswer(BitArray bits)
    {
        return bits[18];
    }

    private static bool ParseTruncated(BitArray bits)
    {
        return bits[17];
    }

    private static bool ParseRecursionDesired(BitArray bits)
    {
        return bits[16];
    }

    private static bool ParseRecursionAvailable(BitArray bits)
    {
        return bits[31];
    }

    private static uint ParseReserved(BitArray bits)
    {
        var opcodeBits = new BitArray(4);
        for (var i = 0; i < 3; i++) opcodeBits[i] = bits[28 + i];

        var opcodeBytes = new byte[2];
        opcodeBits.CopyTo(opcodeBytes, 0);
        return BitConverter.ToUInt16(opcodeBytes);
    }

    private static uint ParseResponseCode(BitArray bits)
    {
        var opcodeBits = new BitArray(4);
        for (var i = 0; i < 4; i++) opcodeBits[i] = bits[24 + i];

        var opcodeBytes = new byte[2];
        opcodeBits.CopyTo(opcodeBytes, 0);
        return BitConverter.ToUInt16(opcodeBytes);
    }

    private static uint ParseQuestionCount(BitArray bits)
    {
        // big endian
        var identifierBits = new BitArray(16);
        // for (var i = 0; i < 16; i++) identifierBits[i] = bits[32 + i];

        var offset = 32;
        for (var i = 0; i < 8; i++) identifierBits[8 + i] = bits[offset + i];
        for (var i = 0; i < 8; i++) identifierBits[i] = bits[offset + 8 + i];

        var identifierBytes = new byte[2];
        identifierBits.CopyTo(identifierBytes, 0);
        return BitConverter.ToUInt16(identifierBytes);
    }

    private static uint ParseAnswerCount(BitArray bits)
    {
        var bytes = new byte[bits.Length / 8];
        bits.CopyTo(bytes, 0);
        // big endian
        var identifierBits = new BitArray(16);
        // for (var i = 0; i < 16; i++) identifierBits[i] = bits[48 + i];
        var offset = 48;
        for (var i = 0; i < 8; i++) identifierBits[8 + i] = bits[offset + i];
        for (var i = 0; i < 8; i++) identifierBits[i] = bits[offset + 8 + i];

        var identifierBytes = new byte[2];
        identifierBits.CopyTo(identifierBytes, 0);
        Util.PrintHex(bits, "asnwercount");
        return BitConverter.ToUInt16(identifierBytes);
    }

    private static uint ParseAuthorityCount(BitArray bits)
    {
        // big endian
        var identifierBits = new BitArray(16);
        // for (var i = 0; i < 16; i++) identifierBits[i] = bits[64 + i];
        var offset = 64;
        for (var i = 0; i < 8; i++) identifierBits[8 + i] = bits[offset + i];
        for (var i = 0; i < 8; i++) identifierBits[i] = bits[offset + 8 + i];

        var identifierBytes = new byte[2];
        identifierBits.CopyTo(identifierBytes, 0);
        return BitConverter.ToUInt16(identifierBytes);
    }

    private static uint ParseAdditionalCount(BitArray bits)
    {
        // big endian
        var identifierBits = new BitArray(16);
        // for (var i = 0; i < 16; i++) identifierBits[i] = bits[80 + i];

        var offset = 80;
        for (var i = 0; i < 8; i++) identifierBits[8 + i] = bits[offset + i];
        for (var i = 0; i < 8; i++) identifierBits[i] = bits[offset + 8 + i];

        var identifierBytes = new byte[2];
        identifierBits.CopyTo(identifierBytes, 0);
        return BitConverter.ToUInt16(identifierBytes);
    }
}