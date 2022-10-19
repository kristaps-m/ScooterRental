namespace ScooterRental;

public class RentedScooter
{
    public string ID { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal PricePerMinute { get; set; }

    public RentedScooter(string id, DateTime startTime, decimal pricePerMinute)
    {
        ID = id;
        StartTime = startTime;
        PricePerMinute = pricePerMinute;
    }

    public RentedScooter(string id, DateTime startTime, DateTime? endTime, decimal pricePerMinute)
    {
        ID = id;
        StartTime = startTime;
        EndTime = endTime;
        PricePerMinute = pricePerMinute;
    }
}