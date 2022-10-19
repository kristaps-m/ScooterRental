using FluentAssertions;

namespace ScooterRental.Tests;
using Exceptions;

[TestClass]
public class RentalCompanyTests
{
    private IScooterService _scooterService;
    private IRentalCompany _company;
    private List<Scooter> _inventory;
    private List<RentedScooter> _rentedScooters;
    private IMoneyCalculator _calculator;

    [TestInitialize]
    public void Setup()
    {
        _inventory = new List<Scooter>();
        _inventory.Add(new Scooter("1",0.2m));
        _inventory.Add(new Scooter("2",0.2m));
        _inventory.Add(new Scooter("3",0.2m));
        _inventory.Add(new Scooter("4",0.2m));
        _inventory.Add(new Scooter("5",0.2m));
        _calculator = new MoneyHustlerCalculator();
        _scooterService = new ScooterService(_inventory);
        _rentedScooters = new List<RentedScooter>();
        _company = new RentalCompany("Bolt Turbo Bee",_scooterService,_rentedScooters);
    }
    
    [TestMethod]
    public void StartRent_StartRentScooter_ScooterIsRented()
    {
        // Act
        _company.StartRent("2");
        // Assert
        _scooterService.GetScooterById("2").IsRented.Should().BeTrue();
    }
    
    [TestMethod]
    public void StartRent_NoneExistingIdForScooter_ShouldThrowScooterDoesNotExistException()
    {
        // Act
        Action action = () => _company.StartRent("Elon Musk");
        // Assert
        action.Should().Throw<ScooterDoesNotExistException>()
            .WithMessage("Scooter with Elon Musk doesn't exist.");
    }
    
    [TestMethod]
    public void StartRent_TryToRentOneScooterTwiceWhileItIsRented_ShouldThrowScooterIsAlreadyRentedException()
    {
        // Act
        _company.StartRent("2");
        Action action = () => _company.StartRent("2");
        // Assert
        action.Should().Throw<ScooterIsAlreadyRentedException>().WithMessage("Scooter with 2 is already rented.");
    }

    [TestMethod]
    public void EndRent_EndRentingOfScooter_ScooterIsNotRented()
    {
        // Arrange // Act
        var scooter = _scooterService.GetScooterById("2");
        var rentedScooter = new RentedScooter("2", DateTime.Now, 0.2m);
        scooter.IsRented = true;
        _rentedScooters.Add(rentedScooter);
        _company.EndRent("2");
        // Assert
        scooter.IsRented.Should().BeFalse();
        rentedScooter.EndTime.HasValue.Should().BeTrue();
    }
    
    [TestMethod]
    public void EndRent_EndRentingOfScooter_EndRentShouldReturnIncome()
    {
        // Arrange // Act
        var scooter = _scooterService.GetScooterById("2");
        _rentedScooters.Add(new RentedScooter("2", DateTime.UtcNow.AddMinutes(-10), 0.2m));
        scooter.IsRented = true;
        // Assert
        _company.EndRent("2").Should().Be(2);
    }

    private decimal SimpleOneScooterCalculationsCreator(RentedScooter newRentedScooter)
    {
        var scooter = _scooterService.GetScooterById("2");
        _rentedScooters.Add(newRentedScooter);
        var rentedScooter = _rentedScooters.FirstOrDefault(s => s.ID == "2");
        scooter.IsRented = true;
        _company.EndRent("2");
        return _calculator.CalculateTheMoney(rentedScooter);
    }
    
    [TestMethod]
    public void CalculateTheMoney_EndRentingOfScooter_EndRentShouldReturnIncome()
    {
        // Arrange // Act
        var calc = SimpleOneScooterCalculationsCreator(new RentedScooter("2", DateTime.UtcNow.AddMinutes(-10), 0.2m));
        // Assert
        calc.Should().Be(2);
    }
    
    [TestMethod]
    public void CalculateTheMoney_EndRentingOfScooterRentOver20EuroInDay_ActivateFixedDayLimit()
    {
        // Arrange // Act
        var calc = SimpleOneScooterCalculationsCreator(new RentedScooter("2", DateTime.UtcNow.AddMinutes(-10), 3m));
        // Assert
        calc.Should().Be(20);
    }
    
    [TestMethod]
    public void CalculateTheMoney_RentScooterForMultipleDays_ActivateFixedDayLimit()
    { 
        // Arrange // Act
        var calc = SimpleOneScooterCalculationsCreator(new RentedScooter("2", DateTime.UtcNow.AddDays(-10), 0.2m));
        // Assert
        calc.Should().Be(220); // 20(48) + 20 * 9 + 20(240)
    }
    
