using System.Collections;
using codecrafters_dns_server.DnsMessages;
using NUnit.Framework;

namespace codecrafters_dns_server.Tests.CodeCraftersTests;

public class Stage7
{
    [Test]
    public void TestQuestions()
    {
        var bytesIn = new byte[]
        {
            0xF8, 0x7A, 0x01, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x03, 0x61, 0x62, 0x63,
            0x11, 0x6C, 0x6F, 0x6E, 0x67, 0x61, 0x73, 0x73, 0x64, 0x6F, 0x6D, 0x61, 0x69, 0x6E, 0x6E, 0x61, 0x6D, 0x65,
            0x03, 0x63, 0x6F, 0x6D,
            0x00,
            0x00, 0x01,
            0x00, 0x01,
            0x03, 0x64, 0x65, 0x66, 0xC0, 0x10, 0x00, 0x01, 0x00, 0x01
        };

        var dnsMessageRequest = DnsMessage.Parse(new BitArray(bytesIn));

        Assert.That(dnsMessageRequest.Header.QuestionCount, Is.EqualTo(2));
        Assert.That(dnsMessageRequest.Questions[0].Name, Is.EqualTo("abc.longassdomainname.com"));
        Assert.That(dnsMessageRequest.Questions[1].Name, Is.EqualTo("def.longassdomainname.com"));

        dnsMessageRequest.Header.QueryResponseIndicator = true;
        dnsMessageRequest.Header.AuthoritativeAnswer = false;
        dnsMessageRequest.Header.Truncation = false;
        dnsMessageRequest.Header.RecursionAvailable = false;
        dnsMessageRequest.Header.ResponseCode = 4;
        dnsMessageRequest.Header.AnswerRecordCount = 1;

        // requested domain 
        foreach (var question in dnsMessageRequest.Questions)
            dnsMessageRequest.Answers.Add(new Answer(question.Name, 1, 1, 60, 4,
                new byte[] { 0x08, 0x08, 0x08, 0x08 }));

        Assert.That(dnsMessageRequest.Answers.Count, Is.EqualTo(2));
    }

    [Test]
    public void TestResponse()
    {
        var bytesIn = new byte[]
        {
            0xF8, 0x7A, 0x01, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x03, 0x61, 0x62, 0x63,
            0x11, 0x6C, 0x6F, 0x6E, 0x67, 0x61, 0x73, 0x73, 0x64, 0x6F, 0x6D, 0x61, 0x69, 0x6E, 0x6E, 0x61, 0x6D, 0x65,
            0x03, 0x63, 0x6F, 0x6D,
            0x00,
            0x00, 0x01,
            0x00, 0x01,
            0x03, 0x64, 0x65, 0x66, 0xC0, 0x10, 0x00, 0x01, 0x00, 0x01
        };

        var dnsMessageRequest = DnsMessage.Parse(new BitArray(bytesIn));
        dnsMessageRequest.Header.QueryResponseIndicator = true;
        dnsMessageRequest.Header.AuthoritativeAnswer = false;
        dnsMessageRequest.Header.Truncation = false;
        dnsMessageRequest.Header.RecursionAvailable = false;
        dnsMessageRequest.Header.ResponseCode = 4;
        dnsMessageRequest.Header.AnswerRecordCount = 2;

        // requested domain 
        foreach (var question in dnsMessageRequest.Questions)
            dnsMessageRequest.Answers.Add(new Answer(question.Name, 1, 1, 60, 4,
                new byte[] { 0x08, 0x08, 0x08, 0x08 }));


        var bytesOut = dnsMessageRequest.ToBytes();

        Util.PrintHex(bytesOut, "out");

        var dnsMessageResponse = DnsMessage.Parse(new BitArray(bytesOut));
    }

    // private var bytes = new byte[]
    // {
    //     0xF8, 0x7A, 0x81, 0x04, 0x00, 0x02, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00,
    //     // q1
    //     0x03, 0x61, 0x62, 0x63,
    //     0x11, 0x6C, 0x6F, 0x6E, 0x67, 0x61, 0x73, 0x73, 0x64, 0x6F, 0x6D, 0x61, 0x69, 0x6E, 0x6E, 0x61, 0x6D, 0x65,
    //     0x03, 0x63, 0x6F, 0x6D,
    //     0x00,
    //     0x00, 0x01,
    //     0x00, 0x01,
    //     // q2
    //     0x03, 0x64, 0x65, 0x66,
    //     0x11, 0x6C, 0x6F, 0x6E, 0x67, 0x61, 0x73, 0x73, 0x64, 0x6F, 0x6D, 0x61, 0x69, 0x6E, 0x6E, 0x61, 0x6D, 0x65,
    //     0x03, 0x63, 0x6F, 0x6D,
    //     0x00,
    //     0x00, 0x01,
    //     0x00, 0x01,
    //     // a1
    //     0x03, 0x61, 0x62, 0x63,
    //     0x11, 0x6C, 0x6F, 0x6E, 0x67, 0x61, 0x73, 0x73, 0x64, 0x6F, 0x6D, 0x61, 0x69, 0x6E, 0x6E, 0x61, 0x6D, 0x65,
    //     0x03, 0x63, 0x6F, 0x6D,
    //     0x00,
    //     0x00, 0x01,
    //     0x00, 0x01,
    //     0x00, 0x00, 0x00, 0x3C,
    //     0x00, 0x04,
    //     0x08, 0x08, 0x08, 0x08,
    //     // a2
    //     0x03, 0x64, 0x65, 0x66,
    //     0x11, 0x6C, 0x6F, 0x6E, 0x67, 0x61, 0x73, 0x73, 0x64, 0x6F, 0x6D, 0x61, 0x69, 0x6E, 0x6E, 0x61, 0x6D, 0x65,
    //     0x03, 0x63, 0x6F, 0x6D,
    //     0x00,
    //     0x00, 0x01,
    //     0x00, 0x01,
    //     0x00, 0x00, 0x00, 0x3C,
    //     0x00, 0x04,
    //     0x08, 0x08, 0x08, 0x08
    // };
}