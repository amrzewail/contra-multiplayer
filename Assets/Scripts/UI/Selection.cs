using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class Selection : MonoBehaviour
    {
        public UnityEvent OnSelect;
        public UnityEvent OnHighlight;

        public void Highlight()
        {
            transform.Find("Cursor").gameObject.SetActive(true);
            OnHighlight?.Invoke();
        }

        public void UnHighlight()
        {
            transform.Find("Cursor").gameObject.SetActive(false);
        }

        public void Select()
        {
            OnSelect?.Invoke();
        }
    }
}