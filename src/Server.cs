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

            Console.WriteLine("Identifier: " + dnsMessage.Header.Identifier);
            // settings for stage 2 
            dnsMessage.Header.Identifier = 1234;
            dnsMessage.Header.QueryResponseIndicator = true;
            dnsMessage.Header.OperationCode = 0;
            dnsMessage.Header.AuthoritativeAnswer = false;
            dnsMessage.Header.Truncation = false;
            dnsMessage.Header.RecursionDesired = false;
            dnsMessage.Header.RecursionAvailable = false;
            dnsMessage.Header.Reserved = 0;
            dnsMessage.Header.ResponseCode = 0;
            dnsMessage.Header.QuestionCount = 1;
            dnsMessage.Header.AnswerRecordCount = 0;
            dnsMessage.Header.AuthorityRecordCount = 0;
            dnsMessage.Header.AdditionalRecordCount = 0;

            var question = new Question("codecrafters.io", 1, 1);
            dnsMessage.Questions.Add(question);


            // Send response
            var output = dnsMessage.ToBytes();

            Util.PrintHex(new BitArray(output), "bitarray");
            Util.PrintHex(output, "bytes");

            var outputString = BitConverter.ToString(output);
            Console.WriteLine($"Sent     {output.Length} bytes to   {sourceEndPoint}: {outputString}");
            udpClient.Send(output, output.Length, sourceEndPoint);
        }
    }
}