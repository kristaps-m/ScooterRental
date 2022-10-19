namespace ScooterRental.Exceptions;

public class NoneExistingYearInListException:Exception
{
    public NoneExistingYearInListException(int? year) 
        :base($"Year {year} you entered does not exist in rented scooters list."){ }
}