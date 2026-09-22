using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// pure UI renderer
public class DialogueUIController : MonoBehaviour
{
    public TMP_Text dialogueText;          // shows NPC's line
    public TMP_Text hintText;              // shows englishHint (optional)
    public Transform buttonContainer;      // parent object that holds response buttons
    public Button buttonPrefab;            // a simple button prefab to spawn per option

    // NPCController subscribes to this to find out which option was picked
    public event Action<int> OnOptionSelected;

    private readonly List<GameObject> spawnedButtons = new List<GameObject>();

    // called by NPCController whenever a new node needs to be shown
    public void DisplayNode(DialogueNode node)
    {
        dialogueText.text = node.spanishLine;
        hintText.text = node.englishHint;

        ClearButtons();

        for (int i = 0; i < node.options.Count; i++)
        {
            int optionIndex = i;   // capture the current value, not the loop variable itself
            DialogueOption option = node.options[i];

            Button newButton = Instantiate(buttonPrefab, buttonContainer);
            newButton.GetComponentInChildren<TMP_Text>().text = option.responseText;
            newButton.onClick.AddListener(() => OnOptionSelected?.Invoke(optionIndex));

            spawnedButtons.Add(newButton.gameObject);
        }
    }

    // removes last conversation's buttons before showing new set
    private void ClearButtons()
    {
        foreach (GameObject button in spawnedButtons)
        {
            Destroy(button);
        }
        spawnedButtons.Clear();
    }
}