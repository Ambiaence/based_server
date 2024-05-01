using System.Net;
using System.Net.Sockets;
using System.Text;

IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("localhost");
IPAddress ipAddress = ipHostInfo.AddressList[0];

IPEndPoint ipEndPoint = new(ipAddress, 5000);

using Socket listener = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.IP);


listener.Bind(ipEndPoint);
listener.Listen(100);

var handler = await listener.AcceptAsync();

while(true)
{
    var buffer = new byte[1_024];
    var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
    var response = System.Text.Encoding.UTF8.GetString(buffer, 0, received);

    var eom = "<|EOM|>";
    if (response.IndexOf(eom) > -1) {
        Console.WriteLine("Message Received", response);

        var ackMessage = "<|ACK|>";

        var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
        await handler.SendAsync(echoBytes, 0);
        Console.WriteLine("Server Sent Ack");
        break;
    }
}

