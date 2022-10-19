namespace ScooterRental.Exceptions;

public class ScootersListIsEmptyException:Exception
{
    public ScootersListIsEmptyException() 
        :base($"Scooters list is empty."){ }
}