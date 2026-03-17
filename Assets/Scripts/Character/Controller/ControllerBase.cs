using UnityEngine;

namespace TrouGame.Character.Controller
{
    public abstract class ControllerBase : MonoBehaviour
    {
        public abstract void MouseMove(Vector2 playerInput);
        public abstract void DirectionChange(Vector2 playerInput);
        public abstract void ControllerStart();
        public abstract void ControllerStop();
        public abstract void Jump();
        public abstract void Sprint();
        public abstract void Crouch();
    } 
}