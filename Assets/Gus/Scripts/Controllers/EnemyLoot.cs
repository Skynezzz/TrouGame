using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LootEntry
{
    public string itemName;
    public int quantity;
}

public class EnemyLoot : MonoBehaviour,ILootable
{
    [Header("Loot")]
    public List<LootEntry> lootTable = new List<LootEntry>();
    public float interactRange = 2f;

    [Header("UI")]
    public GameObject lootIndicator; 
    
    private bool isLooted = false;
    private bool isDead   = false;
    private Transform player;
    void Start()
    {
        // Récupère le joueur une seule fois
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        // Indicateur désactivé au départ
        if (lootIndicator != null) lootIndicator.SetActive(false);
    }
    
    void Update()
    {
        if (!isDead || isLooted || player == null || lootIndicator == null) return;

        float sqrDist  = (transform.position - player.position).sqrMagnitude;
        bool  inRange  = sqrDist <= interactRange * interactRange;

        // Active/désactive uniquement si l'état change
        if (lootIndicator.activeSelf != inRange)
            lootIndicator.SetActive(inRange);
    }

    // Appelé par EnemyController quand l'ennemi meurt
    public void OnEnemyDied()
    {
        isDead = true;
        Debug.Log($"[LOOT] {gameObject.name} est mort — loot disponible !");
    }

    public bool CanBeLooted() => isDead && !isLooted;

    public List<LootEntry> CollectLoot()
    {
        if (!CanBeLooted())
        {
            Debug.LogWarning("[LOOT] Impossible de looter : déjà looté ou pas encore mort.");
            return null;
        }

        isLooted = true;

        // Cache l'indicateur dès que le loot est collecté
        if (lootIndicator != null) lootIndicator.SetActive(false);

        Debug.Log($"[LOOT] Loot collecté sur {gameObject.name}");
        return lootTable;
    }
}   