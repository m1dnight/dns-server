using System.Collections;
using codecrafters_dns_server.DnsMessages;
using NUnit.Framework;

namespace codecrafters_dns_server.Tests.DnsMessageTests;

public class AnswerTest
{
    // Response 
    private DnsMessage DnsMessageResponse { set; get; }
    private byte[] BytesInResponse { set; get; }
    private BitArray BitsInResponse { set; get; }
    private byte[] BytesOutResponse { set; get; }
    private BitArray BitsOutResponse { set; get; }

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        // Response 
        BytesInResponse =
        [
            0xf6, 0x08, 0x81, 0x80, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x06, 0x67, 0x6f, 0x6f,
            0x67, 0x6c, 0x65, 0x03, 0x63, 0x6f, 0x6d, 0x00, 0x00, 0x01, 0x00, 0x01, 0xc0, 0x0c, 0x00, 0x01,
            0x00, 0x01, 0x00, 0x00, 0x01, 0x2c, 0x00, 0x04, 0x8e, 0xfa, 0x4b, 0xee
        ];

        BitsInResponse = new BitArray(BytesInResponse);
        DnsMessageResponse = DnsMessage.Parse(BitsInResponse);

        BytesOutResponse = DnsMessageResponse.ToBytes();
        BitsOutResponse = new BitArray(BytesOutResponse);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Answers
    [Test]
    public void TestQuestions()
    {
        Assert.That(DnsMessageResponse.Header.QueryResponseIndicator, Is.EqualTo(true));
        Assert.That(DnsMessageResponse.Header.AnswerRecordCount, Is.EqualTo(1));
        var answer = DnsMessageResponse.Answers[0];
        Assert.That(answer.Name, Is.EqualTo("google.com"));
    }
}