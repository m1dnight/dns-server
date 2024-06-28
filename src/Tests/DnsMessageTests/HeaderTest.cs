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
            0xf6, 0x08, 0x81, 0x80, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x06, 0x67, 0x6f, 0x6f,
            0x67, 0x6c, 0x65, 0x03, 0x63, 0x6f, 0x6d, 0x00, 0x00, 0x01, 0x00, 0x01, 0xc0, 0x0c, 0x00, 0x01,
            0x00, 0x01, 0x00, 0x00, 0x01, 0x2c, 0x00, 0x04, 0x8e, 0xfa, 0x4b, 0xee
        ];

        BitsInResponse = new BitArray(BytesInResponse);
        HeaderBytesResponse = Header.Parse(BitsInResponse);

        BytesInRequest =
        [
            0x18, 0x2d, 0x01, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x76, 0x75, 0x62,
            0x02, 0x61, 0x63, 0x02, 0x62, 0x65, 0x03, 0x63, 0x6f, 0x6d, 0x00, 0x00, 0x01, 0x00, 0x01
        ];

        BitsInRequest = new BitArray(BytesInRequest);
        HeaderBytesRequest = Header.Parse(BitsInRequest);
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Identifier
    [Test]
    public void IdentifierTest()
    {
        Assert.That(HeaderBytesResponse.Identifier, Is.EqualTo(62984));
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
    /// Truncation
    [Test]
    public void Truncation()
    {
        Assert.That(HeaderBytesResponse.Truncation, Is.EqualTo(false));
        Assert.That(HeaderBytesResponse.Truncation, Is.EqualTo(false));
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// RecursionDesired
    [Test]
    public void RecursionDesired()
    {
        Assert.That(HeaderBytesResponse.RecursionDesired, Is.EqualTo(true));
        Assert.That(HeaderBytesResponse.RecursionDesired, Is.EqualTo(true));
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// RecursionAvailable
    [Test]
    public void RecursionAvailable()
    {
        Assert.That(HeaderBytesResponse.RecursionAvailable, Is.EqualTo(true));
        Assert.That(HeaderBytesResponse.RecursionAvailable, Is.EqualTo(true));
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Reserved
    [Test]
    public void Reserved()
    {
        Assert.That(HeaderBytesResponse.Reserved, Is.EqualTo(0));
        // Assert.That(HeaderBytesResponse.Reserved, Is.EqualTo(true));
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// ResponseCode
    [Test]
    public void ResponseCode()
    {
        Assert.That(HeaderBytesResponse.ResponseCode, Is.EqualTo(0));
        // Assert.That(HeaderBytesResponse.ResponseCode, Is.EqualTo(true));
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// QuestionCount
    [Test]
    public void QuestionCount()
    {
        Assert.That(HeaderBytesResponse.QuestionCount, Is.EqualTo(1));
        // Assert.That(HeaderBytesResponse.QuestionCount, Is.EqualTo(true));
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// AnswerRecordCount
    [Test]
    public void AnswerRecordCount()
    {
        Assert.That(HeaderBytesResponse.AnswerRecordCount, Is.EqualTo(1));
        // Assert.That(HeaderBytesResponse.AnswerRecordCount, Is.EqualTo(true));
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// AuthorityRecordCount
    [Test]
    public void AuthorityRecordCount()
    {
        Assert.That(HeaderBytesResponse.AuthorityRecordCount, Is.EqualTo(0));
        // Assert.That(HeaderBytesResponse.AuthorityRecordCount, Is.EqualTo(true));
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// AdditionalRecordCount
    [Test]
    public void AdditionalRecordCount()
    {
        Assert.That(HeaderBytesResponse.AdditionalRecordCount, Is.EqualTo(0));
        // Assert.That(HeaderBytesResponse.AdditionalRecordCount, Is.EqualTo(true));
    }
}