namespace Pipchi.SharedKernel.Extensions;

public static class DecimalExtensions
{
    public static string Trimmed(this decimal value)
        => value.ToString("0.########");
}
