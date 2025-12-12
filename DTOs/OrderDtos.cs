using Microsoft.Win32;
using System;
using System.Text.Json.Serialization;

namespace Snap.APIs.DTOs
{
    public class LatLngDto
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime Date { get; set; }
        public string From { get; set; } = null!;
        public string To { get; set; } = null!;
        public LatLngDto FromLatLng { get; set; } = new LatLngDto();
        public LatLngDto ToLatLng { get; set; } = new LatLngDto();
        public double ExpectedPrice { get; set; }
        public string Type { get; set; } = null!;
        public double Distance { get; set; }
        public string? Notes { get; set; }
        public int NoPassengers { get; set; }
        public string? UserImage { get; set; }
        public string? UserName { get; set; }
        public string? UserPhone { get; set; }
        public string? Status { get; set; }
        public int? Driverid { get; set; }
        public double Review { get; set; }
        public string? PaymentWay { get; set; }
        public string CarType { get; set; }
        public bool PinkMode { get; set; }

    }

    public class CreateOrderDto
    {
        public string UserId { get; set; } = null!;
        public DateTime Date { get; set; }
        public string From { get; set; } = null!;
        public string To { get; set; } = null!;
        public LatLngDto FromLatLng { get; set; } = new LatLngDto();
        public LatLngDto ToLatLng { get; set; } = new LatLngDto();
        public double ExpectedPrice { get; set; }
        public string Type { get; set; } = null!;
        public double Distance { get; set; }
        public string? Notes { get; set; }
        public int NoPassengers { get; set; }
        public string? PaymentWay { get; set; }
        public string CarType { get; set; }
        public bool PinkMode { get; set; }

    }

    public class UpdateOrderDriverDto
    {
        public int OrderId { get; set; }
        public int Driverid { get; set; }
        public string Status { get; set; } = null!;
    }
}
