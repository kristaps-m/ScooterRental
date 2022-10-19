using ScooterRental.ValidationClasses;
using ScooterRental.Exceptions;

namespace ScooterRental;

public class ScooterService:IScooterService
{
    private readonly List<Scooter> _scooters;

    public ScooterService(List<Scooter> inventory)
    {
        _scooters = inventory;
    }

    public void AddScooter(string id, decimal pricePerMinute)
    {
        Validators.ScooterIdValidator(id);
        
        if (pricePerMinute <= 0)
        {
            throw new InvalidPriceException(pricePerMinute);
        }
        
        if (_scooters.Any(scooter => scooter.Id == id))
        {
            throw new DuplicateScooterException(id);
        }
        
        _scooters.Add(new Scooter(id,pricePerMinute));
    }

    public void RemoveScooter(string id)
    {
        Validators.ScooterIdValidator(id);
        
        var scooter = _scooters.FirstOrDefault(s => s.Id == id);
        
        if (scooter == null)
        {
            throw new ScooterDoesNotExistException(id);
        }

        if (!scooter.IsRented)
        {
            _scooters.Remove(scooter);
        }
    }

    public IList<Scooter> GetScooters()
    {
        Validators.ListIsEmptyValidator(_scooters);
        // ToList() prevents from adding new scooters, when GetScooters is called.
        return _scooters.ToList();
    }

    public Scooter GetScooterById(string scooterId)
    {
        Validators.ScooterIdValidator(scooterId);
        
        var isScooter = _scooters.FirstOrDefault(s => s.Id == scooterId);
        
        if (isScooter == null)
        {
            throw new ScooterDoesNotExistException(scooterId);
        }

        foreach (var scooter in _scooters)
        {
            if (scooter.Id == scooterId)
            {
                return scooter;
            }
        }

        return null;
    }
}