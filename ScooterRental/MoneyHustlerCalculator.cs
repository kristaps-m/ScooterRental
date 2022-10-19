using ScooterRental.Exceptions;

namespace ScooterRental;

public class MoneyHustlerCalculator : IMoneyCalculator
{
    private DateTime _startTime;
    private DateTime _endTime;
    private string _ID;
    private decimal _priceInOneMinute;
    private const decimal DAY_LIMIT = 20m;

    public decimal CalculateTheMoney(RentedScooter oneRentedScooter)
    {
        var totalCost = 0m;
        DateTime endTime = oneRentedScooter.EndTime ?? DateTime.UtcNow;
        DateTime startTime = oneRentedScooter.StartTime;
        endTime = endTime.AddSeconds(-2);
        decimal fromMidnightEnd = Decimal.Round((decimal)endTime.TimeOfDay.TotalMinutes);
        decimal toMidnight = Decimal.Round((decimal)(1440 - startTime.TimeOfDay.TotalMinutes));
        decimal toMidnightEnd = (decimal)(1440 - endTime.TimeOfDay.TotalMinutes);
        TimeSpan theTimeRented = endTime - startTime;
        var daysRented = theTimeRented.Days;
        var minutesRented = theTimeRented.Minutes;
        var have24HoursPassed = toMidnightEnd < toMidnight ? true : false;
        endTime = endTime.AddSeconds(2);
        
        if (have24HoursPassed)
        {
            if (startTime.Day == endTime.Day)
            {
                if (minutesRented * oneRentedScooter.PricePerMinute >= DAY_LIMIT)
                {
                    totalCost += DAY_LIMIT;
                }
                else
                {
                    totalCost += minutesRented * oneRentedScooter.PricePerMinute;
                }
            }
            else
            {
                daysRented -= 1;
                totalCost += AddToTotalCostCalculationsToMidnightAndFromMidnightEnd(totalCost, daysRented, toMidnight, fromMidnightEnd, oneRentedScooter.PricePerMinute);
            }
        }
        else
        {
            totalCost += AddToTotalCostCalculationsToMidnightAndFromMidnightEnd(totalCost, daysRented, toMidnight, fromMidnightEnd, oneRentedScooter.PricePerMinute);
        }
        
        return Decimal.Ceiling(totalCost);
    }

    private decimal AddToTotalCostCalculationsToMidnightAndFromMidnightEnd(decimal inTotalCost, int inDaysRented, decimal inToMidnight, decimal inFromMidnightEnd, decimal costPerMin)
    {
        inTotalCost += (inDaysRented) * DAY_LIMIT;
        
        if (inToMidnight * costPerMin > DAY_LIMIT)
        {
            inTotalCost += DAY_LIMIT;
        }
        else
        {
            inTotalCost += inToMidnight * costPerMin;
        }
            
        if (inFromMidnightEnd * costPerMin > DAY_LIMIT)
        {
            inTotalCost += DAY_LIMIT;
        }
        else
        {
            inTotalCost += inFromMidnightEnd * costPerMin;
        }

        return inTotalCost;
    }

    public decimal TotallyHonestYearIncomeCalculator(int? year, bool includeNotCompletedRentals,
        IList<RentedScooter> listOfRentedScooters, IScooterService boltTurboBee)
    {
        var totalYearIncome = 0m;
        if (year.HasValue && listOfRentedScooters.Any(s => s.StartTime.Year == year))
        {
            foreach (var rentedScooter in listOfRentedScooters)
            {
                if (rentedScooter.StartTime.Year == year)
                {
                    if (includeNotCompletedRentals && boltTurboBee.GetScooterById(rentedScooter.ID).IsRented)
                    {
                        rentedScooter.EndTime = DateTime.UtcNow;
                        totalYearIncome += new MoneyHustlerCalculator().CalculateTheMoney(rentedScooter);
                    }
                    else
                    {
                        totalYearIncome += new MoneyHustlerCalculator().CalculateTheMoney(rentedScooter);
                    }
                }
            }

            return totalYearIncome;
        }
        // if year is null, calculate income from all years.
        else if (!year.HasValue) 
        {
            foreach (var rentedScooter in listOfRentedScooters)
            {
                if (includeNotCompletedRentals && boltTurboBee.GetScooterById(rentedScooter.ID).IsRented)
                {
                    rentedScooter.EndTime = DateTime.UtcNow;
                    totalYearIncome += new MoneyHustlerCalculator().CalculateTheMoney(rentedScooter);
                }
                else
                {
                    totalYearIncome += new MoneyHustlerCalculator().CalculateTheMoney(rentedScooter);
                }
            }
            return totalYearIncome;
        }

        throw new NoneExistingYearInListException(year);
    }
}