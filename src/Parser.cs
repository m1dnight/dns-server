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

        // Parse the questions
        var questions = ParseQuestions(bits, header.QuestionCount);


        return new DnsMessage(header, questions);
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

    private static uint ParseAnswerCount(BitArray bits)
    {
        // big endian
        var identifierBits = new BitArray(16);
        // for (var i = 0; i < 16; i++) identifierBits[i] = bits[48 + i];
        var offset = 48;
        for (var i = 0; i < 8; i++) identifierBits[8 + i] = bits[offset + i];
        for (var i = 0; i < 8; i++) identifierBits[i] = bits[offset + 8 + i];

        var identifierBytes = new byte[2];
        identifierBits.CopyTo(identifierBytes, 0);
        return BitConverter.ToUInt16(identifierBytes);
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

    private static uint ParseResponseCode(BitArray bits)
    {
        var opcodeBits = new BitArray(4);
        for (var i = 0; i < 4; i++) opcodeBits[i] = bits[24 + i];

        var opcodeBytes = new byte[2];
        opcodeBits.CopyTo(opcodeBytes, 0);
        return BitConverter.ToUInt16(opcodeBytes);
    }

    private static uint ParseReserved(BitArray bits)
    {
        var opcodeBits = new BitArray(4);
        for (var i = 0; i < 3; i++) opcodeBits[i] = bits[28 + i];

        var opcodeBytes = new byte[2];
        opcodeBits.CopyTo(opcodeBytes, 0);
        return BitConverter.ToUInt16(opcodeBytes);
    }

    private static bool ParseRecursionAvailable(BitArray bits)
    {
        return bits[31];
    }

    private static bool ParseRecursionDesired(BitArray bits)
    {
        return bits[16];
    }

    private static bool ParseTruncated(BitArray bits)
    {
        return bits[17];
    }

    private static bool ParseAuthorativeAnswer(BitArray bits)
    {
        return bits[18];
    }

    private static uint ParseOperationCode(BitArray bits)
    {
        var opcodeBits = new BitArray(4);
        for (var i = 0; i < 4; i++) opcodeBits[i] = bits[19 + i];

        var opcodeBytes = new byte[2];
        opcodeBits.CopyTo(opcodeBytes, 0);
        return BitConverter.ToUInt16(opcodeBytes);
    }

    private static bool ParseIsResponse(BitArray bits)
    {
        return bits[23];
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

    // 1  1  1  1  1  1
    // 0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                                               |
    // /                     QNAME                     /
    // /                                               /
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                     QTYPE                     |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                     QCLASS                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+


    private static List<Question> ParseQuestions(BitArray bits, uint questionCount)
    {
        var bytes = new byte[bits.Length / 8];
        bits.CopyTo(bytes, 0);

        // questions start at byte 13
        var questions = new List<Question>();
        var offset = 12; // header is 12 bytes
        for (var i = 0; i < questionCount; i++)
        {
            var q = ParseQuestion(bytes, offset);
            questions.Add(q);
        }

        return questions;
    }


    private static Question ParseQuestion(byte[] bytes, int offset)
    {
        var hasLabel = true;

        var labels = new List<string>();
        while (hasLabel)
        {
            var label = ParseLabel(bytes, ref offset);
            if (label == null)
                hasLabel = false;
            else
                labels.Add(System.Text.Encoding.UTF8.GetString(label));
        }

        // parse type and class 
        var labelType = Parse16BitsBigEndian(bytes, offset * 8);
        var labelClass = Parse16BitsBigEndian(bytes, offset * 8 + 4);

        // create a Question with the labels 
        var q = new Question
        {
            Name = string.Join(".", labels),
            Type = labelType,
            Class = labelClass
        };

        return q;
    }

    private static byte[]? ParseLabel(byte[] bytes, ref int offset)
    {
        var labelLength = bytes[offset];
        offset++;

        // if the label length is the nullbyte, the label is terminated.
        if (labelLength == 0x00) return null;

        var label = bytes[offset..(offset + labelLength)];
        offset += labelLength;
        return label;
    }

    private static uint Parse16BitsBigEndian(byte[] data, int offset)
    {
        // data is big endian
        // 16 bits needed, so extract 2 bytes, reverse, and write into a 4 byte array
        var uintBytes = new byte[4];
        var byteOffset = offset / 8;

        var slice = data[byteOffset..(byteOffset + 2)];
        Array.Reverse(slice);
        Array.Copy(slice, uintBytes, 2);

        // convert little endian byte array to uint
        return BitConverter.ToUInt32(uintBytes);
    }
}