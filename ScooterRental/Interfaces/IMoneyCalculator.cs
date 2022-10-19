namespace ScooterRental;

public interface IMoneyCalculator
{
    /// <summary>
    /// Calculate income for one rented scooter.
    /// </summary>
    /// <param name="oneRentedScooter">is one rented scooter</param>
    public decimal CalculateTheMoney(RentedScooter oneRentedScooter);

    public decimal TotallyHonestYearIncomeCalculator(int? year, bool includeNotCompletedRentals,
        IList<RentedScooter> listOfRentedScooters, IScooterService boltTurboBee);
}