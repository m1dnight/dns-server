// using System.Collections;
// using NUnit.Framework;
//
// namespace codecrafters_dns_server.Tests;
//
// public class PacketTest
// {
//     [Test]
//     public void ResponsePacket()
//     {
//         byte[] receivedData =
//         {
//             0x4B, 0x64, 0x81, 0x80, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
//         };
//         // parse the message 
//         var input = new BitArray(receivedData);
//
//         Assert.That(input[23], Is.True);
//         Util.PrintHex(input, " IN");
//         var dnsMessage = codecrafters_dns_server.Parser.ParseDnsMessage(input);
//
//         Assert.That(dnsMessage.Header.QueryResponseIndicator, Is.True);
//     }
//
//     [Test]
//     public void RequestPacket()
//     {
//         byte[] receivedData =
//         [
//             0x4B, 0x64, 0x01, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
//         ];
//         // parse the message 
//         var input = new BitArray(receivedData);
//
//         Assert.That(input[23], Is.False);
//
//         Util.PrintHex(input, " IN");
//         var dnsMessage = codecrafters_dns_server.Parser.ParseDnsMessage(input);
//
//         Assert.That(dnsMessage.Header.QueryResponseIndicator, Is.False);
//     }
//
//     [Test]
//     public void ParseBackResponsePacket()
//     {
//         byte[] bytesIn =
//         [
//             0x4B, 0x64, 0x81, 0x80, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
//         ];
//         // parse the message 
//         var bitsIn = new BitArray(bytesIn);
//
//
//         var dnsMessage = codecrafters_dns_server.Parser.ParseDnsMessage(bitsIn);
//         var bytesOut = dnsMessage.ToBytesBigEndian();
//         var bitsOut = new BitArray(bytesOut);
//
//         Assert.That(bitsIn[23], Is.True);
//         Assert.That(bitsOut[23], Is.True);
//
//
//         Util.PrintHex(bytesIn, " IN");
//         Util.PrintHex(bytesOut, "OUT");
//         Util.PrintBinary(bytesIn, "IN");
//         Util.PrintBinary(bytesOut, "OUT");
//
//         var mismatches = new List<int>();
//         for (var i = 0; i < 8 * 12; i++)
//             if (bitsIn[i] != bitsOut[i])
//                 mismatches.Add(i);
//
//         Assert.That(mismatches, Is.Empty);
//     }
// }

