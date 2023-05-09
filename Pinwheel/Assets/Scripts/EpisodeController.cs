using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class EpisodeController : MonoBehaviour
{
    public GameObject quizPanel;
    public GameObject childPanel;
    public List<Button> buttons;
    public List<Sprite> sprites;
    public Sprite pw_0;
    public Sprite pw_1;
    public Sprite pw_2;
    public Sprite pw_3;
    public Sprite pw_4;
    public Sprite pw_5;
    public Button submitButton;
    public Button closeButton;
    private List<int> selectedButtons;
    private List<int> truthIndices = new List<int> { 0, 1 };
    private GameObject messagePanel;

    private PlayerMovements playerMovement;
    private bool nextLevel = false;
    public GameObject counterPanel;

    void Start()
    {
        selectedButtons = new List<int>();
        messagePanel = quizPanel.transform.Find("messagePanel").gameObject;
        initialProcesses();
        submitFunctions();
        CloseFunctions();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            playerMovement = collision.gameObject.GetComponent<PlayerMovements>();

            if (playerMovement.score == 6)
            {
                playerMovement.isMoveEnabled = false;
                playerMovement.respawnPoint = new Vector3(transform.position.x, transform.position.y + 3, 0);
                quizPanel.SetActive(true);
                quizPanel.transform.DOScale(Vector3.one, 0.5f);
                StartCoroutine(ShowMessageDelayed("Bilgilendirme", "Ayný renklere sahip iki rüzgar gülünü seçiniz.", MessageController.MessageType.Info, 0.2f));
            }

        }
    }

    private (List<Sprite>, List<int>) ShuffleSprites(List<Sprite> sprites)
    {
        List<Sprite> randomSprites = new List<Sprite>();
        List<int> indexList = new List<int>();
        List<int> indices = Enumerable.Range(0, sprites.Count).ToList();

        while (sprites.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, sprites.Count);
            randomSprites.Add(sprites[randomIndex]);
            indexList.Add(indices[randomIndex]);
            sprites.RemoveAt(randomIndex);
            indices.RemoveAt(randomIndex);
        }

        return (randomSprites, indexList);
    }

    private void clickPinwheelButton(int originalIndex)
    {
        switch (selectedButtons.Count)
        {
            case 0:
                selectedButtons.Add(originalIndex);
                break;
            case 1:
                if (!selectedButtons.Contains(originalIndex))
                    selectedButtons.Add(originalIndex);
                else selectedButtons.Remove(originalIndex);

                break;
            case 2:
                if (!selectedButtons.Contains(originalIndex))
                    selectedButtons[1] = originalIndex;
                else selectedButtons.Remove(originalIndex);

                break;
        }
    }

    private void updateColors(List<int> indexList)
    {
        int i = 0;

        foreach (var button in buttons)
        {
            Image image = button.GetComponent<Image>();

            if (selectedButtons.Contains(indexList[i]))
                image.color = new Color(0f, 1f, 0f, 0.2f);
            else
                image.color = new Color(1f, 1f, 1f, 0.2f);

            i++;
        }
    }

    private void submitFunctions()
    {
        submitButton.onClick.AddListener(() =>
        {
            switch (checkEpisodeStatus())
            {
                case 0:
                    showMessage("Hata", "Lütfen iki rüzgar gülü seçiniz.", MessageController.MessageType.Error);
                    break;

                case 1:
                    showMessage("Uyarý", "Seçimleriniz hatalý lütfen dikkatli inceleyiniz", MessageController.MessageType.Warning);
                    break;

                case 2:
                    showMessage("Bilgilendirme", "Çok yaklaþtýnýz biraz daha dikkatli inceleyiniz.", MessageController.MessageType.Info);
                    break;

                case 3:
                    showMessage("Baþarýlý", "Tebrikler eþleþtirme baþarýlý.", MessageController.MessageType.Success);
                    if (!nextLevel)
                    {
                        nextLevel = true;
                        updateSubmitButton();
                    }
                    else
                    {
                        StartCoroutine(CountdownToNextLevel());
                    }
                    break;
            }
        });
    }

    private void CloseFunctions()
    {
        closeButton.onClick.AddListener(() =>
        {
            playerMovement.isMoveEnabled = true;
            quizPanel.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => quizPanel.SetActive(false));
        });
    }

    private void showMessagePanel()
    {
        messagePanel.transform.localScale = Vector3.zero;
        messagePanel.SetActive(true);
        messagePanel.transform.DOScale(Vector3.one, 0.5f);
    }

    private void showMessage(String title, String message, MessageController.MessageType messageType)
    {
        try
        {
            showMessagePanel();
            MessageController messageController = FindObjectOfType<MessageController>();
            messageController.ShowMessage(title, message, messageType);
        }
        catch { 
        }
    }

    private void showNextLevelCounter(string message)
    {
        counterPanel.transform.localScale = Vector3.zero;
        counterPanel.SetActive(true);
        counterPanel.transform.DOScale(Vector3.one, 0.5f);
        CounterController counter = FindObjectOfType<CounterController>();
        counter.ShowMessage(message);
    }

    private IEnumerator ShowMessageDelayed(string title, string message, MessageController.MessageType messageType, float delay)
    {
        yield return new WaitForSeconds(delay);
        showMessage(title, message, messageType);
    }

    private int checkEpisodeStatus()
    {
        if (selectedButtons.Count != 2) // eksik seçilme durumu
            return 0;

        else if (selectedButtons.Contains(truthIndices[0]) && selectedButtons.Contains(truthIndices[1])) // ikisinin de doðru olma durumu 
            return 3;

        else if (truthIndices.Intersect(selectedButtons).Any()) // bir tanesinin doðru olma durumu
            return 2;

        else // hiçbirinin doðru olmama durumu
            return 1;
    }

    private void initialProcesses()
    {
        Button[] childButtons = childPanel.GetComponentsInChildren<Button>();
        buttons.AddRange(childButtons);

        sprites.AddRange(new Sprite[] { pw_0, pw_1, pw_2, pw_3, pw_4, pw_5 });

        (List<Sprite> randomSprites, List<int> indexList) = ShuffleSprites(sprites);

        List<(Button, Sprite, int)> buttonSpritePairs = Enumerable.Range(0, buttons.Count)
        .Zip(randomSprites, (i, s) => (buttons[i], s))
        .Select(pair => (pair.Item1, pair.Item2, indexList[randomSprites.IndexOf(pair.Item2)]))
        .ToList();

        buttonSpritePairs.ForEach(pair =>
        {
            Button button = pair.Item1;
            Sprite sprite = pair.Item2;
            int index = pair.Item3;

            updateColors(indexList);

            button.onClick.AddListener(() =>
            {
                clickPinwheelButton(index);
                updateColors(indexList);
            });

            Image[] images = button.GetComponentsInChildren<Image>();
            if (images.Length == 2)
            {
                images[1].sprite = sprite;
            }
        });
    }
    
    private void updateSubmitButton()
    {
        if (nextLevel)
        {
            Image image = submitButton.GetComponent<Image>();
            if (image != null)
            {
                Sprite sprite = Resources.Load<Sprite>("right_arrow_red");
                image.sprite = sprite;
                image.rectTransform.sizeDelta = new Vector2(80, 80);

                TextMeshProUGUI tmp = submitButton.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null) tmp.gameObject.SetActive(false);


                ColorBlock newColors = submitButton.colors;
                newColors.normalColor = new Color(1f, 1f, 1f, 0.8f);
                newColors.highlightedColor = new Color(1f, 1f, 1f, 0.9f);
                newColors.pressedColor = new Color(1f, 1f, 1f, 1f);
                newColors.selectedColor = new Color(1f, 1f, 1f, 0.9f);
                newColors.disabledColor = new Color(1f, 1f, 1f, 0.8f);
                submitButton.colors = newColors;
            }
        }
    }

    private IEnumerator CountdownToNextLevel()
    {
        int count = 4;
        while (count > 0)
        {
            showNextLevelCounter(count.ToString());
            yield return new WaitForSeconds(1f);
            count--;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
