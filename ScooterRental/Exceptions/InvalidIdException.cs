namespace ScooterRental.Exceptions;

public class InvalidIdException:Exception
{
    public InvalidIdException(string id) : base($"Given '{id}' is not valid.") { }
}