namespace PinkCoinProgression.Core;

public readonly record struct GuaranteedChance(int Guaranteed, int RemainderPercent)
{
    public static GuaranteedChance FromPercentage(int percent)
    {
        if (percent < 0) throw new ArgumentOutOfRangeException(nameof(percent));
        return new(percent / 100, percent % 100);
    }

    public int Roll(Func<int> roll0To99)
    {
        ArgumentNullException.ThrowIfNull(roll0To99);
        if (RemainderPercent == 0) return Guaranteed;
        int result = roll0To99();
        if (result is < 0 or > 99) throw new ArgumentOutOfRangeException(nameof(roll0To99));
        return Guaranteed + (result < RemainderPercent ? 1 : 0);
    }

    public override string ToString() => Guaranteed > 0
        ? $"{Guaranteed} + {RemainderPercent}%"
        : $"{RemainderPercent}%";
}

public static class Effects
{
    public static int BonusDraw(CharacterProgress p) => p.Level(UpgradeId.CardDraw);
    public static int BonusEnergy(CharacterProgress p) => p.Level(UpgradeId.MaxEnergy);
    public static int BonusMaxHp(CharacterProgress p) => p.Level(UpgradeId.MaxHp);
    public static int StartingPlating(CharacterProgress p) => p.Level(UpgradeId.StartingPlating);
    public static int ExtraShopRemovals(CharacterProgress p) => p.Level(UpgradeId.ShopRemovals);
    public static int RestHealPercent(CharacterProgress p) => 30 + 5 * p.Level(UpgradeId.RestHealing);
    public static GuaranteedChance SmithBonus(CharacterProgress p) =>
        GuaranteedChance.FromPercentage(5 * p.Level(UpgradeId.BonusSmith));
    public static GuaranteedChance ExtraRestActions(CharacterProgress p) =>
        GuaranteedChance.FromPercentage(5 * p.Level(UpgradeId.ExtraRestActions));
    public static int DiscountPercent(CharacterProgress p, UpgradeId id) =>
        id is UpgradeId.CardDiscount or UpgradeId.RelicDiscount
            or UpgradeId.PotionDiscount or UpgradeId.RemovalDiscount
            ? Math.Min(100, p.Level(id))
            : throw new ArgumentException("Not a shop discount.", nameof(id));

    public static long DiscountedPrice(long basePrice, int percent)
    {
        if (basePrice < 0 || percent is < 0 or > 100) throw new ArgumentOutOfRangeException();
        // Floor fractional gold amounts; game adapter should verify native rounding conventions.
        return (long)decimal.Floor((decimal)basePrice * (100 - percent) / 100m);
    }

    /// <summary>Returns distinct eligible bonus upgrade targets. Exclude the manually smithed card.</summary>
    public static IReadOnlyList<T> SelectBonusSmithTargets<T>(
        IEnumerable<T> eligibleCards, T manuallyUpgraded, int requested,
        Func<int, int> chooseIndex, IEqualityComparer<T>? comparer = null) where T : notnull
    {
        if (requested < 0) throw new ArgumentOutOfRangeException(nameof(requested));
        ArgumentNullException.ThrowIfNull(eligibleCards);
        ArgumentNullException.ThrowIfNull(chooseIndex);
        var candidates = eligibleCards.Distinct(comparer).ToList();
        candidates.RemoveAll(c => EqualityComparer<T>.Default.Equals(c, manuallyUpgraded)
            || (comparer?.Equals(c, manuallyUpgraded) ?? false));
        var result = new List<T>();
        while (result.Count < requested && candidates.Count > 0)
        {
            int index = chooseIndex(candidates.Count);
            if (index < 0 || index >= candidates.Count) throw new ArgumentOutOfRangeException(nameof(chooseIndex));
            result.Add(candidates[index]);
            candidates.RemoveAt(index);
        }
        return result;
    }

    /// <summary>Each unique action is usable once; returns count including the base action.</summary>
    public static int RestSiteActionBudget(CharacterProgress p, int availableUniqueActions, Func<int> roll0To99)
    {
        if (availableUniqueActions < 0) throw new ArgumentOutOfRangeException(nameof(availableUniqueActions));
        return Math.Min(availableUniqueActions, 1 + ExtraRestActions(p).Roll(roll0To99));
    }
}
