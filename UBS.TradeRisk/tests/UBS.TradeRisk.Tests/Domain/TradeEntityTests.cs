using UBS.TradeRisk.Domain.Entities;
using FluentAssertions;

namespace UBS.TradeRisk.Tests.Domain;

public class TradeEntityTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnValidTrade()
    {
        // Arrange
        var value = 500000m;
        var clientSector = "Private";
        var clientId = "CLI001";

        // Act
        var trade = Trade.Create(value, clientSector, clientId);

        // Assert
        trade.Should().NotBeNull();
        trade.Value.Should().Be(value);
        trade.ClientSector.Should().Be(clientSector);
        trade.ClientId.Should().Be(clientId);
        trade.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Trade.Create(-1000, "Public", "CLI001"));
    }

    [Fact]
    public void Create_WithZeroValue_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Trade.Create(0, "Public", "CLI001"));
    }

    [Fact]
    public void Create_WithInvalidSector_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Trade.Create(100000, "InvalidSector", "CLI001"));
    }

    [Fact]
    public void Create_WithEmptyClientId_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Trade.Create(100000, "Public", string.Empty));
    }

    [Fact]
    public void ClassifyRisk_ShouldSetRiskCategory()
    {
        // Arrange
        var trade = Trade.Create(500000, "Public", "CLI001");
        var riskCategory = "LOWRISK";

        // Act
        trade.ClassifyRisk(riskCategory);

        // Assert
        trade.RiskCategory.Should().Be(riskCategory);
    }

    [Fact]
    public void ClassifyRisk_WithEmptyCategory_ShouldThrowArgumentException()
    {
        // Arrange
        var trade = Trade.Create(500000, "Public", "CLI001");

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            trade.ClassifyRisk(string.Empty));
    }
}