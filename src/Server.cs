using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_dns_server;
using codecrafters_dns_server.DnsMessages;

internal class Program
{
    private static void Main()
    {
        Console.WriteLine("Logs from your program will appear here!");

        var ipAddress = IPAddress.Parse("127.0.0.1");
        var port = 2053;
        var udpEndPoint = new IPEndPoint(ipAddress, port);

        var udpClient = new UdpClient(udpEndPoint);

        while (true)
        {
            // Receive data
            var sourceEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var receivedData = udpClient.Receive(ref sourceEndPoint);

            // print out bytes 
            var receivedString = BitConverter.ToString(receivedData);
            Console.WriteLine($"Received {receivedData.Length} bytes from {sourceEndPoint}: {receivedString}");


            // parse the message 
            var input = new BitArray(receivedData);
            var dnsMessage = DnsMessage.Parse(input);

            dnsMessage.Header.QueryResponseIndicator = true;
            dnsMessage.Header.AuthoritativeAnswer = false;
            dnsMessage.Header.Truncation = false;
            dnsMessage.Header.RecursionAvailable = false;
            dnsMessage.Header.ResponseCode = 4;
            dnsMessage.Header.AnswerRecordCount = 1;

            // requested domain 
            var domain = dnsMessage.Questions[0].Name;
            dnsMessage.Answers = new List<Answer>
            {
                new(domain, 1, 1, 60, 4, new byte[] { 0x08, 0x08, 0x08, 0x08 })
            };


            // Send response
            var output = dnsMessage.ToBytes();

            Util.PrintHex(receivedData, "input");
            Util.PrintHex(output, "output");

            var outputString = BitConverter.ToString(output);
            Console.WriteLine($"Sent     {output.Length} bytes to   {sourceEndPoint}: {outputString}");
            udpClient.Send(output, output.Length, sourceEndPoint);
        }
    }

    private static DnsMessage CreateDnsMessage()
    {
        var header = new Header
        {
            Identifier = 1234,
            QueryResponseIndicator = true,
            OperationCode = 0,
            AuthoritativeAnswer = false,
            Truncation = false,
            RecursionDesired = false,
            RecursionAvailable = false,
            Reserved = 0,
            ResponseCode = 0,
            QuestionCount = 1,
            AnswerRecordCount = 1,
            AuthorityRecordCount = 0,
            AdditionalRecordCount = 0
        };

        var question = new Question("codecrafters.io", 1, 1);
        var answer = new Answer("codecrafters.io", 1, 1, 60, 4, new byte[] { 0x08, 0x08, 0x08, 0x08 });

        return new DnsMessage(header, new List<Question> { question }, new List<Answer> { answer });
    }
}