using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace HungDuyParkingBridge.Services
{
    public class WebSocketService
    {
        private readonly HttpListener _httpListener;
        private readonly ConcurrentBag<WebSocket> _connectedClients;
        private readonly string _prefix;
        private bool _isRunning;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public WebSocketService(string prefix = "http://localhost:5001/")
        {
            _httpListener = new HttpListener();
            _connectedClients = new ConcurrentBag<WebSocket>();
            _prefix = prefix;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync()
        {
            try
            {
                _httpListener.Prefixes.Add(_prefix);
                _httpListener.Start();
                _isRunning = true;

                Debug.WriteLine($"[WebSocket] Server started on {_prefix}");
                Debug.WriteLine("[WebSocket] Available endpoints:");
                Debug.WriteLine($"  WebSocket: ws://localhost:5001/ws");
                Debug.WriteLine($"  Status: http://localhost:5001/status");
                Debug.WriteLine($"  Test: http://localhost:5001/test");

                _ = Task.Run(async () => await AcceptConnectionsAsync(_cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WebSocket Error] Failed to start: {ex.Message}");
                throw;
            }
        }

        public async Task StopAsync()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
            
            // Close all connected clients
            foreach (var client in _connectedClients)
            {
                if (client.State == WebSocketState.Open)
                {
                    try
                    {
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server shutdown", CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[WebSocket] Error closing client: {ex.Message}");
                    }
                }
            }

            _httpListener.Stop();
            Debug.WriteLine("[WebSocket] Service stopped");
        }

        private async Task AcceptConnectionsAsync(CancellationToken cancellationToken)
        {
            while (_isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var context = await _httpListener.GetContextAsync();
                    
                    if (context.Request.IsWebSocketRequest)
                    {
                        _ = Task.Run(async () => await HandleWebSocketConnection(context));
                    }
                    else
                    {
                        await HandleHttpRequest(context);
                    }
                }
                catch (Exception ex) when (!(ex is ObjectDisposedException))
                {
                    Debug.WriteLine($"[WebSocket] Error accepting connections: {ex.Message}");
                }
            }
        }

        private async Task HandleWebSocketConnection(HttpListenerContext context)
        {
            WebSocket? webSocket = null;
            try
            {
                var webSocketContext = await context.AcceptWebSocketAsync(null);
                webSocket = webSocketContext.WebSocket;
                _connectedClients.Add(webSocket);

                Debug.WriteLine("[WebSocket] Client connected");

                // Send welcome message
                await SendToClient(webSocket, new
                {
                    type = "connected",
                    message = "Connected to HungDuy Parking Bridge WebSocket",
                    timestamp = DateTime.UtcNow,
                    server = "HungDuyParkingBridge v1.0.2"
                });

                // Notify all clients about new connection
                await BroadcastMessageAsync("User connected", "system");

                // Handle incoming messages
                await HandleClientMessages(webSocket);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WebSocket] Connection error: {ex.Message}");
            }
            finally
            {
                if (webSocket != null)
                {
                    Debug.WriteLine("[WebSocket] Client disconnected");
                    await BroadcastMessageAsync("User disconnected", "system");
                }
            }
        }

        private async Task HandleClientMessages(WebSocket webSocket)
        {
            var buffer = new byte[4096];
            
            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        await ProcessClientMessage(webSocket, message);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[WebSocket] Error handling client message: {ex.Message}");
                    break;
                }
            }
        }

        private async Task ProcessClientMessage(WebSocket sender, string message)
        {
            try
            {
                var messageObj = JsonSerializer.Deserialize<Dictionary<string, object>>(message);
                
                if (messageObj?.TryGetValue("type", out var typeObj) == true)
                {
                    var type = typeObj?.ToString();
                    
                    switch (type)
                    {
                        case "ping":
                            await SendToClient(sender, new { type = "pong", timestamp = DateTime.UtcNow });
                            break;
                            
                        case "message":
                            if (messageObj.TryGetValue("content", out var content))
                            {
                                await BroadcastMessageAsync(content?.ToString() ?? "", "user");
                            }
                            break;
                            
                        case "status":
                            await SendToClient(sender, new
                            {
                                type = "status",
                                server = "HungDuyParkingBridge",
                                version = "1.0.2",
                                connectedClients = _connectedClients.Count(c => c.State == WebSocketState.Open),
                                timestamp = DateTime.UtcNow
                            });
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WebSocket] Error processing message: {ex.Message}");
            }
        }

        private async Task HandleHttpRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            
            // Add CORS headers
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            response.AddHeader("Access-Control-Allow-Headers", "*");

            try
            {
                var path = request.Url?.AbsolutePath ?? "";
                
                switch (path.ToLower())
                {
                    case "/status":
                        await SendJsonResponse(response, new
                        {
                            status = "online",
                            server = "HungDuyParkingBridge WebSocket Server",
                            version = "1.0.2",
                            connectedClients = _connectedClients.Count(c => c.State == WebSocketState.Open),
                            timestamp = DateTime.UtcNow,
                            endpoints = new[]
                            {
                                "WebSocket: ws://localhost:5001/ws",
                                "Status: http://localhost:5001/status",
                                "Test: http://localhost:5001/test"
                            }
                        });
                        break;
                        
                    case "/test":
                        if (request.HttpMethod == "POST")
                        {
                            await BroadcastFileNotificationAsync("test-file.txt", "test", 1024);
                            await SendJsonResponse(response, new { message = "Test notification sent" });
                        }
                        else
                        {
                            await SendJsonResponse(response, new
                            {
                                message = "WebSocket Test Endpoint",
                                usage = "POST to this endpoint to send test notification"
                            });
                        }
                        break;
                        
                    case "/":
                    case "/ws":
                        // Return instructions for WebSocket connection
                        var html = GetWebSocketTestPage();
                        var htmlBytes = Encoding.UTF8.GetBytes(html);
                        response.ContentType = "text/html";
                        response.ContentLength64 = htmlBytes.Length;
                        await response.OutputStream.WriteAsync(htmlBytes);
                        break;
                        
                    default:
                        response.StatusCode = 404;
                        await SendJsonResponse(response, new { error = "Not found" });
                        break;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                await SendJsonResponse(response, new { error = ex.Message });
            }
            finally
            {
                response.Close();
            }
        }

        private async Task SendJsonResponse(HttpListenerResponse response, object data)
        {
            response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            var bytes = Encoding.UTF8.GetBytes(json);
            response.ContentLength64 = bytes.Length;
            await response.OutputStream.WriteAsync(bytes);
        }

        private async Task SendToClient(WebSocket client, object data)
        {
            if (client.State == WebSocketState.Open)
            {
                var json = JsonSerializer.Serialize(data);
                var bytes = Encoding.UTF8.GetBytes(json);
                await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public async Task BroadcastMessageAsync(string message, string sender = "server")
        {
            var data = new
            {
                type = "message",
                sender = sender,
                content = message,
                timestamp = DateTime.UtcNow
            };

            await BroadcastToAllClients(data);
        }

        public async Task BroadcastFileNotificationAsync(string fileName, string action, long fileSize)
        {
            var data = new
            {
                type = "fileNotification",
                fileName = fileName,
                action = action,
                fileSize = fileSize,
                timestamp = DateTime.UtcNow
            };

            await BroadcastToAllClients(data);
        }

        private async Task BroadcastToAllClients(object data)
        {
            var json = JsonSerializer.Serialize(data);
            var bytes = Encoding.UTF8.GetBytes(json);

            var tasks = _connectedClients
                .Where(c => c.State == WebSocketState.Open)
                .Select(async client =>
                {
                    try
                    {
                        await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[WebSocket] Error broadcasting to client: {ex.Message}");
                    }
                });

            await Task.WhenAll(tasks);
        }

        private string GetWebSocketTestPage()
        {
            return @"<!DOCTYPE html>
<html>
<head>
    <title>HungDuy Parking WebSocket Test</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .container { max-width: 800px; margin: 0 auto; }
        .status { padding: 10px; margin: 10px 0; border-radius: 5px; }
        .connected { background-color: #d4edda; color: #155724; }
        .disconnected { background-color: #f8d7da; color: #721c24; }
        button { margin: 5px; padding: 10px 15px; }
        #messages { border: 1px solid #ccc; height: 300px; overflow-y: auto; padding: 10px; }
        input[type='text'] { width: 300px; padding: 5px; }
    </style>
</head>
<body>
    <div class='container'>
        <h1>🔌 HungDuy Parking WebSocket Test</h1>
        <div id='status' class='status disconnected'>Disconnected</div>
        
        <button onclick='connect()'>Connect</button>
        <button onclick='disconnect()'>Disconnect</button>
        <button onclick='sendPing()'>Ping</button>
        <button onclick='requestStatus()'>Status</button>
        <button onclick='clearMessages()'>Clear</button>
        <br><br>
        
        <input type='text' id='messageInput' placeholder='Type message...' onkeypress='handleKeyPress(event)'>
        <button onclick='sendMessage()'>Send Message</button>
        
        <h3>Messages:</h3>
        <div id='messages'></div>
    </div>

    <script>
        let ws = null;
        const status = document.getElementById('status');
        const messages = document.getElementById('messages');
        
        function addMessage(msg) {
            const div = document.createElement('div');
            div.textContent = `[${new Date().toLocaleTimeString()}] ${msg}`;
            messages.appendChild(div);
            messages.scrollTop = messages.scrollHeight;
        }
        
        function updateStatus(connected) {
            if (connected) {
                status.textContent = 'Connected';
                status.className = 'status connected';
            } else {
                status.textContent = 'Disconnected';
                status.className = 'status disconnected';
            }
        }
        
        function connect() {
            if (ws) return;
            
            ws = new WebSocket('ws://localhost:5001/ws');
            
            ws.onopen = function() {
                updateStatus(true);
                addMessage('Connected to WebSocket server');
            };
            
            ws.onmessage = function(event) {
                const data = JSON.parse(event.data);
                addMessage(`Received: ${JSON.stringify(data, null, 2)}`);
            };
            
            ws.onclose = function() {
                updateStatus(false);
                addMessage('Connection closed');
                ws = null;
            };
            
            ws.onerror = function(error) {
                addMessage(`Error: ${error}`);
            };
        }
        
        function disconnect() {
            if (ws) {
                ws.close();
            }
        }
        
        function sendPing() {
            if (ws && ws.readyState === WebSocket.OPEN) {
                ws.send(JSON.stringify({ type: 'ping' }));
            }
        }
        
        function requestStatus() {
            if (ws && ws.readyState === WebSocket.OPEN) {
                ws.send(JSON.stringify({ type: 'status' }));
            }
        }
        
        function sendMessage() {
            const input = document.getElementById('messageInput');
            const message = input.value.trim();
            
            if (message && ws && ws.readyState === WebSocket.OPEN) {
                ws.send(JSON.stringify({ type: 'message', content: message }));
                input.value = '';
            }
        }
        
        function handleKeyPress(event) {
            if (event.key === 'Enter') {
                sendMessage();
            }
        }
        
        function clearMessages() {
            messages.innerHTML = '';
        }
        
        // Auto connect on page load
        setTimeout(connect, 500);
    </script>
</body>
</html>";
        }
    }
}