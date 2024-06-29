using System.Collections;
using codecrafters_dns_server.DnsMessages;
using NUnit.Framework;

namespace codecrafters_dns_server.Tests.DnsMessageTests;

public class CompressionTest
{
    private DnsMessage DnsMessageRequest { set; get; }
    private byte[] BytesInRequest { set; get; }
    private BitArray BitsInRequest { set; get; }

    private byte[] BytesOutRequest { set; get; }
    private BitArray BitsOutRequest { set; get; }


    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        // Request 
        BytesInRequest =
        [
            // header 
            0x8E, 0x97, 0x01, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            // q1 name (abc.longassdomainname.com)
            0x03, 0x61, 0x62, 0x63, 0x11, 0x6C, 0x6F, 0x6E, 0x67, 0x61, 0x73, 0x73, 0x64, 0x6F, 0x6D, 0x61, 0x69, 0x6E,
            0x6E, 0x61, 0x6D, 0x65, 0x03, 0x63, 0x6F, 0x6D, 0x00,
            // q1 type
            0x00, 0x01,
            // q1 class 
            0x00, 0x01,
            // q2 name def
            0x03, 0x64, 0x65, 0x66,
            // q2 pointer
            0xC0, 0x10,
            // q2 type 
            0x00, 0x01,
            // q2 class
            0x00, 0x01
        ];

        BitsInRequest = new BitArray(BytesInRequest);
        DnsMessageRequest = DnsMessage.Parse(BitsInRequest);

        BytesOutRequest = DnsMessageRequest.ToBytes();
        BitsOutRequest = new BitArray(BytesOutRequest);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Questions
    [Test]
    public void TestQuestions()
    {
        Assert.That(DnsMessageRequest.Header.QuestionCount, Is.EqualTo(2));
        Assert.That(DnsMessageRequest.Questions[0].Name, Is.EqualTo("abc.longassdomainname.com"));
        Assert.That(DnsMessageRequest.Questions[1].Name, Is.EqualTo("def.longassdomainname.com"));
    }
}