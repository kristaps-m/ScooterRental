using ScooterRental.Exceptions;
namespace ScooterRental;

public class RentalCompany:IRentalCompany
{
    public string Name { get; }
    private IList<RentedScooter> _listOfRentedScooters;
    private IScooterService _boltTurboBee;
    private IMoneyCalculator _calculator;

    public RentalCompany(string name,IScooterService inScooterService,
        IList<RentedScooter> rentedScooters)
    {
        Name = name;
        _boltTurboBee = inScooterService;
        _listOfRentedScooters = rentedScooters;
        _calculator = new MoneyHustlerCalculator();
    }

    public void StartRent(string id)
    {
        var scooter = _boltTurboBee.GetScooterById(id);
        if (scooter == null)
        {
            throw new ScooterDoesNotExistException(id);
        }
        else if (scooter.IsRented)
        {
            throw new ScooterIsAlreadyRentedException(id);
        }
        
        scooter.IsRented = true;
        _listOfRentedScooters.Add(new RentedScooter(scooter.Id,DateTime.UtcNow, scooter.PricePerMinute));
    }

    public decimal EndRent(string id)
    {
        var scooter = _boltTurboBee.GetScooterById(id);
        var rentedScooter = _listOfRentedScooters.FirstOrDefault(s => s.ID == id && !s.EndTime.HasValue);
        if (rentedScooter == null)
        {
            return 0;
        }
        rentedScooter.EndTime = DateTime.UtcNow;
        DateTime endTime = rentedScooter.EndTime ?? DateTime.Now;
        DateTime startTime = rentedScooter.StartTime;
        scooter.IsRented = false;
        var newRentedScooter = new RentedScooter(id,startTime,endTime,scooter.PricePerMinute);

        return _calculator.CalculateTheMoney(newRentedScooter);
    }

    public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
    {
        return _calculator.TotallyHonestYearIncomeCalculator(year, includeNotCompletedRentals,
            _listOfRentedScooters, _boltTurboBee);
    }
}