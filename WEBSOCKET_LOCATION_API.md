# Driver Location WebSocket API

This document describes how to use the WebSocket connection for real-time driver location tracking using SignalR.

## SignalR Hub Endpoint

**WebSocket URL:** `/locationHub`

Example connection URL: `https://your-domain.com/locationHub`

## Connection Types

### 1. Driver Connection
Drivers connect to share their location in real-time.

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
    const driverId = "your-driver-user-id";
    
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
                        latitude: position.coords.latitude,
                        longitude: position.coords.longitude,
                        timestamp: new Date().toISOString(),
                        speed: position.coords.speed,
                        heading: position.coords.heading,
                        accuracy: position.coords.accuracy
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
    "driverId": "string",
    "latitude": 40.7128,
    "longitude": -74.0060,
    "timestamp": "2024-01-01T12:00:00Z",
    "speed": 50.5,
    "heading": 180.0,
    "accuracy": 10.0
}
```

### 2. Get Online Drivers (GET)
```
GET /api/location/drivers
```

### 3. Get Specific Driver Location (GET)
```
GET /api/location/driver/{driverId}
```

### 4. Get Nearby Drivers (GET)
```
GET /api/location/nearby?latitude=40.7128&longitude=-74.0060&radiusKm=10
```