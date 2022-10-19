namespace ScooterRental.Exceptions;

public class ScooterDoesNotExistException:Exception
{
    public ScooterDoesNotExistException(string id) 
        :base($"Scooter with {id} doesn't exist."){ }
}