    [TestMethod]
    public void CalculateTheMoney_RentScooterForMultipleDaysAnd10Minutes_ActivateFixedDayLimit()
    {
        // Arrange // Act
        var calc = SimpleOneScooterCalculationsCreator(new RentedScooter("2", DateTime.UtcNow.AddDays(-10).AddMinutes(-10), 7m));
        // Assert
        calc.Should().Be(220); // 200 + 20
    }
    
    [TestMethod]
    public void CalculateTheMoney_RentScooterForMultipleDaysAndMinus10Minutes_ActivateFixedDayLimit()
    {
        // Arrange // Act
        var calc = SimpleOneScooterCalculationsCreator(new RentedScooter("2", DateTime.UtcNow.AddDays(-10).AddMinutes(10), 7m));
        // Assert
        calc.Should().Be(220); // 11 * 20 = 
    }

    [TestMethod]
    public void CreateListOfRentedScooters_CreateMethodForListOfScooters_FindOneScooterGetResult()
    {
        // Arrange // Act
        var scooter = _scooterService.GetScooterById("2");
        CreateListOfRentedScooters(100,10,0.2m);
        var rentedScooter = _rentedScooters.FirstOrDefault(s => s.ID == "2");
        scooter.IsRented = true;
        _company.EndRent("2");
        var calc = _calculator.CalculateTheMoney(rentedScooter);
        // Assert
        calc.Should().Be(2);
    }
    
    [TestMethod]
    public void CalculateIncome_ListOfScooters_CalculateAllIncomeFromGivenYear()
    {
        // Arrange // Act
        CreateListOfRentedScooters(100,10,0.2m);
        var yearIncome = _company.CalculateIncome(2022,false);
        // Assert
        yearIncome.Should().Be(200); // 100 * ( 10 * 0.2 ) = 100 * 2 = 200
    }
    
    [TestMethod]
    public void CalculateIncome_ListOfScootersWithMultipleYears_CalculateAllIncomeFromAllYears()
    {
        // Arrange
        var start = new DateTime(2020, 2, 1, 11, 22, 00);
        var end = new DateTime(2020, 2, 2, 00, 10, 00);
        // Act
        _rentedScooters.Add(new RentedScooter("1", start, end, 0.2m));
        _rentedScooters.Add(new RentedScooter("2", start, end, 0.2m));
        _rentedScooters.Add(new RentedScooter("3", start, end, 0.2m));
        _rentedScooters.Add(new RentedScooter("4", start, end, 0.2m));
        _rentedScooters.Add(new RentedScooter("5", start, end, 0.2m));
        CreateListOfRentedScooters(100,10,0.2m);
        var yearIncome = _company.CalculateIncome(null,false);
        // Assert
        yearIncome.Should().Be(310); // 100 * ( 10 * 0.2 ) = 100 * 2 = 200 + 110
    }
    
    [TestMethod]
    public void CalculateIncome_NoneExistingYearInList_ThrowNoneExistingYearInListException()
    {
        // Arrange
        var year = 1989;
        // Act
        CreateListOfRentedScooters(100,10,0.2m);
        Action action = () => _company.CalculateIncome(year,false);
        // Assert
        action.Should().Throw<NoneExistingYearInListException>().
            WithMessage($"Year {year} you entered does not exist in rented scooters list.");
    }

    [TestMethod]
    public void CalculateTheMoney_ScooterWithStartEndRentTime_CalculateIncomeFrom10Minutes()
    {
        // Arrange
        var start = new DateTime(2020, 1, 1, 1, 00, 00);
        var end = new DateTime(2020, 1, 1, 1, 10, 00);
        // Act // Arrange
        var calc = SimpleOneScooterCalculationsCreator(new RentedScooter("2", start, end, 0.2m));
        // Assert
        calc.Should().Be(2);
    }

    [TestMethod]
    public void CalculateTheMoney_ScooterIsRentedNearMidnight_HandleMidnightCalculation()
    {
        // Arrange
        var start = new DateTime(2020, 1, 1, 23, 30, 00);
        var end = new DateTime(2020, 1, 2, 00, 30, 00);
        // Arrange // Act
        var calc = SimpleOneScooterCalculationsCreator(new RentedScooter("2", start, end, 1m));
        // Assert
        calc.Should().Be(40);
    }
    
    [TestMethod]
    public void CalculateTheMoney_ScooterIsRentedNearMidnightAndMultipleDays_HandleMidnightCalculation()
    {
        // Arrange
        var start = new DateTime(2020, 1, 1, 23, 45, 00);
        var end = new DateTime(2020, 1, 5, 00, 15, 00);
        // Arrange // Act
        var calc = SimpleOneScooterCalculationsCreator(new RentedScooter("2", start, end, 1m));
        // Assert
        calc.Should().Be(90); // 3 full days = 20 * 3 + 15 + 15
    }

