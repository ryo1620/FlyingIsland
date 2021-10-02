using UnityEngine;
using UnityEngine.UI;

// Android上にデバッグログの内容を表示させるためのスクリプト（テスト用）
public class LogDisplay : MonoBehaviour
{
    public Text message = null;

    private void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void HandleLog(string logText, string stackTrace, LogType type)
    {
        message.text = logText;
    }
}