using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Snap.APIs.DTOs; 
using Snap.Repository.Data; 

public static class WebSocketConnectionHandler
{
    // تخزين الاتصالات المفتوحة (ConnectionId -> WebSocket)
    private static readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

    // ربط كل اتصال بـ DriverId (ConnectionId -> DriverId)
    private static readonly ConcurrentDictionary<string, int> _connectionToDriverMap = new();

    // تخزين آخر موقع للسائقين الأونلاين (DriverId -> LocationDto)
    private static readonly ConcurrentDictionary<int, DriverLocationResponseDto> _onlineDrivers = new();

    /// <summary>
    /// الدالة الرئيسية لمعالجة الاتصال
    /// </summary>
    public static async Task Handle(HttpContext context, WebSocket webSocket, SnapDbContext dbContext)
    {
        // 1. استخراج DriverId من الرابط (wss://.../ws?driverId=123)
        var driverIdQuery = context.Request.Query["driverId"].FirstOrDefault();

        if (string.IsNullOrEmpty(driverIdQuery) || !int.TryParse(driverIdQuery, out var driverId))
        {
            await CloseSocket(webSocket, "Invalid Driver ID");
            return;
        }

        // 2. تسجيل الاتصال
        var connectionId = Guid.NewGuid().ToString();
        _sockets.TryAdd(connectionId, webSocket);
        _connectionToDriverMap.TryAdd(connectionId, driverId);

        Console.WriteLine($"✅ Driver {driverId} connected via WebSocket. ConnectionId: {connectionId}");

        // 3. (اختياري) جلب بيانات السائق الأولية وإضافته للقائمة
        // نستخدم الـ dbContext الممرر من الـ Program.cs
        var driver = await dbContext.Drivers.AsNoTracking().FirstOrDefaultAsync(d => d.Id == driverId);

        if (driver != null)
        {
            var initialLocation = new DriverLocationResponseDto
            {
                DriverId = driverId,
                DriverName = driver.DriverFullname, // تأكد من اسم الخاصية في الـ Entity
                Lat = 0,
                Lng = 0,
                LastUpdate = DateTime.UtcNow,
                IsOnline = true
            };

            _onlineDrivers.AddOrUpdate(driverId, initialLocation, (k, v) => initialLocation);

            // بث رسالة "سائق متصل" للجميع (كما في SignalR)
            await BroadcastToAll("DriverConnected", initialLocation);
        }

        // 4. حلقة الاستقبال (Receive Loop)
        var buffer = new byte[1024 * 4];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var messageString = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    // معالجة الرسالة القادمة من الفلاتر (نتوقع JSON لـ LocationUpdateDto)
                    await ProcessIncomingMessage(driverId, messageString);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket Error for Driver {driverId}: {ex.Message}");
        }
        finally
        {
            // 5. عند انتهاء الاتصال
            await OnDisconnected(connectionId, webSocket);
        }
    }

    private static async Task ProcessIncomingMessage(int driverId, string jsonMessage)
    {
        try
        {
            // محاولة تحويل الـ JSON
            var locationUpdate = JsonSerializer.Deserialize<LocationUpdateDto>(jsonMessage, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (locationUpdate != null)
            {
                // تحديث البيانات في الذاكرة
                if (_onlineDrivers.TryGetValue(driverId, out var currentData))
                {
                    currentData.Lat = locationUpdate.Lat;
                    currentData.Lng = locationUpdate.Lng;
                    currentData.LastUpdate = locationUpdate.Timestamp;
                    currentData.IsOnline = true;

                    // بث التحديث للجميع (Type: LocationUpdate)
                    await BroadcastToAll("LocationUpdate", currentData);
                }
            }
        }
        catch (JsonException)
        {
            Console.WriteLine($"Invalid JSON received from Driver {driverId}");
        }
    }

    private static async Task OnDisconnected(string connectionId, WebSocket webSocket)
    {
        _sockets.TryRemove(connectionId, out _);

        if (_connectionToDriverMap.TryRemove(connectionId, out var driverId))
        {
            Console.WriteLine($"Driver {driverId} disconnected.");

            if (_onlineDrivers.TryGetValue(driverId, out var driverLocation))
            {
                driverLocation.IsOnline = false;
                // بث رسالة الانفصال
                await BroadcastToAll("DriverDisconnected", driverLocation);
            }
        }

        if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
    }

    /// <summary>
    /// دالة مساعدة لإرسال البيانات لجميع المتصلين
    /// الهيكل: { "Type": "EventName", "Data": { ... } }
    /// </summary>
    private static async Task BroadcastToAll(string eventType, object data)
    {
        var payload = new
        {
            Type = eventType,
            Data = data
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null, // يحافظ على PascalCase كما في C# 
            WriteIndented = false
        };

        var jsonString = JsonSerializer.Serialize(payload, jsonOptions);
        var bytes = Encoding.UTF8.GetBytes(jsonString);
        var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);

        var tasks = new List<Task>();

        foreach (var socket in _sockets.Values)
        {
            if (socket.State == WebSocketState.Open)
            {
                tasks.Add(socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None));
            }
        }

        await Task.WhenAll(tasks);
    }

    private static async Task CloseSocket(WebSocket socket, string reason)
    {
        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, reason, CancellationToken.None);
    }
}