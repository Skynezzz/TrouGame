using UnityEngine;
using PrimeTween;

public class ContainerAnimator : MonoBehaviour
{
    [Header("Couvercle")]
    public Transform lid;                  // ← glisse la partie couvercle ici
    public Vector3   openRotationOffset = new Vector3(-110f, 0f, 0f); // angle d'ouverture
    public float     duration           = 0.5f;
    public Ease      ease               = Ease.OutBack;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool       isOpen = false;

    void Start()
    {
        if (lid == null)
        {
            Debug.LogWarning($"[ContainerAnimator] Aucun couvercle assigné sur {gameObject.name}");
            return;
        }

        // Sauvegarde la rotation initiale comme état fermé
        closedRotation = lid.localRotation;
        openRotation   = Quaternion.Euler(lid.localEulerAngles + openRotationOffset);
    }

    public void Open()
    {
        if (isOpen || lid == null) return;

        isOpen = true;

        Tween.LocalRotation(lid, openRotation, duration, ease);
    }
}