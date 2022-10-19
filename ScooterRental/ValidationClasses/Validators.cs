using ScooterRental.Exceptions;

namespace ScooterRental.ValidationClasses;

public class Validators
{
    public static void ScooterIdValidator(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new InvalidIdException(id);
        }
    }

    public static void ListIsEmptyValidator(List<Scooter> inList)
    {
        if (inList.Count == 0)
        {
            throw new ScootersListIsEmptyException();
        }
    }
}