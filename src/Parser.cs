using System.Collections;

namespace codecrafters_dns_server;

public class Parser
{
    public static DnsMessage ParseDnsMessage(BitArray bits)
    {
        // Parse the header
        var header = new Header();
        header.Identifier = ParseIdentifier(bits);
        header.IsResponse = ParseIsResponse(bits);
        header.OperationCode = ParseOperationCode(bits);
        header.AuthoritativeAnswer = ParseAuthorativeAnswer(bits);
        header.Truncated = ParseTruncated(bits);
        header.RecursionDesired = ParseRecursionDesired(bits);
        header.RecursionAvailable = ParseRecursionAvailable(bits);
        header.Reserved = ParseReserved(bits);
        header.ResponseCode = ParseResponseCode(bits);

        header.QuestionCount = ParseQuestionCount(bits);
        header.AnswerCount = ParseAnswerCount(bits);
        header.AuthorityCount = ParseAuthorityCount(bits);
        header.AdditionalCount = ParseAdditionalCount(bits);


        return new DnsMessage(header);
    }

    private static uint ParseAdditionalCount(BitArray bits)
    {
        var identifierBits = new BitArray(16);
        for (var i = 0; i < 16; i++) identifierBits[i] = bits[80 + i];

        var identifierBytes = new byte[2];
        identifierBits.CopyTo(identifierBytes, 0);
        return BitConverter.ToUInt16(identifierBytes);
    }

    private static uint ParseAuthorityCount(BitArray bits)
    {
        var identifierBits = new BitArray(16);
        for (var i = 0; i < 16; i++) identifierBits[i] = bits[64 + i];

        var identifierBytes = new byte[2];
        identifierBits.CopyTo(identifierBytes, 0);
        return BitConverter.ToUInt16(identifierBytes);
    }

    private static uint ParseAnswerCount(BitArray bits)
    {
        var identifierBits = new BitArray(16);
        for (var i = 0; i < 16; i++) identifierBits[i] = bits[48 + i];

        var identifierBytes = new byte[2];
        identifierBits.CopyTo(identifierBytes, 0);
        return BitConverter.ToUInt16(identifierBytes);
    }

    private static uint ParseQuestionCount(BitArray bits)
    {
        var identifierBits = new BitArray(16);
        for (var i = 0; i < 16; i++) identifierBits[i] = bits[32 + i];

        var identifierBytes = new byte[2];
        identifierBits.CopyTo(identifierBytes, 0);
        return BitConverter.ToUInt16(identifierBytes);
    }

    private static uint ParseResponseCode(BitArray bits)
    {
        var opcodeBits = new BitArray(4);
        for (var i = 0; i < 4; i++) opcodeBits[i] = bits[28 + i];

        var opcodeBytes = new byte[2];
        opcodeBits.CopyTo(opcodeBytes, 0);
        return BitConverter.ToUInt16(opcodeBytes);
    }

    private static uint ParseReserved(BitArray bits)
    {
        var opcodeBits = new BitArray(4);
        for (var i = 0; i < 3; i++) opcodeBits[i] = bits[25 + i];

        var opcodeBytes = new byte[2];
        opcodeBits.CopyTo(opcodeBytes, 0);
        return BitConverter.ToUInt16(opcodeBytes);
    }

    private static bool ParseRecursionAvailable(BitArray bits)
    {
        return bits[24];
    }

    private static bool ParseRecursionDesired(BitArray bits)
    {
        return bits[23];
    }

    private static bool ParseTruncated(BitArray bits)
    {
        return bits[22];
    }

    private static bool ParseAuthorativeAnswer(BitArray bits)
    {
        return bits[21];
    }

    private static uint ParseOperationCode(BitArray bits)
    {
        var opcodeBits = new BitArray(4);
        for (var i = 0; i < 4; i++) opcodeBits[i] = bits[17 + i];

        var opcodeBytes = new byte[2];
        opcodeBits.CopyTo(opcodeBytes, 0);
        return BitConverter.ToUInt16(opcodeBytes);
    }

    private static bool ParseIsResponse(BitArray bits)
    {
        return bits[16];
    }

    private static uint ParseIdentifier(BitArray bits)
    {
        var identifierBits = new BitArray(16);
        for (var i = 0; i < 16; i++) identifierBits[i] = bits[i];

        var identifierBytes = new byte[2];
        identifierBits.CopyTo(identifierBytes, 0);
        return BitConverter.ToUInt16(identifierBytes);
    }
}