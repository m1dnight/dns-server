using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
    var receivedString = Encoding.ASCII.GetString(receivedData);

    BitArray input = new BitArray(receivedData);
    Console.WriteLine($"Received {receivedData.Length} bytes from {sourceEndPoint}: {receivedString}");

    // Create an empty response
    var response = Encoding.ASCII.GetBytes("");

    // Send response
    udpClient.Send(response, response.Length, sourceEndPoint);
}