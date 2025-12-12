using System;
using System.ComponentModel.DataAnnotations;

namespace Snap.APIs.DTOs
{
    public class TripsHistoryDto
    {
        public int Id { get; set; }
        [Range(0, 5)]
        public int Review { get; set; }
        public string PaymentWay { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
        public double TotalTip { get; set; }
        public int DriverId { get; set; }
    }

    public class CreateTripsHistoryDto
    {
        [Range(0, 5)]
        public int Review { get; set; }
        public string PaymentWay { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
        public double TotalTip { get; set; }
        public int DriverId { get; set; }
    }
}
