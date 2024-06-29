[![progress-banner](https://backend.codecrafters.io/progress/dns-server/1947f5f4-171d-4d5e-9281-c5a1f97eb3ef)](https://app.codecrafters.io/users/codecrafters-bot?r=2qF)

# Create a test packet

## Request

```sh
nc -u -l 1053 > query_packet.txt
dig +retry=0 -p 1053 @127.0.0.1 +noedns vub.ac.be.com
hexdump -e '16/1 "0x%02x, " "\n"'  query_packet.txt
```

## Response

```sh
nc -u 8.8.8.8 53 < query_packet.txt > response_packet.txt
hexdump -e '16/1 "0x%02x, " "\n"' response_packet.txt
```

## Generate a request packet

```sh
echo -n -e '\xD0\xCD\x81\x04\x00\x01\x00\x01\x00\x00\x00\x00\x0C\x63\x6F\x64\x65\x63\x72\x61\x66\x74\x65\x72\x73\x02\x69\x6F\x00\x00\x01\x00\x01\x0C\x63\x6F\x64\x65\x63\x72\x61\x66\x74\x65\x72\x73\x02\x69\x6F\x00\x00\x01\x00\x01\x00\x00\x00\x3C\x00\x04\x08\x08\x08\x08' > custom_request.txt
nc -u 127.0.0.1 2053 < custom_request.txt
```

This is a starting point for C# solutions to the
["Build Your Own DNS server" Challenge](https://app.codecrafters.io/courses/dns-server/overview).

In this challenge, you'll build a DNS server that's capable of parsing and
creating DNS packets, responding to DNS queries, handling various record types
and doing recursive resolve. Along the way we'll learn about the DNS protocol,
DNS packet format, root servers, authoritative servers, forwarding servers,
various record types (A, AAAA, CNAME, etc) and more.

**Note**: If you're viewing this repo on GitHub, head over to
[codecrafters.io](https://codecrafters.io) to try the challenge.

# Passing the first stage

The entry point for your `your_server.sh` implementation is in `src/Server.cs`.
Study and uncomment the relevant code, and push your changes to pass the first
stage:

```sh
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```

Time to move on to the next stage!

# Stage 2 & beyond

Note: This section is for stages 2 and beyond.

1. Ensure you have `dotnet (8.0)` installed locally
1. Run `./your_server.sh` to run your program, which is implemented in
   `src/Server.cs`.
1. Commit your changes and run `git push origin master` to submit your solution
   to CodeCrafters. Test output will be streamed to your terminal.
