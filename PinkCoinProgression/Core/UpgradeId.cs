namespace PinkCoinProgression.Core;

public enum UpgradeId
{
    MaxHp, CardDraw, MaxEnergy, StartingPlating,
    RestHealing, BonusSmith, ExtraRestActions,
    ShopRemovals, CardDiscount, RelicDiscount, PotionDiscount, RemovalDiscount
}

public sealed record UpgradeDefinition(decimal BasePrice, decimal Multiplier, int? Cap);

public static class UpgradeCatalog
{
    public static readonly IReadOnlyDictionary<UpgradeId, UpgradeDefinition> All =
        new Dictionary<UpgradeId, UpgradeDefinition>
        {
            [UpgradeId.MaxHp] = new(5m, 1.2745m, null),
            [UpgradeId.CardDraw] = new(75m, 1.4422m, 5),
            [UpgradeId.MaxEnergy] = new(150m, 1.7321m, 7),
            [UpgradeId.StartingPlating] = new(20m, 1.4963m, 50),
            [UpgradeId.RestHealing] = new(10m, 1.3125m, 14),
            [UpgradeId.BonusSmith] = new(15m, 1.1616m, 100),
            [UpgradeId.ExtraRestActions] = new(15m, 1.1854m, 60),
            [UpgradeId.ShopRemovals] = new(45m, 2.1544m, 10),
            [UpgradeId.CardDiscount] = new(5m, 1.311m, 100),
            [UpgradeId.RelicDiscount] = new(15m, 1.4051m, 100),
            [UpgradeId.PotionDiscount] = new(2m, 1.3653m, 100),
            [UpgradeId.RemovalDiscount] = new(8m, 1.2508m, 100),
        };

    public static long Price(UpgradeId id, int alreadyOwned)
    {
        if (alreadyOwned < 0) throw new ArgumentOutOfRangeException(nameof(alreadyOwned));
        var def = All[id];
        if (def.Cap is int cap && alreadyOwned >= cap)
            throw new InvalidOperationException("Upgrade is at MAX.");
        decimal amount = def.BasePrice;
        try
        {
            checked
            {
                for (int i = 0; i < alreadyOwned; i++) amount *= def.Multiplier;
                return decimal.ToInt64(decimal.Round(amount, 0, MidpointRounding.AwayFromZero));
            }
        }
        catch (OverflowException e)
        {
            throw new InvalidOperationException("Upgrade price exceeds supported currency range.", e);
        }
    }
}
