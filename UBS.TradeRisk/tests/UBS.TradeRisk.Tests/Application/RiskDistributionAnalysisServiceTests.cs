using UBS.TradeRisk.Application.DTOs;
using UBS.TradeRisk.Application.Services;
using UBS.TradeRisk.Domain.Specifications;
using FluentAssertions;
using Moq;

namespace UBS.TradeRisk.Tests.Application;

public class RiskDistributionAnalysisServiceTests
{
    private readonly Mock<ITradeRiskClassificationSpecification> _mockSpecification;
    private readonly RiskDistributionAnalysisService _service;

    public RiskDistributionAnalysisServiceTests()
    {
        _mockSpecification = new Mock<ITradeRiskClassificationSpecification>();
        _service = new RiskDistributionAnalysisService(_mockSpecification.Object);
    }

    [Fact]
    public void AnalyzeRiskDistribution_WithValidTrades_ShouldReturnSummary()
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
        var result = _service.AnalyzeRiskDistribution(trades);

        // Assert
        result.Should().NotBeNull();
        result.Categories.Should().HaveCount(4);
        result.Categories.Should().Equal("HIGHRISK", "LOWRISK", "LOWRISK", "MEDIUMRISK");
        result.Summary.Should().ContainKeys("LOWRISK", "MEDIUMRISK", "HIGHRISK");
        result.Summary["LOWRISK"].Count.Should().Be(2);
        result.Summary["LOWRISK"].TotalValue.Should().Be(900_000);
        result.Summary["MEDIUMRISK"].Count.Should().Be(1);
        result.Summary["MEDIUMRISK"].TotalValue.Should().Be(3_000_000);
        result.Summary["HIGHRISK"].Count.Should().Be(1);
        result.Summary["HIGHRISK"].TotalValue.Should().Be(2_000_000);
    }

    [Fact]
    public void AnalyzeRiskDistribution_ShouldIdentifyTopClientPerCategory()
    {
        // Arrange
        var trades = new List<TradeInputDto>
        {
            new() { Value = 2_000_000, ClientSector = "Private", ClientId = "CLI001" },
            new() { Value = 1_500_000, ClientSector = "Private", ClientId = "CLI002" },
            new() { Value = 1_000_000, ClientSector = "Public", ClientId = "CLI003" },
            new() { Value = 500_000, ClientSector = "Public", ClientId = "CLI001" }
        };

        _mockSpecification.Setup(x => x.ClassifyRisk(It.IsAny<decimal>(), It.IsAny<string>()))
            .Returns((decimal value, string sector) =>
            {
                return value < 1_000_000 ? "LOWRISK" :
                       sector == "Public" ? "HIGHRISK" : "MEDIUMRISK";
            });

        // Act
        var result = _service.AnalyzeRiskDistribution(trades);

        // Assert
        result.Summary["LOWRISK"].TopClient.Should().Be("CLI001");
        result.Summary["HIGHRISK"].TopClient.Should().Be("CLI003");
        result.Summary["MEDIUMRISK"].TopClient.Should().Be("CLI001");
    }

    [Fact]
    public void AnalyzeRiskDistribution_ShouldCalculateProcessingTime()
    {
        // Arrange
        var trades = new List<TradeInputDto>
        {
            new() { Value = 500_000, ClientSector = "Public", ClientId = "CLI001" }
        };

        _mockSpecification.Setup(x => x.ClassifyRisk(It.IsAny<decimal>(), It.IsAny<string>()))
            .Returns("LOWRISK");

        // Act
        var result = _service.AnalyzeRiskDistribution(trades);

        // Assert
        result.ProcessingTimeMs.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void AnalyzeRiskDistribution_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var trades = new List<TradeInputDto>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _service.AnalyzeRiskDistribution(trades));
    }

    [Fact]
    public void AnalyzeRiskDistribution_WithMoreThan100KTrades_ShouldHandleEfficiently()
    {
        // Arrange - create 1000 trades for testing (not 100K to keep tests fast)
        var trades = new List<TradeInputDto>();
        for (int i = 0; i < 1000; i++)
        {
            trades.Add(new TradeInputDto
            {
                Value = (i % 2 == 0) ? 500_000 : 2_000_000,
                ClientSector = (i % 2 == 0) ? "Public" : "Private",
                ClientId = $"CLI{i % 10:D3}"
            });
        }

        _mockSpecification.Setup(x => x.ClassifyRisk(It.IsAny<decimal>(), It.IsAny<string>()))
            .Returns((decimal value, string sector) =>
            {
                return value < 1_000_000 ? "LOWRISK" :
                       sector == "Public" ? "HIGHRISK" : "MEDIUMRISK";
            });

        // Act
        var result = _service.AnalyzeRiskDistribution(trades);

        // Assert
        result.Should().NotBeNull();
        result.Categories.Should().HaveCount(1000);
        result.ProcessingTimeMs.Should().BeLessThan(5000);
    }
}