using UnityEngine;
using UnityEngine.Events;

namespace TrouGame.Miscellaneous
{
	public class TriggerUnityEvent : MonoBehaviour
	{
        public UnityEvent OnTriggerEnterAction;
        public UnityEvent OnTriggerStayAction;
        public UnityEvent OnTriggerExitAction;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                OnTriggerEnterAction.Invoke();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")
                OnTriggerStayAction.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
                OnTriggerExitAction.Invoke();
        }
    }
}