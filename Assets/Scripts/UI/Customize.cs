using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

namespace UI
{
    public class Customize : MonoBehaviour
    {

        public Selections menuSelections;
        public Selections colorSelections;
        public Image playerImage;

        private Selections _currentSelections;


        private void Start()
        {
            Color selectedColor = Color.blue;
            if(ColorUtility.TryParseHtmlString(PlayerPrefs.GetString("player_color", $"#{ColorUtility.ToHtmlStringRGB(Color.blue)}"), out selectedColor))
            {
                playerImage.color = selectedColor;
            }

            colorSelections.Deactivate();

            _currentSelections = menuSelections;
            menuSelections.Activate();
        }

        public void UpCallback(CallbackContext context)
        {
            if (context.phase == UnityEngine.InputSystem.InputActionPhase.Started)
                _currentSelections.Up();
        }

        public void DownCallback(CallbackContext context)
        {
            if (context.phase == UnityEngine.InputSystem.InputActionPhase.Started)
                _currentSelections.Down();
        }

        public void RightCallback(CallbackContext context)
        {
            if (context.phase == UnityEngine.InputSystem.InputActionPhase.Started)
                _currentSelections.Right();
        }

        public void LeftCallback(CallbackContext context)
        {
            if (context.phase == UnityEngine.InputSystem.InputActionPhase.Started)
                _currentSelections.Left();
        }

        public void SelectCallback(CallbackContext context)
        {
            if (context.phase == UnityEngine.InputSystem.InputActionPhase.Started)
                _currentSelections.Select();
        }

        public void ColorHighlightCallback(Color color)
        {
            playerImage.color = color;

        }

        public void ColorSelectionCallback(Color color)
        {
            playerImage.color = color;
            PlayerPrefs.SetString("player_color", $"#{ColorUtility.ToHtmlStringRGB(color)}");

            _currentSelections.Deactivate();
            _currentSelections = menuSelections;
            _currentSelections.Activate();
        }

        public void PickColorCallback()
        {
            _currentSelections.Deactivate();
            _currentSelections = colorSelections;
            _currentSelections.Activate();
        }

        public void BackCallback()
        {
            SceneManager.LoadScene((int)SceneIndex.MainMenu);
        }

    }
}