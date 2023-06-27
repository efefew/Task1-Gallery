using TMPro;

using UnityEngine;

public class Errors : MonoBehaviour
{
    private string output;
    private string stack;
    public GameObject label, LogMenu;
    public Transform content;

    private void OnEnable() => Application.logMessageReceived += HandleLog;

    private void OnDisable() => Application.logMessageReceived -= HandleLog;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            LogMenu.SetActive(!LogMenu.activeInHierarchy);
        }
    }
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;
        TMP_Text text = Instantiate(label, content).GetComponent<TMP_Text>();
        text.text = output;
        switch (type)
        {
            case LogType.Error: text.color = Color.red; break;
            case LogType.Warning: text.color = Color.yellow; break;
            case LogType.Assert: text.color = Color.gray; break;
            case LogType.Exception: text.color = Color.red; break;
            case LogType.Log: text.color = Color.white; break;
            default: break;
        }
    }
}
