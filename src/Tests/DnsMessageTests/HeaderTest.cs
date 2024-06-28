using System.Collections;
using codecrafters_dns_server.DnsMessages;
using NUnit.Framework;

namespace codecrafters_dns_server.Tests.DnsMessageTests;

public class HeaderTest
{
    private Header HeaderBytesResponse { set; get; }
    private Header HeaderBytesRequest { set; get; }

    private byte[] BytesInResponse { set; get; }
    private byte[] BytesInRequest { set; get; }
    public BitArray BitsInResponse { set; get; }
    public BitArray BitsInRequest { set; get; }

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
            0x00, 0x00,
            0x00, 0x00,
            0x00, 0x00
        ];

        BitsInResponse = new BitArray(BytesInResponse);
        HeaderBytesResponse = Header.Parse(BitsInResponse);

        BytesInRequest =
        [
            0x18, 0x2d,
            0x01, 0x20,
            0x00, 0x01,
            0x00, 0x00,
            0x00, 0x00,
            0x00, 0x00,
            0x03, 0x76,
            0x75, 0x62,
            0x02, 0x61,
            0x63, 0x02,
            0x62, 0x65,
            0x03, 0x63,
            0x6f, 0x6d,
            0x00, 0x00,
            0x01, 0x00,
            0x01
        ];

        BitsInRequest = new BitArray(BytesInRequest);
        HeaderBytesRequest = Header.Parse(BitsInRequest);
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Identifier
    [Test]
    public void IdentifierTest()
    {
        Assert.That(HeaderBytesResponse.Identifier, Is.EqualTo(50098));
        Assert.That(HeaderBytesRequest.Identifier, Is.EqualTo(6189));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// QueryResponseIndicator
    ///
    [Test]
    public void QueryResponseIndicator()
    {
        Assert.That(HeaderBytesResponse.QueryResponseIndicator, Is.EqualTo(true));
        Assert.That(HeaderBytesRequest.QueryResponseIndicator, Is.EqualTo(false));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// OperationCode
    [Test]
    public void OperationCode()
    {
        Assert.That(HeaderBytesResponse.OperationCode, Is.EqualTo(0));
        Assert.That(HeaderBytesResponse.OperationCode, Is.EqualTo(0));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// AuthoritativeAnswer
    [Test]
    public void AuthoritativeAnswer()
    {
        Assert.That(HeaderBytesResponse.AuthoritativeAnswer, Is.EqualTo(false));
        Assert.That(HeaderBytesResponse.AuthoritativeAnswer, Is.EqualTo(false));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// AuthoritativeAnswer
    [Test]
    public void Truncation()
    {
        Assert.That(HeaderBytesResponse.Truncation, Is.EqualTo(false));
        Assert.That(HeaderBytesResponse.Truncation, Is.EqualTo(false));
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// AuthoritativeAnswer
    [Test]
    public void RecursionDesired()
    {
        Assert.That(HeaderBytesResponse.RecursionDesired, Is.EqualTo(true));
        Assert.That(HeaderBytesResponse.RecursionDesired, Is.EqualTo(true));
    }
}