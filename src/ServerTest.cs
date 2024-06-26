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
    public void TestIdentifier()
    {
        // these are the exact same bytes as the request, except the identifier which is different each time.
        var receivedData = new byte[]
        {
            0x06, 0xA0, 0x01, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x63, 0x6F, 0x64, 0x65,
            0x63, 0x72, 0x61, 0x66, 0x74, 0x65, 0x72, 0x73, 0x02, 0x69, 0x6F, 0x00, 0x00, 0x01, 0x00, 0x01
        };

        // parse the message 
        var input = new BitArray(receivedData);
        var dnsMessage = Parser.ParseDnsMessage(input);


        // check that the identifier is the same in the sent data 
        var output = dnsMessage.ToBytesBigEndian();
        var sentData = new BitArray(output);
        for (var i = 0; i < 16; i++) Assert.That(input[i], Is.EqualTo(sentData[i]));
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
        var bytes = dnsMessage.ToBytesBigEndian();
        var bits = new BitArray(bytes);
        Assert.That(bits[16], Is.True);
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
        var bytes = dnsMessage.ToBytesBigEndian();
        var bits = new BitArray(bytes);
        Assert.That(bits[16], Is.False);
    }

    [Test]
    public void TestOpCodeAllTrue()
    {
        // construct message with response set to true.
        var header = new Header
        {
            OperationCode = 0x7 // 111
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytesBigEndian();
        var bits = new BitArray(bytes);
        Assert.That(bits[17], Is.True);
        Assert.That(bits[18], Is.True);
        Assert.That(bits[19], Is.True);
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
        var bytes = dnsMessage.ToBytesBigEndian();
        var bits = new BitArray(bytes);
        Assert.That(bits[17], Is.False);
        Assert.That(bits[18], Is.False);
        Assert.That(bits[19], Is.False);
    }

    [Test]
    public void TestOpCodeAllFalseAndTrue()
    {
        // construct message with response set to true.
        var header = new Header
        {
            OperationCode = 0x5 // 000
        };
        var dnsMessage = new DnsMessage(header);

        // check the bit in the bytes.
        var bytes = dnsMessage.ToBytesBigEndian();
        var bits = new BitArray(bytes);

        Assert.That(bits[17], Is.True);
        Assert.That(bits[18], Is.False);
        Assert.That(bits[19], Is.True);
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
        var bytes = dnsMessage.ToBytesBigEndian();
        var bits = new BitArray(bytes);

        Util.PrintHex(bytes, "bits");
        Assert.That(bits[21], Is.True);
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
        var bytes = dnsMessage.ToBytesBigEndian();
        var bits = new BitArray(bytes);

        Assert.That(bits[21], Is.False);
    }
}