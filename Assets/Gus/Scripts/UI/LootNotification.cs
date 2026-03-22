using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LootNotification : MonoBehaviour
{
    public static LootNotification Instance { get; private set; }

    [Header("UI")]
    public TMP_Text notificationText;

    [Header("Timing")]
    public float displayDuration = 2.5f;
    public float fadeDuration    = 0.5f;

    private Coroutine currentCoroutine;
    private Queue<string> messageQueue = new Queue<string>();
    private bool isDisplaying = false;

    // -------------------------------------------------------------------------
    // SINGLETON
    // -------------------------------------------------------------------------

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        notificationText.alpha = 0f;
    }

    // -------------------------------------------------------------------------
    // PUBLIC API
    // -------------------------------------------------------------------------

    // Appelé depuis PlayerInventory
    public void ShowLoot(string itemName, int quantity)
    {
        messageQueue.Enqueue($"+ {quantity}  {itemName}");

        if (!isDisplaying)
            currentCoroutine = StartCoroutine(ProcessQueue());
    }

    // -------------------------------------------------------------------------
    // COROUTINE
    // -------------------------------------------------------------------------

    IEnumerator ProcessQueue()
    {
        isDisplaying = true;

        while (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();

            yield return StartCoroutine(FadeText(message));

            // Petite pause entre deux messages
            if (messageQueue.Count > 0)
                yield return new WaitForSeconds(0.15f);
        }

        isDisplaying = false;
    }

    IEnumerator FadeText(string message)
    {
        notificationText.text  = message;

        // Fade in
        yield return StartCoroutine(SetAlpha(0f, 1f, fadeDuration));

        // Affichage
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        yield return StartCoroutine(SetAlpha(1f, 0f, fadeDuration));
    }

    IEnumerator SetAlpha(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            notificationText.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        notificationText.alpha = to;
    }
}