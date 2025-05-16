namespace TradeBridge.Core.Enums;

/// <summary>
/// Defines the pricing model for contracts
/// </summary>
public enum PricingType
{
    GDPIndexed,
    FixedEscalation,
    FlatRate,
    Market
}