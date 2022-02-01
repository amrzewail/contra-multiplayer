using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogDebugger : MonoBehaviour
{
    private TextMeshProUGUI _text;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        _text = GetComponentInChildren<TextMeshProUGUI>();

        Application.logMessageReceived += Application_logMessageReceived;
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Log:
                _text.text = "* "+condition + "\n" + _text.text;
                break;
        }
    }

}
