namespace UBS.TradeRisk.Application.DTOs;

/// <summary>
/// DTO para validação de entrada
/// </summary>
public class ValidationResultDto
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}