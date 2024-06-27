using System.Collections;
using NUnit.Framework;

namespace codecrafters_dns_server;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ResponsePacket()
    {
        byte[] receivedData =
        {
            0x4B, 0x64, 0x81, 0x80, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };
        // parse the message 
        var input = new BitArray(receivedData);

        Util.PrintHex(input, " IN");
        var dnsMessage = Parser.ParseDnsMessage(input);


        Assert.That(dnsMessage.Header.IsResponse, Is.True);
    }

    [Test]
    public void RequestPacket()
    {
        byte[] receivedData =
        {
            0x4B, 0x64, 0x01, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };
        // parse the message 
        var input = new BitArray(receivedData);

        Util.PrintHex(input, " IN");
        var dnsMessage = Parser.ParseDnsMessage(input);


        Assert.That(dnsMessage.Header.IsResponse, Is.False);
    }

    [Test]
    public void BitOrder()
    {
        uint input = 1;
        var bytes = BitConverter.GetBytes(input);
        var bits = new BitArray(bytes);

        Util.PrintHex(bytes, "bytes");
        Util.PrintHex(bits, "bits");

        Assert.That(bits[0], Is.True);
    }

    [Test]
    public void EndianSchmendian()
    {
        uint input = 256;
        var bytes = new byte[2];
        bytes[0] = (byte)(input >> 0);
        bytes[1] = (byte)(input >> 8);

        uint output = BitConverter.ToUInt16(bytes);

        Assert.That(input, Is.EqualTo(output));
    }

    [Test]
    public void EndianSchmendianBig()
    {
        var bytes = new byte[] { 0x04, 0xD2 };

        uint output = BitConverter.ToUInt16(bytes);

        Assert.That(output, Is.Not.EqualTo(1234));
    }

    [Test]
    public void EndianSchmendianBigCorrect()
    {
        var bytes = new byte[] { 0x04, 0xD2 };
        (bytes[0], bytes[1]) = (bytes[1], bytes[0]);
        uint output = BitConverter.ToUInt16(bytes);

        Assert.That(output, Is.EqualTo(1234));
    }

    [Test]
    public void EndianSchmendianBigParseBit()
    {
        // input of 1234 in big endian 
        var bytes = new byte[] { 0x04, 0xD2 };
        var inputBits = new BitArray(bytes);
        var outputBits = new BitArray(16);
        for (var i = 0; i < 8; i++) outputBits[8 + i] = inputBits[i];
        for (var i = 0; i < 8; i++) outputBits[i] = inputBits[8 + i];

        var outputBytes = new byte[2];
        outputBits.CopyTo(outputBytes, 0);
        uint output = BitConverter.ToUInt16(outputBytes);
        Assert.That(output, Is.EqualTo(1234));
    }


    [Test]
    public void TestFullParse()
    {
        // these are the exact same bytes as the request, except the identifier which is different each time.
        var receivedData = new byte[]
        {
            0x06, 0xA0, 0x01, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x63, 0x6F, 0x64, 0x65,
            0x63, 0x72, 0x61, 0x66, 0x74, 0x65, 0x72, 0x73, 0x02, 0x69, 0x6F, 0x00, 0x00, 0x01, 0x00, 0x01
        };

        // parse the message 
        var input = new BitArray(receivedData);

        Util.PrintHex(input, " IN");
        var dnsMessage = Parser.ParseDnsMessage(input);

        // convert the message back to output in big endian, should be exactly the same as the input
        var output = dnsMessage.ToBytesBigEndian();
        var sentData = new BitArray(output);

        Util.PrintHex(sentData, "OUT");

        var mismatches = new List<int>();
        for (var i = 0; i < 8 * 12; i++)
            if (input[i] != sentData[i])
                mismatches.Add(i);

        Assert.That(mismatches, Is.Empty);
    }


    [Test]
    public void TestFullParseAdditionalCount()
    {
        // these are the exact same bytes as the request, except the identifier which is different each time.
        var receivedData = new byte[]
        {
            0x06, 0xA0, 0x01, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x04, 0xD2, 0x0C, 0x63, 0x6F, 0x64, 0x65,
            0x63, 0x72, 0x61, 0x66, 0x74, 0x65, 0x72, 0x73, 0x02, 0x69, 0x6F, 0x00, 0x00, 0x01, 0x04, 0xD2
        };

        // parse the message 
        var input = new BitArray(receivedData);
        var dnsMessage = Parser.ParseDnsMessage(input);

        Assert.That(dnsMessage.Header.AdditionalCount, Is.EqualTo(1234));
    }


    [Test]
    public void TestFullParseAuthorityCount()
    {
        // these are the exact same bytes as the request, except the identifier which is different each time.
        var receivedData = new byte[]
        {
            0x06, 0xA0, 0x01, 0x20, 0x00, 0x01, 0x00, 0x00, 0x04, 0xD2, 0x00, 0x00, 0x0C, 0x63, 0x6F, 0x64, 0x65,
            0x63, 0x72, 0x61, 0x66, 0x74, 0x65, 0x72, 0x73, 0x02, 0x69, 0x6F, 0x00, 0x00, 0x01, 0x04, 0xD2
        };

        // parse the message 
        var input = new BitArray(receivedData);
        var dnsMessage = Parser.ParseDnsMessage(input);

        Assert.That(dnsMessage.Header.AuthorityCount, Is.EqualTo(1234));
    }


    [Test]
    public void TestFullParseQuestionCount()
    {
        // these are the exact same bytes as the request, except the identifier which is different each time.
        var receivedData = new byte[]
        {
            0x06, 0xA0, 0x01, 0x20, 0x04, 0xD2, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00,
            0x0C, 0x63, 0x6F, 0x64, 0x65, 0x63, 0x72, 0x61, 0x66, 0x74, 0x65, 0x72, 0x73, 0x02, 0x69, 0x6F, 0x00, 0x00,
            0x01, 0x04, 0xD2
        };

        // parse the message 
        var input = new BitArray(receivedData);
        var dnsMessage = Parser.ParseDnsMessage(input);

        Assert.That(dnsMessage.Header.QuestionCount, Is.EqualTo(1234));
    }


    [Test]
    public void TestFullParseAnswerCount()
    {
        // these are the exact same bytes as the request, except the identifier which is different each time.
        var receivedData = new byte[]
        {
            0x06, 0xA0, 0x01, 0x20, 0x00, 0x01, 0x04, 0xD2, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x63, 0x6F, 0x64, 0x65,
            0x63, 0x72, 0x61, 0x66, 0x74, 0x65, 0x72, 0x73, 0x02, 0x69, 0x6F, 0x00, 0x00, 0x01, 0x04, 0xD2
        };

        // parse the message 
        var input = new BitArray(receivedData);
        var dnsMessage = Parser.ParseDnsMessage(input);

        Assert.That(dnsMessage.Header.AnswerCount, Is.EqualTo(1234));
    }

    [Test]
    public void TestIdentifier1234()
    {
        // these are the exact same bytes as the request, except the identifier which is different each time.
        var receivedData = new byte[]
        {
            0x04, 0xD2, 0x01, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x63, 0x6F, 0x64, 0x65,
            0x63, 0x72, 0x61, 0x66, 0x74, 0x65, 0x72, 0x73, 0x02, 0x69, 0x6F, 0x00, 0x00, 0x01, 0x00, 0x01
        };


        // parse the message 
        var input = new BitArray(receivedData);
        var dnsMessage = Parser.ParseDnsMessage(input);

        Assert.That(dnsMessage.Header.Identifier, Is.EqualTo(1234));
    }

    [Test]
    public void TestIdentifierParseBack()
    {
        // these are the exact same bytes as the request, except the identifier which is different each time.
        var inputBytes = new byte[]
        {
            0x04, 0xD2, 0x01, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x63, 0x6F, 0x64, 0x65,
            0x63, 0x72, 0x61, 0x66, 0x74, 0x65, 0x72, 0x73, 0x02, 0x69, 0x6F, 0x00, 0x00, 0x01, 0x00, 0x01
        };

        // parse the message 
        var inputBits = new BitArray(inputBytes);
        var dnsMessage = Parser.ParseDnsMessage(inputBits);
        Assert.That(dnsMessage.Header.Identifier, Is.EqualTo(1234));

        var outputBytes = dnsMessage.ToBytesBigEndian();
        var outputBits = new BitArray(outputBytes);

        var output2 = Parser.ParseDnsMessage(outputBits);
        Assert.That(output2.Header.Identifier, Is.EqualTo(1234));
    }

    [Test]
    public void TestIsResponseTrue()
    {
        // construct message with response set to true.
        var header = new Header
        {
            IsResponse = true
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);
        Assert.That(bits[23], Is.True);
    }

    [Test]
    public void TestIsResponseFalse()
    {
        // construct message with response set to true.
        var header = new Header
        {
            IsResponse = false
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);
        Assert.That(bits[23], Is.False);
    }

    [Test]
    public void TestOpCodeAllTrue()
    {
        // construct message with response set to true.
        var header = new Header
        {
            OperationCode = 0xF // 1111
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);
        Assert.That(bits[19], Is.True);
        Assert.That(bits[20], Is.True);
        Assert.That(bits[21], Is.True);
        Assert.That(bits[22], Is.True);
    }

    [Test]
    public void TestOpCodeAllFalse()
    {
        // construct message with response set to true.
        var header = new Header
        {
            OperationCode = 0x0 // 000
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);
        Assert.That(bits[19], Is.False);
        Assert.That(bits[20], Is.False);
        Assert.That(bits[21], Is.False);
        Assert.That(bits[22], Is.False);
    }

    [Test]
    public void TestOpCodeAllFalseAndTrue()
    {
        // construct message with response set to true.
        var header = new Header
        {
            OperationCode = 0x9 // 1001
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);

        Assert.That(bits[19], Is.True);
        Assert.That(bits[20], Is.False);
        Assert.That(bits[21], Is.False);
        Assert.That(bits[22], Is.True);
    }

    [Test]
    public void TestAuthorativeAnswerTrue()
    {
        // construct message with response set to true.
        var header = new Header
        {
            AuthoritativeAnswer = true
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);

        Util.PrintHex(bytes, "bits");
        Assert.That(bits[18], Is.True);
    }

    [Test]
    public void TestAuthorativeAnswerFalse()
    {
        // construct message with response set to true.
        var header = new Header
        {
            AuthoritativeAnswer = false
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);

        Assert.That(bits[18], Is.False);
    }

    [Test]
    public void TestTruncatedFalse()
    {
        // construct message with response set to true.
        var header = new Header
        {
            Truncated = false
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);

        Assert.That(bits[17], Is.False);
    }

    [Test]
    public void TestTruncatedTrue()
    {
        // construct message with response set to true.
        var header = new Header
        {
            Truncated = true
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);

        Assert.That(bits[17], Is.True);
    }

    [Test]
    public void TestRecursionDesiredTrue()
    {
        // construct message with response set to true.
        var header = new Header
        {
            RecursionDesired = true
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);

        Assert.That(bits[16], Is.True);
    }

    [Test]
    public void TestRecursionDesiredFalse()
    {
        // construct message with response set to true.
        var header = new Header
        {
            RecursionDesired = false
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);

        Assert.That(bits[16], Is.False);
    }

    [Test]
    public void TestRecursionAvailableTrue()
    {
        // construct message with response set to true.
        var header = new Header
        {
            RecursionAvailable = true
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);

        Assert.That(bits[31], Is.True);
    }

    [Test]
    public void TestRecursionAvailableFalse()
    {
        // construct message with response set to true.
        var header = new Header
        {
            RecursionAvailable = false
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);

        Assert.That(bits[31], Is.False);
    }

    [Test]
    public void TestReservedTrue()
    {
        // construct message with response set to true.
        var header = new Header
        {
            Reserved = 0x7
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);
        Assert.That(bits[28], Is.True);
        Assert.That(bits[29], Is.True);
        Assert.That(bits[30], Is.True);
    }

    [Test]
    public void TestReservedFalse()
    {
        // construct message with response set to true.
        var header = new Header
        {
            Reserved = 0x0
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);
        Assert.That(bits[28], Is.False);
        Assert.That(bits[29], Is.False);
        Assert.That(bits[30], Is.False);
    }

    [Test]
    public void TestResponseCodeTrue()
    {
        // construct message with response set to true.
        var header = new Header
        {
            ResponseCode = 0xF // 1111
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);
        Assert.That(bits[24], Is.True);
        Assert.That(bits[25], Is.True);
        Assert.That(bits[26], Is.True);
        Assert.That(bits[27], Is.True);
    }

    [Test]
    public void TestResponseCodeFalse()
    {
        // construct message with response set to true.
        var header = new Header
        {
            ResponseCode = 0x0 // 1111
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytes();
        var bits = new BitArray(bytes);
        Assert.That(bits[24], Is.False);
        Assert.That(bits[25], Is.False);
        Assert.That(bits[26], Is.False);
        Assert.That(bits[27], Is.False);
    }
}