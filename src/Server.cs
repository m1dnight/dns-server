using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_dns_server;

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
            var dnsMessage = Parser.ParseDnsMessage(input);


            // Create an empty response
            var output = dnsMessage.ToBytes();

            // Send response
            var outputString = BitConverter.ToString(output);
            Console.WriteLine($"Sent     {output.Length} bytes to   {sourceEndPoint}: {outputString}");
            udpClient.Send(output, output.Length, sourceEndPoint);
        }
    }
}