using System.Collections;
using codecrafters_dns_server.DnsMessages;
using NUnit.Framework;

namespace codecrafters_dns_server.Tests.DnsMessageTests;

public class QuestionTest
{
    private DnsMessage DnsMessageRequest { set; get; }
    private byte[] BytesInRequest { set; get; }
    private BitArray BitsInRequest { set; get; }

    private byte[] BytesOutRequest { set; get; }
    private BitArray BitsOutRequest { set; get; }

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
            0x18, 0x2d, 0x81, 0x83, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x03, 0x76, 0x75, 0x62,
            0x02, 0x61, 0x63, 0x02, 0x62, 0x65, 0x03, 0x63, 0x6f, 0x6d, 0x00, 0x00, 0x01, 0x00, 0x01, 0xc0,
            0x13, 0x00, 0x06, 0x00, 0x01, 0x00, 0x00, 0x07, 0x08, 0x00, 0x2f, 0x04, 0x6d, 0x61, 0x74, 0x74,
            0x02, 0x6e, 0x73, 0x0a, 0x63, 0x6c, 0x6f, 0x75, 0x64, 0x66, 0x6c, 0x61, 0x72, 0x65, 0xc0, 0x16,
            0x03, 0x64, 0x6e, 0x73, 0xc0, 0x33, 0x8b, 0xa8, 0x4b, 0xaa, 0x00, 0x00, 0x27, 0x10, 0x00, 0x00,
            0x09, 0x60, 0x00, 0x09, 0x3a, 0x80, 0x00, 0x00, 0x07, 0x08
        ];

        BitsInResponse = new BitArray(BytesInResponse);
        DnsMessageResponse = DnsMessage.Parse(BitsInResponse);

        BytesOutResponse = DnsMessageResponse.ToBytes();
        BitsOutResponse = new BitArray(BytesOutResponse);

        // Request 
        BytesInRequest =
        [
            0x18, 0x2d, 0x01, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x76, 0x75, 0x62,
            0x02, 0x61, 0x63, 0x02, 0x62, 0x65, 0x03, 0x63, 0x6f, 0x6d, 0x00, 0x00, 0x01, 0x00, 0x01
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
        Assert.That(DnsMessageRequest.Questions.Count, Is.EqualTo(1));
        var question = DnsMessageRequest.Questions[0];
        Assert.That(question.Name, Is.EqualTo("vub.ac.be.com"));
    }


    [Test]
    public void TestTwoQuestions()
    {
        byte[] bytesInRequest =
        [
            0x18, 0x2d, 0x01, 0x20, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            
            //q1
            0x03, 0x76, 0x75, 0x62,
            0x02, 0x61, 0x63, 
            0x02, 0x62, 0x65, 
            0x03, 0x63, 0x6f, 0x6d, 
            0x00, 
            0x00, 0x01, 
            0x00, 0x01,
            // q2
            0x03, 0x76, 0x75, 0x62,
            0x02, 0x61, 0x63, 
            0x02, 0x62, 0x65, 
            0x03, 0x63, 0x6f, 0x6d, 
            0x00, 
            0x00, 0x01, 
            0x00, 0x01
            
        ];

        var bitsInRequest = new BitArray(bytesInRequest);
        var dnsMessageRequest = DnsMessage.Parse(bitsInRequest);

        var bytesOutRequest = dnsMessageRequest.ToBytes();
        var bitsOutRequest = new BitArray(bytesOutRequest);

        Assert.That(dnsMessageRequest.Header.QuestionCount, Is.EqualTo(2));
    }
}