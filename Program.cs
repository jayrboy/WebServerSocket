using Fleck;

var server = new WebSocketServer("ws://0.0.0.0:8181");
var wsConnections = new Dictionary<IWebSocketConnection, string>();

server.Start(ws =>
{
    ws.OnOpen = () =>
    {
        string name = "";
        wsConnections.Add(ws, name);
    };

    ws.OnMessage = (message) =>
    {
        if (string.IsNullOrEmpty(wsConnections[ws]))
        {
            // ตั้งชื่อผู้ใช้จากข้อความแรก
            wsConnections[ws] = message;
            Broadcast($"{message} joined the room.", wsConnections);
        }
        else
        {
            var userName = wsConnections[ws];
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var formattedMessage = $"{timestamp} - {userName}: {message}";
            Broadcast(formattedMessage, wsConnections);
        }
    };

    ws.OnClose = () =>
    {
        if (wsConnections.ContainsKey(ws))
        {
            var userName = wsConnections[ws];
            wsConnections.Remove(ws);
            Broadcast($"{userName} left the room.", wsConnections);
        }
    };
});

void Broadcast(string message, Dictionary<IWebSocketConnection, string> connections)
{
    foreach (var connection in connections.Keys)
    {
        if (connection.IsAvailable)
        {
            connection.Send(message);
        }
    }
}

WebApplication.CreateBuilder(args).Build().Run();
