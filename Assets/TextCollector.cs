using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class TextCollector : MonoBehaviour
{
    void Start()
    {
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();

       var allTexts = texts.SelectMany(texts => texts.GetComponentsInChildren<TextMeshProUGUI>());

        foreach (var txt in allTexts)
        {
            txt.text = "";
            Debug.Log("Text deleted: " + txt.name);
        }
    }
}