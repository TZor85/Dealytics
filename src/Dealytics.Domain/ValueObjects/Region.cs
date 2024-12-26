namespace Dealytics.Domain.ValueObjects;

public record Region()
{
    public string? Category { get; set; }
    public string? Name { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool? IsHash { get; set; }
    public bool? IsColor { get; set; }
    public bool? IsBoard { get; set; }
    public string? Color { get; set; }
    public bool? IsOnlyNumber { get; set; }
    public double? InactiveUmbral { get; set; }
    public double? Umbral { get; set; }
}