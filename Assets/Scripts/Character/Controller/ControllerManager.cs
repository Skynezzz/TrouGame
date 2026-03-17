using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TrouGame.Character.Controller
{
    public class ControllerManager : MonoBehaviour
    {
        [SerializeField] private List<ControllerBase> controllers = new List<ControllerBase>();
        [SerializeField] private ControllerBase currentController;

        private void controllerCheck()
        {
            if (controllers.Count == 0)
                throw new System.ArgumentNullException("controllers");
            if (!currentController)
                SetController(controllers[0]);
        }

        private void SetController(ControllerBase controller)
        {
            currentController?.ControllerStop();
            currentController = controller;
            currentController.ControllerStart();
        }

        public void Input_MouseMove(InputAction.CallbackContext ctx)
        {
            if (true || ctx.performed)
            {
                controllerCheck();
                currentController?.MouseMove(ctx.ReadValue<Vector2>());
            }
        }

        public void Input_DirectionChange(InputAction.CallbackContext ctx)
        {
            if (true || ctx.performed)
            {
                controllerCheck();
                currentController?.DirectionChange(ctx.ReadValue<Vector2>());
            }
        }

        public void Input_Jump(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                controllerCheck();
                currentController?.Jump();
            }
        }

        public void Input_Sprint(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                controllerCheck();
                currentController?.Sprint();
            }
        }

        public void Input_Crouch(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                controllerCheck();
                currentController?.Crouch();
            }
        }
    }
}