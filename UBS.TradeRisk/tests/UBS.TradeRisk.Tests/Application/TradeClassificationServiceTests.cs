using UBS.TradeRisk.Application.DTOs;
using UBS.TradeRisk.Application.Services;
using UBS.TradeRisk.Domain.Specifications;
using Moq;
using FluentAssertions;

namespace UBS.TradeRisk.Tests.Application;

public class TradeClassificationServiceTests
{
    private readonly Mock<ITradeRiskClassificationSpecification> _mockSpecification;
    private readonly TradeClassificationService _service;

    public TradeClassificationServiceTests()
    {
        _mockSpecification = new Mock<ITradeRiskClassificationSpecification>();
        _service = new TradeClassificationService(_mockSpecification.Object);
    }

    [Fact]
    public void ClassifyTrades_WithValidTrades_ShouldReturnCategorizedTrades()
    {
        // Arrange
        var trades = new List<TradeInputDto>
        {
            new() { Value = 500_000, ClientSector = "Public", ClientId = "CLI001" },
            new() { Value = 2_000_000, ClientSector = "Private", ClientId = "CLI002" }
        };

        _mockSpecification
            .Setup(x => x.ClassifyRisk(500_000, "Public"))
            .Returns("LOWRISK");

        _mockSpecification
            .Setup(x => x.ClassifyRisk(2_000_000, "Private"))
            .Returns("HIGHRISK");

        // Act
        var result = _service.ClassifyTrades(trades);

        // Assert
        result.Should().NotBeNull();
        result.Categories.Should().HaveCount(2);
        result.Categories[0].Should().Be("LOWRISK");
        result.Categories[1].Should().Be("HIGHRISK");
    }

    [Fact]
    public void ClassifyTrades_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var trades = new List<TradeInputDto>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _service.ClassifyTrades(trades));
    }

    [Fact]
    public void ClassifyTrades_WithNullList_ShouldThrowArgumentException()
    {
        // Arrange
        List<TradeInputDto>? trades = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _service.ClassifyTrades(trades!));
    }

    [Fact]
    public void ClassifyTrades_WithInvalidValue_ShouldThrowArgumentException()
    {
        // Arrange
        var trades = new List<TradeInputDto>
        {
            new() { Value = -100, ClientSector = "Public", ClientId = "CLI001" }
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _service.ClassifyTrades(trades));
    }

    [Fact]
    public void ClassifyTrades_WithInvalidSector_ShouldThrowArgumentException()
    {
        // Arrange
        var trades = new List<TradeInputDto>
        {
            new() { Value = 500_000, ClientSector = "Invalid", ClientId = "CLI001" }
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _service.ClassifyTrades(trades));
    }

    [Fact]
    public void ClassifyTrades_WithEmptyClientId_ShouldThrowArgumentException()
    {
        // Arrange
        var trades = new List<TradeInputDto>
        {
            new() { Value = 500_000, ClientSector = "Public", ClientId = string.Empty }
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _service.ClassifyTrades(trades));
    }

    [Fact]
    public void ClassifyTrades_WithMultipleTrades_ShouldMaintainOrder()
    {
        // Arrange
        var trades = new List<TradeInputDto>
        {
            new() { Value = 2_000_000, ClientSector = "Private", ClientId = "CLI001" },
            new() { Value = 400_000, ClientSector = "Public", ClientId = "CLI002" },
            new() { Value = 500_000, ClientSector = "Public", ClientId = "CLI003" },
            new() { Value = 3_000_000, ClientSector = "Public", ClientId = "CLI004" }
        };

        _mockSpecification.Setup(x => x.ClassifyRisk(It.IsAny<decimal>(), It.IsAny<string>()))
            .Returns((decimal value, string sector) =>
            {
                return value < 1_000_000 ? "LOWRISK" :
                       sector == "Public" ? "MEDIUMRISK" : "HIGHRISK";
            });

        // Act
        var result = _service.ClassifyTrades(trades);

        // Assert
        result.Categories.Should().Equal("HIGHRISK", "LOWRISK", "LOWRISK", "MEDIUMRISK");
    }
}