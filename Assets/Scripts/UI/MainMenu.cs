using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {

        public Selections selections;

        private bool _isRevealing = false;

        private IEnumerator Start()
        {
            _isRevealing = true;

            RectTransform root = (RectTransform)transform.Find("Root");
            root.transform.Find("Reveal").gameObject.SetActive(false);

            Vector3 pos = root.anchoredPosition3D;
            pos.x = Screen.width;
            root.anchoredPosition3D = pos;

            while(root.anchoredPosition3D.x > 1)
            {
                pos = root.anchoredPosition3D;
                pos.x = Mathf.MoveTowards(pos.x, 0, Time.deltaTime * 315 * (Screen.width / 800f));
                root.anchoredPosition3D = pos;
                yield return null;
            }

            root.transform.Find("Reveal").gameObject.SetActive(true);
            _isRevealing = false;
        }

        public async void SingleplayerCallback()
        {
            bool ready = await GameNetworkManager.singleton.StartSinglePlayer();
            if (ready)
            {
                SceneManager.LoadScene((int)SceneIndex.Game);
            }
            Debug.Log("Singleplayer: "+ ready);
        }

        public void MultiplayerCallback()
        {
            SceneManager.LoadScene((int)SceneIndex.Multiplayer);

            Debug.Log("Multiplayer");
        }

        public async void CustomizeCallback()
        {
            SceneManager.LoadScene((int)SceneIndex.Customize);

            Debug.Log("Customize");
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
            {
                if (_isRevealing)
                {
                    ((RectTransform)transform.Find("Root")).anchoredPosition3D = Vector3.zero;
                }
                else
                {
                    selections.Select();
                }
            }
        }

    }
}