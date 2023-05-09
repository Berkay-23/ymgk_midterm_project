using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CounterController : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI messageGUI;

    void Start()
    {
        panel.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        panel.SetActive(true);
        messageGUI.text = message;
    }

    public void HideMessage()
    {
        panel.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => panel.SetActive(false));
    }
}
