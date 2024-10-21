using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Card : MonoBehaviour
{
    public Image frontImage;
    public Button button;
    private Sprite cardImage;
    private bool isRevealed = false;
    private MemoryGameManager gameManager;

    public void Initialize(Sprite image, MemoryGameManager manager)
    {
        cardImage = image;
        frontImage.sprite = cardImage;
        button.onClick.AddListener(OnCardClicked);
        gameManager = manager;
    }

    void OnCardClicked()
    {
        if (!isRevealed)
        {
            RevealCard();
            gameManager.OnCardRevealed(this);
        }
    }

    public void RevealCard()
    {
        frontImage.gameObject.SetActive(true);
        isRevealed = true;
    }

    public void HideCard()
    {
        frontImage.gameObject.SetActive(false);
        isRevealed = false;
    }

    public bool IsRevealed => isRevealed;

    public Sprite GetImage()
    {
        return cardImage;
    }
}
