using System.Collections;
using NUnit.Framework;

namespace codecrafters_dns_server;

public class QuestionTest
{
    [Test]
    public void TestQuestion()
    {
        var inputBytes = new byte[]
        {
            0x4b, 0x64, 0x01, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x67, 0x6f, 0x6f, 0x67, 0x6c,
            0x65, 0x03, 0x63, 0x6f, 0x6d, 0x00, 0x00, 0x01, 0x00, 0x01
        };
        var inputBits = new BitArray(inputBytes);

        // Parse the message
        var dnsMessage = Parser.ParseDnsMessage(inputBits);

        Assert.That(dnsMessage.Questions.Count, Is.EqualTo(1));
        var question = dnsMessage.Questions[0];
        Assert.That(question.Name, Is.EqualTo("google.com"));
    }

    [Test]
    public void TestBytes()
    {
        var inputBytes = new byte[]
        {
            0x4b, 0x64, 0x01, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x67, 0x6f, 0x6f, 0x67, 0x6c,
            0x65, 0x03, 0x63, 0x6f, 0x6d, 0x00, 0x00, 0x01, 0x00, 0x01
        };
        var inputBits = new BitArray(inputBytes);

        // Parse the message
        var dnsMessage = Parser.ParseDnsMessage(inputBits);

        var outputBytes = dnsMessage.ToBytesBigEndian();
        var outputBits = new BitArray(outputBytes);

        Util.PrintHex(inputBits, "input");
        Util.PrintHex(outputBits, "output");

        Assert.That(outputBits, Is.EqualTo(inputBits));
    }
}