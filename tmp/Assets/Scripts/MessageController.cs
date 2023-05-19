using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageController : MonoBehaviour
{
    public GameObject panel;
    private TextMeshProUGUI messageTMPro;
    private TextMeshProUGUI titleTMPro;

    public void Start()
    {
        panel.SetActive(false);
        titleTMPro = panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        messageTMPro = panel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }


    public enum MessageType
    {
        Warning,
        Info,
        Success,
        Error
    }

    private Dictionary<MessageType, Color> messageTypeColors = new Dictionary<MessageType, Color>()
    {
        { MessageType.Warning, new Color(1f, 0.5f, 0f, 0.2f) },  // Orange
        { MessageType.Info, new Color(0f, 0.5f, 1f, 0.2f) },  // Blue
        { MessageType.Success, new Color(0f, 0.8f, 0f, 0.2f) },  // Green
        { MessageType.Error, new Color(1f, 0f, 0f, 0.2f) }  // Red
    };

    public void ShowMessage(string title, string message, MessageType messageType)
    {
        panel.SetActive(true);
        titleTMPro.text = title;
        messageTMPro.text = message;
        panel.GetComponent<Image>().color = messageTypeColors[messageType];
        Invoke(nameof(HideMessage), 1f);
    }


    public void HideMessage()
    {
        panel.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => panel.SetActive(false));
    }
}
