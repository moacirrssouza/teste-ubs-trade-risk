using UBS.TradeRisk.Application.DTOs;
using UBS.TradeRisk.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace UBS.TradeRisk.Api.Controllers;

/// <summary>
/// Controller para classificação de trades
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TradesClassificationController : ControllerBase
{
    private readonly ITradeClassificationService _tradeClassificationService;
    private readonly ILogger<TradesClassificationController> _logger;

    public TradesClassificationController(
        ITradeClassificationService tradeClassificationService,
        ILogger<TradesClassificationController> logger)
    {
        _tradeClassificationService = tradeClassificationService ??
            throw new ArgumentNullException(nameof(tradeClassificationService));
        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));
    }

    
    [HttpPost("classify")]
    [ProducesResponseType(typeof(TradeClassificationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ClassifyTrades([FromBody] List<TradeInputDto> trades)
    {
        try
        {
            _logger.LogInformation("Iniciando classificação de {Count} trades", trades?.Count ?? 0);

            if (trades == null || trades.Count == 0)
            {
                _logger.LogWarning("Lista de trades vazia recebida");
                return BadRequest(new { message = "Trades list cannot be null or empty" });
            }

            var result = _tradeClassificationService.ClassifyTrades(trades);

            _logger.LogInformation("Classificação completada com sucesso para {Count} trades", trades.Count);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Erro de validação ao classificar trades: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado ao classificar trades");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while processing the request" });
        }
    }
}