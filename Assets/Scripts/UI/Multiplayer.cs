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

            GameNetworkManager.singleton.HostMultiplayer();
            Debug.Log("Singleplayer");
        }

        public void JoinSession()
        {
            joinSearchingText.gameObject.SetActive(true);
            invadeSearchingText.gameObject.SetActive(false);
            
            GameNetworkManager.singleton.StopSearching();
            GameNetworkManager.singleton.JoinMultiplayer();

            Debug.Log("Multiplayer");
        }

        public void Invade()
        {
            joinSearchingText.gameObject.SetActive(false);
            invadeSearchingText.gameObject.SetActive(true);

            GameNetworkManager.singleton.StopSearching();
            GameNetworkManager.singleton.JoinMultiplayer();

            Debug.Log("INVADE");
        }
        public void Back()
        {
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