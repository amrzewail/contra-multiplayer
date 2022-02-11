using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogDebugger : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private static LogDebugger singleton;

    // Start is called before the first frame update
    void Awake()
    {
        if (singleton)
        {
            Destroy(this.gameObject);
            return;
        }

        singleton = this;

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
        if(_text.text.Length > 1000) _text.text = _text.text.Substring(0, 1000);
    }

}
