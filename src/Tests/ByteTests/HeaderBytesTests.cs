using System.Collections;
using codecrafters_dns_server.DnsMessages;
using NUnit.Framework;

namespace codecrafters_dns_server.Tests.ByteTests;

public class HeaderBytesTests
{
    // Request 

    private Header DnsMessageRequestHeader { set; get; }
    private byte[] BytesInRequest { set; get; }
    private BitArray BitsInRequest { set; get; }

    private byte[] BytesOutRequest { set; get; }
    private BitArray BitsOutRequest { set; get; }

    // Response 
    private Header DnsMessageResponseHeader { set; get; }
    private byte[] BytesInResponse { set; get; }
    private BitArray BitsInResponse { set; get; }
    private byte[] BytesOutResponse { set; get; }
    private BitArray BitsOutResponse { set; get; }


    // 1  1  1  1  1  1
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
    // 1  1  1  1  1  1
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
        // construct message with response set to true.
        BytesInResponse =
        [
            0xc3, 0xb2,
            0x81, 0x80,
            0x00, 0x00,
            0x00, 0x01,
            0x00, 0x00,
            0x00, 0x00
        ];

        BitsInResponse = new BitArray(BytesInResponse);
        DnsMessageResponseHeader = Header.Parse(BitsInResponse);

        BytesOutResponse = DnsMessageResponseHeader.ToBytes();
        BitsOutResponse = new BitArray(BytesOutResponse);

        BytesInRequest =
        [
            0x18, 0x2d,
            0x01, 0x20,
            0x00, 0x01,
            0x00, 0x00,
            0x00, 0x00,
            0x00, 0x00
        ];

        BitsInRequest = new BitArray(BytesInRequest);
        DnsMessageRequestHeader = Header.Parse(BitsInRequest);

        BytesOutRequest = DnsMessageRequestHeader.ToBytes();
        BitsOutRequest = new BitArray(BytesOutRequest);
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Identifier
    [Test]
    public void ByteEqualLength()
    {
        Assert.That(BytesInRequest.Length, Is.EqualTo(BytesOutRequest.Length));
        Assert.That(BytesInResponse.Length, Is.EqualTo(BytesOutResponse.Length));
    }

    [Test]
    public void ByteEquivalent()
    {
        for (var i = 0; i < BytesInRequest.Length; i++)
            Assert.That(BytesInRequest[i], Is.EqualTo(BytesOutRequest[i]), "Byte " + i + " in Request");

        for (var i = 0; i < BytesInResponse.Length; i++)
            Assert.That(BytesInResponse[i], Is.EqualTo(BytesOutResponse[i]), "Byte " + i + " in Request");
    }
}