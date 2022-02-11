using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class Selection : MonoBehaviour
    {
        public UnityEvent OnSelect;

        public void Highlight()
        {
            transform.Find("Cursor").gameObject.SetActive(true);
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