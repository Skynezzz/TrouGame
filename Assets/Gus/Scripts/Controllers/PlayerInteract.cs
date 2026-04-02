using UnityEngine;
using System.Collections.Generic;

public class PlayerInteract : MonoBehaviour
{
    public float interactRange = 3f;
    public PlayerInventory inventory;

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
            TryLoot();
    }

    void TryLoot()
    {
        int lootLayer  = LayerMask.GetMask("DeadEnemy", "Container");
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, lootLayer);

        foreach (Collider hit in hits)
        {
            // ✅ On cherche l'interface, peu importe le type d'objet
            ILootable lootable = hit.GetComponent<ILootable>();

            if (lootable != null && lootable.CanBeLooted())
            {
                List<LootEntry> items = lootable.CollectLoot();
                if (items == null) return;

                foreach (LootEntry entry in items)
                    inventory.AddItem(entry.itemName, entry.quantity);

                return;
            }
        }

        Debug.Log("[INTERACT] Rien à looter à portée.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}