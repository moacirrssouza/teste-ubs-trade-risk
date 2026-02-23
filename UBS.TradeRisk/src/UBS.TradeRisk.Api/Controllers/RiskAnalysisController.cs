using UBS.TradeRisk.Application.DTOs;
using UBS.TradeRisk.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace UBS.TradeRisk.Api.Controllers;

/// <summary>
/// Controller para análise de distribuição de risco
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RiskAnalysisController : ControllerBase
{
    private readonly IRiskDistributionAnalysisService _riskDistributionAnalysisService;
    private readonly ILogger<RiskAnalysisController> _logger;

    public RiskAnalysisController(
        IRiskDistributionAnalysisService riskDistributionAnalysisService,
        ILogger<RiskAnalysisController> logger)
    {
        _riskDistributionAnalysisService = riskDistributionAnalysisService ??
            throw new ArgumentNullException(nameof(riskDistributionAnalysisService));
        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));
    }

 
    [HttpPost("analyze")]
    [ProducesResponseType(typeof(RiskDistributionAnalysisResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult AnalyzeRiskDistribution([FromBody] List<TradeInputDto> trades)
    {
        try
        {
            _logger.LogInformation("Iniciando análise de distribuição de risco para {Count} trades", trades?.Count ?? 0);

            if (trades == null || trades.Count == 0)
            {
                _logger.LogWarning("Lista de trades vazia recebida para análise");
                return BadRequest(new { message = "Trades list cannot be null or empty" });
            }

            if (trades.Count > 100_000)
            {
                _logger.LogWarning("Número de trades excede o limite: {Count}", trades.Count);
                return BadRequest(new { message = "Maximum 100,000 trades allowed per request" });
            }

            var result = _riskDistributionAnalysisService.AnalyzeRiskDistribution(trades);

            _logger.LogInformation("Análise completada em {ProcessingTime}ms para {Count} trades",
                result.ProcessingTimeMs, trades.Count);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Erro de validação ao analisar risco: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado ao analisar distribuição de risco");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while processing the request" });
        }
    }
}