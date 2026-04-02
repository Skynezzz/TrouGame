using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    private Dictionary<string, int> items = new Dictionary<string, int>();

    public void AddItem(string itemName, int quantity)
    {
        if (items.ContainsKey(itemName))
            items[itemName] += quantity;
        else
            items[itemName] = quantity;

        // ✅ Notification automatique
        LootNotification.Instance?.ShowLoot(itemName, quantity);

        Debug.Log($"[INVENTAIRE] {itemName} x{items[itemName]} (total)");
    }

    public Dictionary<string, int> GetItems() => items;
}
