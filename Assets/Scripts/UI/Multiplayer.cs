using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

namespace UI
{
    public class Multiplayer : MonoBehaviour
    {

        public Selections selections;

        public TextMeshProUGUI joinSearchingText;
        public TextMeshProUGUI invadeSearchingText;

        private void Start()
        {
            joinSearchingText.gameObject.SetActive(false);
            invadeSearchingText.gameObject.SetActive(false);
        }

        public void HostSession()
        {
            joinSearchingText.gameObject.SetActive(false);
            invadeSearchingText.gameObject.SetActive(false);

            GameManager.instance.isInvader = false;
            GameNetworkManager.singleton.HostMultiplayer();
            Debug.Log("Host");
        }

        public void JoinSession()
        {
            joinSearchingText.gameObject.SetActive(true);
            invadeSearchingText.gameObject.SetActive(false);

            GameManager.instance.isInvader = false;
            GameNetworkManager.singleton.StopSearching();
            GameNetworkManager.singleton.JoinMultiplayer();

            Debug.Log("Join");
        }

        public void Invade()
        {
            joinSearchingText.gameObject.SetActive(false);
            invadeSearchingText.gameObject.SetActive(true);

            GameManager.instance.isInvader = true;
            GameNetworkManager.singleton.StopSearching();
            GameNetworkManager.singleton.JoinMultiplayer();

            Debug.Log("Invade");
        }
        public void Back()
        {
            GameManager.instance.isInvader = false;
            GameNetworkManager.singleton.StopSearching();
            SceneManager.LoadScene((int)SceneIndex.MainMenu);
        }

        public void UpCallback(CallbackContext context)
        {
            if (context.phase == UnityEngine.InputSystem.InputActionPhase.Started)
                selections.Up();
        }

        public void DownCallback(CallbackContext context)
        {
            if (context.phase == UnityEngine.InputSystem.InputActionPhase.Started)
                selections.Down();
        }

        public void SelectCallback(CallbackContext context)
        {
            if (context.phase == UnityEngine.InputSystem.InputActionPhase.Started)
                selections.Select();
        }

    }
}