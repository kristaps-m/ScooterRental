using FluentAssertions;

namespace ScooterRental.Tests;

[TestClass]
public class ScooterTests
{
    private Scooter _scooter;
    [TestMethod]
    public void ScooterCreation_IDAndPricePerMinuteSetCorrectly()
    {
        // Arrange
        _scooter = new Scooter("1", 0.2m);
        // Assert
        _scooter.Id.Should().Be("1");
        _scooter.PricePerMinute.Should().Be(0.2m);
        _scooter.IsRented.Should().BeFalse();
    }
}