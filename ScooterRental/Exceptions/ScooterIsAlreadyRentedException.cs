namespace ScooterRental.Exceptions;

public class ScooterIsAlreadyRentedException:Exception
{
    public ScooterIsAlreadyRentedException(string id) 
        :base($"Scooter with {id} is already rented."){ }
}