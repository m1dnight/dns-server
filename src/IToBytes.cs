namespace codecrafters_dns_server;

public interface IToBytes
{
    byte[] ToBytes();
    byte[] ToBytesBigEndian();
}