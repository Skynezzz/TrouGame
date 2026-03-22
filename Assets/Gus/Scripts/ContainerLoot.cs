using UnityEngine;
using System.Collections.Generic;

public class ContainerLoot : MonoBehaviour, ILootable
{
    [Header("Loot")]
    public List<LootEntry> lootTable = new List<LootEntry>();

    [Header("UI")]
    public GameObject lootIndicator;
    public float interactRange = 2f;

    private bool isLooted = false;
    private Transform player;
    private ContainerAnimator containerAnimator;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        // Récupère l'animator sur le même objet
        containerAnimator = GetComponent<ContainerAnimator>();

        if (lootIndicator != null) lootIndicator.SetActive(false);
    }

    void Update()
    {
        if (isLooted || player == null || lootIndicator == null) return;

        bool inRange = (transform.position - player.position).sqrMagnitude
                       <= interactRange * interactRange;

        if (lootIndicator.activeSelf != inRange)
            lootIndicator.SetActive(inRange);
    }

    public bool CanBeLooted() => !isLooted;

    public List<LootEntry> CollectLoot()
    {
        if (!CanBeLooted())
        {
            Debug.LogWarning("[LOOT] Conteneur déjà vidé.");
            return null;
        }

        isLooted = true;

        if (lootIndicator != null) lootIndicator.SetActive(false);

        // ✅ Déclenche l'animation
        containerAnimator?.Open();

        Debug.Log($"[LOOT] Conteneur {gameObject.name} vidé.");
        return lootTable;
    }
}