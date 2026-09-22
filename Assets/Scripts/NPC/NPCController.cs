using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// The "brain" of an NPC
[RequireComponent(typeof(XRSimpleInteractable))]
public class NPCController : MonoBehaviour
{
    public NPCDialogueData dialogueData;      // which conversation this NPC has
    public DialogueUIController dialogueUI;   // the UI piece this NPC talks to
    public GameObject promptPanel;            // "press to talk" panel
    public GameObject dialoguePanel;          // the actual conversation panel

    private XRSimpleInteractable interactable;
    private DialogueNode currentNode;

    // Simple state machine — mirrors the Idle -> PromptVisible -> Talking flow we designed.
    private enum State { Idle, PromptVisible, Talking }
    private State currentState = State.Idle;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        // uses XR interactive toolkit to show interactions
        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);
        interactable.activated.AddListener(OnActivated);   // trigger (needs Allow Hovered Activate on the interactors)

        // listen for the UI telling us which response the player picked
        dialogueUI.OnOptionSelected += HandleOptionSelected;
    }

    private void OnDisable()
    {
        // Always unsubscribe to avoid errors/leaks when this object is disabled or destroyed
        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.hoverExited.RemoveListener(OnHoverExited);
        interactable.activated.RemoveListener(OnActivated);
        dialogueUI.OnOptionSelected -= HandleOptionSelected;
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (currentState != State.Idle) return;   // don't show the prompt mid-conversation

        currentState = State.PromptVisible;
        promptPanel.SetActive(true);
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        if (currentState != State.PromptVisible) return;   // don't cancel an active conversation

        currentState = State.Idle;
        promptPanel.SetActive(false);
    }

    // trigger pulled while pointing at the NPC
    private void OnActivated(ActivateEventArgs args)
    {
        if (currentState == State.Talking) return;   // in a conversation

        currentState = State.Talking;
        promptPanel.SetActive(false);
        dialoguePanel.SetActive(true);

        // Start at the dialogue's designated first node
        ShowNode(dialogueData.GetNode(dialogueData.startNodeId));
    }

    // push node's data to the UI to render
    private void ShowNode(DialogueNode node)
    {
        currentNode = node;
        dialogueUI.DisplayNode(node);
    }

    // called when the UI reports which response button was clicked
    private void HandleOptionSelected(int optionIndex)
    {
        DialogueOption chosen = currentNode.options[optionIndex];

        if (chosen.endsConversation)
        {
            EndDialogue();
        }
        else
        {
            ShowNode(dialogueData.GetNode(chosen.nextNodeId));
        }
    }

    // resets state and hides panel
    private void EndDialogue()
    {
        currentState = State.Idle;
        dialoguePanel.SetActive(false);
    }
}