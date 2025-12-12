# Driver Location WebSocket API

This document describes how to use the WebSocket connection for real-time driver location tracking using SignalR.

## SignalR Hub Endpoint

**WebSocket URL:** `/locationHub`

Example connection URL: `https://your-domain.com/locationHub`

## Connection Types

### 1. Driver Connection
Drivers connect to share their location in real-time using their **Driver ID (int)**.

### 2. Client Connection (Users/Admin)
Clients connect to receive real-time location updates from all drivers.

## Frontend JavaScript Example

### Basic Connection Setup

```javascript
// Include SignalR library
// <script src="https://unpkg.com/@microsoft/signalr@latest/dist/browser/signalr.min.js"></script>

const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://your-domain.com/locationHub")
    .withAutomaticReconnect()
    .build();

// Start the connection
connection.start()
    .then(() => {
        console.log("Connected to location hub");
        // Initialize based on user type
        if (isDriver) {
            initializeDriverConnection();
        } else {
            initializeClientConnection();
        }
    })
    .catch(err => console.error("Connection failed: ", err));
```

### Driver Implementation

```javascript
function initializeDriverConnection() {
    const driverId = 123; // Driver ID as integer (not string)
    
    // Connect as driver
    connection.invoke("ConnectDriver", driverId)
        .then(() => {
            console.log("Driver connected successfully");
            // Start sending location updates
            startLocationTracking();
        })
        .catch(err => console.error("Driver connection failed: ", err));
}

function startLocationTracking() {
    // Get location every 10 seconds
    setInterval(() => {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                (position) => {
                    const locationUpdate = {
                        lat: position.coords.latitude,
                        lng: position.coords.longitude,
                        timestamp: new Date().toISOString()
                    };
                    
                    // Send location via SignalR
                    connection.invoke("UpdateLocation", locationUpdate)
                        .catch(err => console.error("Failed to send location: ", err));
                },
                (error) => {
                    console.error("Location error: ", error);
                },
                {
                    enableHighAccuracy: true,
                    timeout: 10000,
                    maximumAge: 5000
                }
            );
        }
    }, 10000); // Update every 10 seconds
}
```

### Client Implementation (Users/Admin)

```javascript
function initializeClientConnection() {
    // Connect as client to receive updates
    connection.invoke("ConnectClient")
        .then(() => {
            console.log("Client connected successfully");
        })
        .catch(err => console.error("Client connection failed: ", err));
    
    // Request current online drivers
    connection.invoke("GetOnlineDrivers")
        .catch(err => console.error("Failed to get online drivers: ", err));
}

// Listen for events
connection.on("OnlineDrivers", (drivers) => {
    console.log("Online drivers:", drivers);
    // Update map with current online drivers
    updateMapWithDrivers(drivers);
});

connection.on("LocationUpdate", (driverLocation) => {
    console.log("Driver location update:", driverLocation);
    // Update specific driver marker on map
    updateDriverMarker(driverLocation);
});

connection.on("DriverConnected", (driver) => {
    console.log("New driver connected:", driver);
    // Add new driver marker to map
    addDriverMarker(driver);
});

connection.on("DriverDisconnected", (driver) => {
    console.log("Driver disconnected:", driver);
    // Update or remove driver marker
    updateDriverStatus(driver, false);
});
```

## REST API Endpoints

### 1. Update Driver Location (POST)
```
POST /api/location/update
Content-Type: application/json

{
    "driverId": 123,
    "lat": 40.7128,
    "lng": -74.0060,
    "timestamp": "2024-01-01T12:00:00Z"
}
```

### 2. Get Online Drivers (GET)
```
GET /api/location/drivers
```

### 3. Get Specific Driver Location (GET)
```
GET /api/location/driver/123
```

### 4. Get Nearby Drivers (GET)
```
GET /api/location/nearby?lat=40.7128&lng=-74.0060&radiusKm=10
```

### 5. Remove Driver Location (DELETE)
```
DELETE /api/location/driver/123
```

## Data Models

### DriverLocationDto (Request)
```json
{
    "driverId": 123,
    "lat": 40.7128,
    "lng": -74.0060,
    "timestamp": "2024-01-01T12:00:00Z"
}
```

### DriverLocationResponseDto (Response)
```json
{
    "driverId": 123,
    "driverName": "John Doe",
    "lat": 40.7128,
    "lng": -74.0060,
    "lastUpdate": "2024-01-01T12:00:00Z",
    "isOnline": true
}
```

## SignalR Events

### Client-to-Server Events
- `ConnectDriver(driverId: int)` - Driver connects and starts sharing location
- `ConnectClient()` - Client connects to receive location updates
- `UpdateLocation(locationUpdate)` - Driver sends location update
- `UpdateLocationWithDriverId(driverId: int, locationUpdate)` - Alternative method with explicit driver ID
- `GetOnlineDrivers()` - Request list of online drivers
- `GetDriverLocation(driverId: int)` - Request specific driver location
- `Ping()` - Keep connection alive

### Server-to-Client Events
- `OnlineDrivers(drivers[])` - List of currently online drivers
- `LocationUpdate(driverLocation)` - Real-time location update
- `DriverConnected(driver)` - New driver came online
- `DriverDisconnected(driver)` - Driver went offline
- `DriverLocationUpdate(driverLocation)` - Location update for other drivers
- `DriverLocation(driverLocation)` - Response to GetDriverLocation
- `DriverNotFound(driverId: int)` - Driver not found response
- `DriverRemoved(driverId: int)` - Driver removed from tracking
- `Pong(timestamp)` - Response to ping

## Important Notes

- **Driver ID**: Uses the real driver ID (integer) from the Driver entity, not the user ID (string)
- **Database Lookup**: The system looks up driver information using `d.Id == driverId` where driverId is an integer
- **Connection Mapping**: The hub maintains a mapping between SignalR connection IDs and driver IDs for proper tracking
- **Offline Detection**: Drivers are considered offline if no location update is received for 5 minutes
- **Memory Storage**: Driver locations are kept in memory and will be lost on server restart
- **Simplified Location Data**: Only lat, lng, and timestamp are tracked
- **Consistent Naming**: Uses `lat` and `lg