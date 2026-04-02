using System.Collections.Generic;

public interface ILootable
{
    bool CanBeLooted();
    List<LootEntry> CollectLoot();
}