using Helteix.Tools.Settings;
using UnityEngine;

[AutoGenerateGameSettings, GameSettingsPath("TrouGame/Controller/Walk"), GameSettingsTitle("WalkController", "#00ffff")]
public class WalkControllerMetrics : GameSettings<WalkControllerMetrics>
{
    [Header("Step")]
    [SerializeField, Range(0, 1)] public float viewSensitivity = 0.1f;
    [SerializeField, Range(0, 1)] public float maxSlope = 0.3f;
    [SerializeField] public float maxSpeed = 20f;
    [SerializeField] public float runMaxSpeed = 20f;
    [SerializeField] public float walkForce = 20f;
    [SerializeField] public float runForce = 20f;
    [SerializeField] public float breakStrenght = 0.3f;
    [SerializeField] public float airConstraint = 0.3f;
    [SerializeField] public float garvityForce = 10f;
    [SerializeField] public float jumpForce = 50f;
    [SerializeField] public float voidJumpTime = 0.2f;
    [SerializeField] public float strafStrenght = 1f;
    [Header("Step")]
    [SerializeField] public float stepAssist;
    [SerializeField] public float bigStepAccelerationMult;
}