    [TestMethod]
    public void CalculateTheMoney_ScooterIsRentedFromMiddleOfDayToMiddleOfDayAndMultipleDays_HandleMidnightCalculation()
    {
        // Arrange
        var start = new DateTime(2020, 2, 1, 11, 22, 44);
        var end = new DateTime(2020, 2, 5, 11, 21, 44);
        // Arrange // Act
        var calc = SimpleOneScooterCalculationsCreator(new RentedScooter("2", start, end, 1m));
        // Assert
        calc.Should().Be(100); // 20 * 5
    }

    [TestMethod]
    public void CalculateTheMoney_RentForExactly24Hours_CalculateCorrectIncome()
    {
        // Arrange
        var start = new DateTime(2020, 2, 1, 11, 22, 44);
        var end = new DateTime(2020, 2, 2, 11, 22, 44);
        // Arrange // Act
        var calc = SimpleOneScooterCalculationsCreator(new RentedScooter("2", start, end, 1m));
        // Assert
        calc.Should().Be(40); // 1 full days = 20+20
    }
    
    [TestMethod]
    public void CalculateTheMoney_RentForLessThan24Hours_CalculateCorrectIncome()
    {
        // Arrange
        var start = new DateTime(2020, 2, 1, 11, 22, 00);
        var end = new DateTime(2020, 2, 2, 10, 00, 00);
        // Arrange // Act
        var calc = SimpleOneScooterCalculationsCreator(new RentedScooter("2", start, end, 1m));
        // Assert
        calc.Should().Be(40); // 1 full days = 20+20
    }
    
    [TestMethod]
    public void CalculateTheMoney_RentForMoreThan24Hours_CalculateCorrectIncome()
    {
        // Arrange
        var start = new DateTime(2020, 2, 1, 11, 22, 00);
        var end = new DateTime(2020, 2, 2, 11, 44, 00);
        // Arrange // Act
        var calc = SimpleOneScooterCalculationsCreator(new RentedScooter("2", start, end, 1m));
        // Assert
        calc.Should().Be(40); // max fee for day one and two 20 + 20
    }
    
    [TestMethod]
    public void CalculateTheMoney_RentForFewMinutesInOneDay_CalculateCorrectIncome()
    {
        // Arrange
        var start = new DateTime(2020, 2, 1, 11, 22, 00);
        var end = new DateTime(2020, 2, 1, 11, 32, 00);
        // Arrange // Act
        var calc = SimpleOneScooterCalculationsCreator(new RentedScooter("2", start, end, 1m));
        // Assert
        calc.Should().Be(9);
    }
    
    [TestMethod]
    public void CalculateTheMoney_RentForOneDayAndFewMinutesAfterMidnight_CalculateCorrectIncome()
    {
        // Arrange
        var start = new DateTime(2020, 2, 1, 11, 22, 00);
        var end = new DateTime(2020, 2, 2, 00, 10, 00);
        // Arrange // Act
        var calc = SimpleOneScooterCalculationsCreator(new RentedScooter("2", start, end, 0.2m));
        // Assert
        calc.Should().Be(22);
    }
    
    /// <summary>
    /// TEST <param name=includeNotCompletedRentals> 
    /// </summary>
    [TestMethod]
    public void EndRent_OneScooterWithEndedRentAndOneScooterStillRented_CalculateIncomeButIfRentedEndTimeIsDateTimeUtcNow()
    {
        // Arrange
        var start = new DateTime(2022, 9, 1, 10, 00, 00);
        var end = new DateTime(2022, 9, 1, 10, 10, 00);
        var start2 = DateTime.UtcNow.AddMinutes(-10);
        var end2 = DateTime.UtcNow.AddDays(2).AddMinutes(50);
        // Arrange // Act
        var scooter = _scooterService.GetScooterById("1");
        _rentedScooters.Add(new RentedScooter("1", start,end, 0.2m));
        scooter.IsRented = false;
        var scooter2 = _scooterService.GetScooterById("2");
        _rentedScooters.Add(new RentedScooter("2", start2,end2, 0.2m));
        scooter2.IsRented = true;
        var yearIncome = _company.CalculateIncome(2022,true);
        // Assert
        yearIncome.Should().Be(4); // 2 + 2 
    }
    
    private void CreateListOfRentedScooters(int amount, int minutes, decimal pricePerM)
    {
        for (int i = 1; i <= amount; i++)
        {
            _rentedScooters.Add(new RentedScooter($"{i}", DateTime.UtcNow.AddMinutes(-minutes), pricePerM));
        }
    }
}