namespace PinkCoinProgression.Core;

/// <summary>Persistent per-character progression. Save one instance per character.</summary>
public sealed class CharacterProgress
{
    public long MaxCoins { get; set; }
    public Dictionary<UpgradeId, int> Levels { get; set; } = new();
    public HashSet<string> RewardedBosses { get; set; } = new(StringComparer.Ordinal);

    public int Level(UpgradeId id) => Levels.GetValueOrDefault(id);
}

/// <summary>Run-scoped currency and shop state. Never save as permanent currency.</summary>
public sealed class RunProgress
{
    public long CurrentCoins { get; private set; }
    public bool StartingShopOpened { get; private set; }

    public void OpenStartingShop(CharacterProgress character)
    {
        if (StartingShopOpened) throw new InvalidOperationException("Starting shop already opened this run.");
        CurrentCoins = character.MaxCoins;
        StartingShopOpened = true;
    }

    public bool TryBuyStartingCard(int normalMerchantPrice, Action grantCard)
    {
        if (!StartingShopOpened) return false;
        if (normalMerchantPrice < 0) throw new ArgumentOutOfRangeException(nameof(normalMerchantPrice));
        ArgumentNullException.ThrowIfNull(grantCard);
        long cost = normalMerchantPrice / 10;
        if (CurrentCoins < cost) return false;
        // Game adapter must ensure the card grant is atomic/authoritative.
        grantCard();
        CurrentCoins -= cost;
        return true;
    }

    public bool TryBuyUpgrade(CharacterProgress character, UpgradeId id)
    {
        if (!StartingShopOpened) return false;
        int level = character.Level(id);
        var definition = UpgradeCatalog.All[id];
        if (definition.Cap is int cap && level >= cap) return false;
        long price = UpgradeCatalog.Price(id, level);
        if (CurrentCoins < price || character.MaxCoins < price) return false;
        checked
        {
            character.MaxCoins -= price;
            CurrentCoins -= price;
            character.Levels[id] = level + 1;
        }
        return true;
    }

    public bool TryAwardBoss(CharacterProgress character, int act, string uniqueBossCompletionId)
    {
        if (act < 1) throw new ArgumentOutOfRangeException(nameof(act));
        if (string.IsNullOrWhiteSpace(uniqueBossCompletionId))
            throw new ArgumentException("Boss completion ID required.", nameof(uniqueBossCompletionId));
        if (!character.RewardedBosses.Add(uniqueBossCompletionId)) return false;
        try
        {
            checked { character.MaxCoins += 2L * ((act - 1L) / 3L + 1L); }
        }
        catch
        {
            character.RewardedBosses.Remove(uniqueBossCompletionId);
            throw;
        }
        // CurrentCoins deliberately does not change: earned coins apply next run.
        return true;
    }
}
