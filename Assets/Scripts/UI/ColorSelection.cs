using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class ColorSelection : Selection
    {
        public Image color;

        public UnityEvent<Color> OnColorSelect;
        public UnityEvent<Color> OnColorHighlight;

        private void Start()
        {
            OnSelect.AddListener(OnSelectCallback);
            OnHighlight.AddListener(OnHighlightCallback);
        }

        private void OnSelectCallback()
        {
            OnColorSelect?.Invoke(color.color);
        }

        private void OnHighlightCallback()
        {
            OnColorHighlight?.Invoke(color.color);
        }
    }
}