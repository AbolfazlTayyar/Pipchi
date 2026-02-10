namespace Pipchi.Contracts.DTOs;

public class SymbolDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Digits { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal MinVolume { get; set; }
    public decimal MaxVolume { get; set; }
}