using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public static class MemoryGameManagerExtension
{
    public static int attempts = 0;
    public static int score = 0;
    public static void AddPoints(this MemoryGameManager manager, int points)
    {
        score++;
        manager.UpdateScoreUI();
    }

    public static void AddAttempt(this MemoryGameManager manager)
    {
        attempts++;
        manager.UpdateAttemptsUI();
    }

    private static void UpdateAttemptsUI(this MemoryGameManager manager)
    {
        if (manager.attemptsText != null)
        {
            manager.attemptsText.text = "Intentos: " + attempts;
        }
    }

    public static IEnumerable<GameObject> GetMatchedCards(this MemoryGameManager manager)
    {
        return manager.completedMatchedCards.ToList();
    }
}