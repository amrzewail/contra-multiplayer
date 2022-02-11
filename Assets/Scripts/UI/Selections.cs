using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{
    public class Selections : MonoBehaviour
    {
        private List<Selection> _selections;

        private int _currentIndex = 0;

        private void Start()
        {
            _selections = GetComponentsInChildren<Selection>().ToList();

            _currentIndex = 0;
            Highlight(0);
        }

        private void Highlight(int index)
        {
            for (int i = 0; i < _selections.Count; i++)
            {
                if (i == index)
                {
                    _selections[i].Highlight();
                }
                else
                {
                    _selections[i].UnHighlight();
                }
            }
        }

        public void Up()
        {
            _currentIndex--;
            if (_currentIndex < 0) _currentIndex = _selections.Count - 1;
            Highlight(_currentIndex);
        }

        public void Down()
        {
            _currentIndex++;
            _currentIndex %= _selections.Count;
            Highlight(_currentIndex);
        }

        public void Select()
        {
            _selections[_currentIndex].Select();
        }
    }
}