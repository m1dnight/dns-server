using System.Collections;
using codecrafters_dns_server.DnsMessages;
using NUnit.Framework;

namespace codecrafters_dns_server.Tests.ByteTests;

public class AnswerBytesTest
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
        BytesInResponse =
        [
            // header
            0xf6, 0x08, 0x81, 0x80, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00,
            // answer name
            0x06, 0x67, 0x6f, 0x6f, 0x67, 0x6c, 0x65, 0x03, 0x63, 0x6f, 0x6d, 0x00,
            // type
            0x00, 0x01,
            // class
            0x00, 0x01,
            // ttl 
            0xc0, 0x0c, 0x00, 0x01,
            // rdlength 
            0x00, 0x01,
            // rdata 
            0x00
        ];

        BitsInResponse = new BitArray(BytesInResponse);
        DnsMessageResponse = DnsMessage.Parse(BitsInResponse);

        BytesOutResponse = DnsMessageResponse.ToBytes();
        BitsOutResponse = new BitArray(BytesOutResponse);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    [Test]
    public void ByteEqualLength()
    {
        Util.PrintHex(BytesInResponse, "IN ");
        Util.PrintHex(BytesOutResponse, "OUT");

        Assert.That(DnsMessageResponse.Header.AnswerRecordCount, Is.EqualTo(1));

        Assert.That(BytesInResponse.Length, Is.EqualTo(BytesOutResponse.Length));
    }
}