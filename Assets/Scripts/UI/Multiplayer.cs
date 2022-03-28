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
        public Selection invadeSelection;

        private void Awake()
        {
            invadeSelection.gameObject.SetActive(PlayerPrefs.GetInt("DidKillBoss", 0) == 1);
        }

        private void Start()
        {
            joinSearchingText.gameObject.SetActive(false);
            invadeSearchingText.gameObject.SetActive(false);
        }

        public async void HostSession()
        {
            joinSearchingText.gameObject.SetActive(false);
            invadeSearchingText.gameObject.SetActive(false);

            GameManager.instance.isInvader = false;
            GameNetworkManager.singleton.StopSearching();
            if (await GameNetworkManager.singleton.HostMultiplayer())
            {
                Debug.Log("Host Start");
                SceneManager.LoadScene((int)SceneIndex.Game);
            }
        }

        public async void JoinSession()
        {
            joinSearchingText.gameObject.SetActive(true);
            invadeSearchingText.gameObject.SetActive(false);

            GameManager.instance.isInvader = false;
            GameNetworkManager.singleton.StopSearching();
            if (await GameNetworkManager.singleton.JoinMultiplayer())
            {
                Debug.Log("Join Start");
                SceneManager.LoadScene((int)SceneIndex.Game);
            }

        }

        public async void Invade()
        {
            joinSearchingText.gameObject.SetActive(false);
            invadeSearchingText.gameObject.SetActive(true);

            GameManager.instance.isInvader = true;
            GameNetworkManager.singleton.StopSearching();
            if (await GameNetworkManager.singleton.JoinMultiplayer())
            {
                Debug.Log("Invade Start");
                SceneManager.LoadScene((int)SceneIndex.Game);
            }

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