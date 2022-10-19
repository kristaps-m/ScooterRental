using FluentAssertions;
using ScooterRental.Exceptions;

namespace ScooterRental.Tests;

[TestClass]
public class ScooterServiceTests
{
    private IScooterService _scooterService;
    private List<Scooter> _inventory;

    [TestInitialize]
    public void Setup()
    {
        _inventory = new List<Scooter>();
        _scooterService = new ScooterService(_inventory);
    }

    [TestMethod]
    public void AddScooter_AddValidScooter_ScooterAdded()
    {
        // Act
        _scooterService.AddScooter("1",0.2m);
        // Assert
        _inventory.Count.Should().Be(1);
    }
    
    [TestMethod]
    public void AddScooter_AddScooterTwice_ThrowsDuplicateScooterException()
    {
        // Act
        _scooterService.AddScooter("1",0.2m);
        Action action = () => _scooterService.AddScooter("1", 0.2m);
        // Assert
        action.Should().Throw<DuplicateScooterException>().WithMessage("Scooter with id 1 already exists");
    }
    
    [TestMethod]
    public void AddScooter_AddScooterWithPriceZeroOLess_ThrowsInvalidPriceException()
    {
        // Act
        Action action = () => _scooterService.AddScooter("1", -0.2m);
        // Assert
        action.Should().Throw<InvalidPriceException>().WithMessage("Given price -0.2 not valid.");
    }
    
    [TestMethod]
    public void AddScooter_AddScooterNullOREmptyID_ThrowsInvalidIdException()
    {
        // Act
        Action action = () => _scooterService.AddScooter("", 0.2m);
        // Assert
        action.Should().Throw<InvalidIdException>().WithMessage("Given '' is not valid.");
    }

    [TestMethod]
    public void RemoveScooter_ScooterExists_ScooterIsRemoved()
    {
        // Act
        _inventory.Add(new Scooter("1",0.2m));
        _scooterService.RemoveScooter("1");
        // Assert
        _inventory.Any(s => s.Id == "1").Should().BeFalse();
        _inventory.Count().Should().Be(0);
    }
    
    [TestMethod]
    public void RemoveScooter_ScooterIsRented_ScooterCanNotBeRemoved()
    {
        // Act
        _inventory.Add(new Scooter("1",0.2m));
        _inventory[0].IsRented = true;
        _scooterService.RemoveScooter("1");
        // Assert
        _inventory.Any(s => s.Id == "1").Should().BeTrue();
        _inventory.Count().Should().Be(1);
    }
    
    [TestMethod]
    public void RemoveScooter_ScooterDoesNotExist_ThrowsScooterDoesNotExistException()
    {
        // Act
        Action action = () => _scooterService.RemoveScooter("1");
        // Assert
        action.Should().Throw<ScooterDoesNotExistException>().WithMessage("Scooter with 1 doesn't exist.");
    }
    
    [TestMethod]
    public void RemoveScooter_NullOREmptyIDGiven_ThrowsInvalidIdException()
    {
        // Act
        Action action = () => _scooterService.RemoveScooter("");
        // Assert
        action.Should().Throw<InvalidIdException>().WithMessage("Given '' is not valid.");
    }

    [TestMethod]
    public void GetScooters_ReturnListOfScooters()
    {

        // Act
        _inventory.Add(new Scooter("1",0.2m));
        _inventory.Add(new Scooter("2",0.2m));
        _inventory.Add(new Scooter("3",0.2m));
        _inventory.Add(new Scooter("4",0.2m));
        _inventory.Add(new Scooter("5",0.2m));
        // Arrange
        var scooterList = _scooterService.GetScooters();
        // Assert
        scooterList.Should().NotBeNullOrEmpty();
        scooterList.Should().ContainEquivalentOf(new Scooter("1", 0.2m));
        scooterList.Should().ContainEquivalentOf(new Scooter("2", 0.2m));
        scooterList.Should().ContainEquivalentOf(new Scooter("3", 0.2m));
        scooterList.Should().ContainEquivalentOf(new Scooter("4", 0.2m));
        scooterList.Should().ContainEquivalentOf(new Scooter("5", 0.2m));
        scooterList.Should().HaveCount(5);
        scooterList.Should().OnlyHaveUniqueItems();
    }
    
    [TestMethod]
    public void GetScooters_ReturnListOfScooters_CanNotAddMoreAfterGetScooters()
    {
        // Act
        _inventory.Add(new Scooter("1",0.2m));
        // Arrange
        var scooterList = _scooterService.GetScooters();
        // Assert
        scooterList.Should().HaveCount(1);
        // Act
        scooterList.Add(new Scooter("2", 0.2m));
        // Arrange
        var scooterList_2 = _scooterService.GetScooters();
        // Assert
        scooterList_2.Should().HaveCount(1);
    }

    [TestMethod]
    public void GetScooters_NullOREmpty_ThrowsScootersListIsEmptyException()
    {
        // Act
        Action action = () => _scooterService.GetScooters();
        // Assert
        action.Should().Throw<ScootersListIsEmptyException>().WithMessage("Scooters list is empty.");
    }

    [TestMethod]
    public void GetScooterById_ReturnsValidScooterUsingId()
    {
        // Arrange
        var expectedResult = new Scooter("2", 0.2m);
        // Act
        _inventory.Add(new Scooter("1",0.2m));
        _inventory.Add(new Scooter("2",0.2m));
        _inventory.Add(new Scooter("3",0.2m));
        // Assert
        _scooterService.GetScooterById("2").Should().BeEquivalentTo(expectedResult);
    }
    
    [TestMethod]
    public void GetScooterById_NoneExistingScooter_ThrowsScooterDoesNotExistException()
    {
        // Arrange
        _inventory.Add(new Scooter("1",0.2m));
        _inventory.Add(new Scooter("2",0.2m));
        _inventory.Add(new Scooter("3",0.2m));
        // Act
        Action action = () => _scooterService.GetScooterById("Elon Musk");
        // Assert
        action.Should().Throw<ScooterDoesNotExistException>().WithMessage("Scooter with Elon Musk doesn't exist.");
    }
}