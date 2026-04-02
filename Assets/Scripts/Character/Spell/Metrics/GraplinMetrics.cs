using Helteix.Tools.Settings;
using UnityEngine;

[AutoGenerateGameSettings, GameSettingsPath("TrouGame/Spells/Graplin"), GameSettingsTitle("Graplin", "#00ffff")]
public class GraplinMetrics : GameSettings<GraplinMetrics>
{
    [SerializeField] public float maxRange = 20f;
    [SerializeField] public float minRange = 1.5f;
    [SerializeField] public float speed = 10f;
    [SerializeField] public float maxGrapPullTime = 1f;
}