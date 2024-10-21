using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class MemoryGameManager : MonoBehaviour
{
    public MemoryGameManager manager;

    public GameObject cardPrefab;
    public Transform cardParent;
    public TextMeshProUGUI cardNamesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI endGameText;

    private List<GameObject> cards = new List<GameObject>();
    private List<Sprite> shuffledImages = new List<Sprite>(); // Lista para cartas mezcladas
    private List<GameObject> matchedCards = new List<GameObject>(); // Lista para cartas emparejadas
    public IEnumerable<GameObject> completedMatchedCards = new List<GameObject>();

    private Card firstRevealed;
    private Card secondRevealed;

    public TextMeshProUGUI attemptsText;

    public CardObject[] cardObjects;
    public int maxPairs = 6;

    public Image firstCardImageDisplay;

    private bool isCheckingMatch = false;

    public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntaje: " + MemoryGameManagerExtension.score;
        }
    }

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        // Filtrar solo los primeros objetos de tipo Sprite
        var validCardImages = cardObjects.Select(co => co.sprite).OfType<Sprite>().Take(maxPairs).ToList();

        // Obtener la primera imagen de carta disponible de manera segura
        var firstCardImage = validCardImages.FirstOrDefault();

        if (validCardImages.FirstOrDefault() == null)
        {
            Debug.LogError("No hay imágenes de cartas disponibles para iniciar el juego.");
            return;
        }

        firstCardImageDisplay.sprite = firstCardImage;

        // Duplica imágenes para tener pares
        shuffledImages = validCardImages.Concat(validCardImages).OrderBy(i => Random.Range(0, validCardImages.Count)).ToList();

        // Crea las cartas
        for (int i = 0; i < shuffledImages.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, cardParent);
            card.GetComponent<Card>().Initialize(shuffledImages[i], this);
            cards.Add(card);
        }
    }

    public void OnCardRevealed(Card card)
    {
        if (isCheckingMatch) return;

        if (firstRevealed == null)
        {
            firstRevealed = card;
        }
        else if (secondRevealed == null)
        {
            manager.AddAttempt();
            secondRevealed = card;
            StartCoroutine(CheckMatch());
        }

        UpdateRevealedCardNames();
    }

    private IEnumerator CheckMatch() // Ver si las cartas coinciden
    {
        isCheckingMatch = true;

        if (firstRevealed.GetImage() == secondRevealed.GetImage())
        {
            matchedCards.Add(firstRevealed.gameObject);
            matchedCards.Add(secondRevealed.gameObject);

            manager.AddPoints(1);

            UpdateRevealedCardNames(true); // Ocultar texto después de 1 segundo
            cardNamesText.color = Color.green;
            yield return new WaitForSeconds(1);

            if (matchedCards.Count == cards.Count)
            {
                ShowFinalResult(EndGame());
            }
        }
        else
        {
            yield return new WaitForSeconds(1);
            firstRevealed.HideCard();
            secondRevealed.HideCard();
            UpdateRevealedCardNames();
        }

        firstRevealed = null;
        secondRevealed = null;

        completedMatchedCards = manager.GetMatchedCards();

        isCheckingMatch = false;
    }

    private void UpdateRevealedCardNames(bool hideAfterDelay = false) // Mostrar nombre de cartas en pantalla
    {
        var revealedCardNames = cards.Where(card => card.GetComponent<Card>().IsRevealed && !matchedCards.Contains(card))
                                     .Select(card => card.GetComponent<Card>().GetImage().name)
                                     .ToList();

        if (revealedCardNames.Any())
        {
            cardNamesText.text = "Cartas: " + string.Join(", ", revealedCardNames);
            if (!hideAfterDelay)
            {
                cardNamesText.color = Color.black;
            }
        }
        else
        {
            cardNamesText.text = string.Empty;
        }

        if (hideAfterDelay)
        {
            StartCoroutine(HideTextAfterDelay(1f));
        }
    }

    private IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        cardNamesText.text = string.Empty;
    }

    private Tuple<int, int, IEnumerable> EndGame()
    {
        Tuple<int, int, IEnumerable> finalScore = new Tuple<int, int, IEnumerable>(MemoryGameManagerExtension.score, MemoryGameManagerExtension.attempts, completedMatchedCards);

        return finalScore;
    }

    private void ShowFinalResult(Tuple<int, int, IEnumerable> result)
    {
        string resultText = $"Puntaje: {result.Item1}, Intentos: {result.Item2}";
        if (endGameText != null)
        {
            endGameText.text = resultText;
            endGameText.color = Color.blue;
        }
    }
}
