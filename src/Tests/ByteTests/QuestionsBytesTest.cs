using System.Collections;
using codecrafters_dns_server.DnsMessages;
using NUnit.Framework;

namespace codecrafters_dns_server.Tests.ByteTests;

public class QuestionBytesTests
{
    // Request 

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

    // 0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                      ID                       |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                    QDCOUNT                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                    ANCOUNT                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                    NSCOUNT                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                    ARCOUNT                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        // BytesInResponse =
        // [
        //     0xFF, 0xFF,
        //     0xFF, 0xFF,
        //     0xFF, 0xFF,
        //     0xFF, 0xFF,
        //     0xFF, 0xFF,
        //     0xFF, 0xFF,
        //     0x81, 0x80,
        //     0x00, 0x00,
        //     0x00, 0x01,
        //     0x00, 0x00,
        //     0x00, 0x00,
        //     0x00, 0x00,
        //     0x00, 0x00
        // ];
        //
        // BitsInResponse = new BitArray(BytesInResponse);
        // DnsMessageResponse = DnsMessage.Parse(BitsInResponse);

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
    /// Question
    /// 
    [Test]
    public void ByteEqualLength()
    {
        Util.PrintHex(BytesInRequest, "IN ");
        Util.PrintHex(BytesOutRequest, "OUT");
        Assert.That(BytesInRequest.Length, Is.EqualTo(BytesOutRequest.Length));
        // Assert.That(BytesInResponse.Length, Is.EqualTo(BytesOutResponse.Length));
    }

    [Test]
    public void ByteEquivalent()
    {
        for (var i = 0; i < BytesInRequest.Length; i++)
            Assert.That(BytesInRequest[i], Is.EqualTo(BytesOutRequest[i]), "Byte " + i + " in Request");

        // for (var i = 0; i < BytesInResponse.Length; i++)
        //     Assert.That(BytesInResponse[i], Is.EqualTo(BytesOutResponse[i]), "Byte " + i + " in Request");
    }
}