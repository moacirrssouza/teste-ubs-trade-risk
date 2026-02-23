using UBS.TradeRisk.Domain.Specifications;
using FluentAssertions;

namespace UBS.TradeRisk.Tests.Domain;

public class TradeRiskClassificationSpecificationTests
{
    private readonly TradeRiskClassificationSpecification _specification;

    public TradeRiskClassificationSpecificationTests()
    {
        _specification = new TradeRiskClassificationSpecification();
    }

    [Fact]
    public void ClassifyRisk_WithValueLessThanThreshold_ShouldReturnLowRisk()
    {
        // Arrange
        var value = 500_000m;
        var sector = "Public";

        // Act
        var result = _specification.ClassifyRisk(value, sector);

        // Assert
        result.Should().Be("LOWRISK");
    }

    [Fact]
    public void ClassifyRisk_WithValueGreaterThanThresholdAndPublicSector_ShouldReturnHighRisk()
    {
        // Arrange
        var value = 2_000_000m;
        var sector = "Public";

        // Act
        var result = _specification.ClassifyRisk(value, sector);

        // Assert
        result.Should().Be("HIGHRISK");
    }

    [Fact]
    public void ClassifyRisk_WithValueGreaterThanThresholdAndPrivateSector_ShouldReturnMediumRisk()
    {
        // Arrange
        var value = 2_000_000m;
        var sector = "Private";

        // Act
        var result = _specification.ClassifyRisk(value, sector);

        // Assert
        result.Should().Be("MEDIUMRISK");
    }

    [Fact]
    public void ClassifyRisk_WithValueEqualToThresholdAndPublicSector_ShouldReturnHighRisk()
    {
        // Arrange
        var value = 1_000_000m;
        var sector = "Public";

        // Act
        var result = _specification.ClassifyRisk(value, sector);

        // Assert
        result.Should().Be("HIGHRISK");
    }

    [Fact]
    public void ClassifyRisk_WithValueEqualToThresholdAndPrivateSector_ShouldReturnMediumRisk()
    {
        // Arrange
        var value = 1_000_000m;
        var sector = "Private";

        // Act
        var result = _specification.ClassifyRisk(value, sector);

        // Assert
        result.Should().Be("MEDIUMRISK");
    }

    [Fact]
    public void ClassifyRisk_WithInvalidSector_ShouldThrowArgumentException()
    {
        // Arrange
        var value = 2_000_000m;
        var sector = "InvalidSector";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _specification.ClassifyRisk(value, sector));
    }

    [Theory]
    [InlineData(100_000, "Public", "LOWRISK")]
    [InlineData(500_000, "Private", "LOWRISK")]
    [InlineData(999_999, "Public", "LOWRISK")]
    [InlineData(1_000_000, "Public", "HIGHRISK")]
    [InlineData(1_500_000, "Public", "HIGHRISK")]
    [InlineData(1_000_000, "Private", "MEDIUMRISK")]
    [InlineData(5_000_000, "Private", "MEDIUMRISK")]
    public void ClassifyRisk_WithVariousInputs_ShouldReturnCorrectCategory(
        decimal value, string sector, string expectedCategory)
    {
        // Act
        var result = _specification.ClassifyRisk(value, sector);

        // Assert
        result.Should().Be(expectedCategory);
    }
}