using System.Collections.Generic;
using UnityEngine;

// Spanish dialogue tree for NPCs.
[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "ImmersoLingo/NPC Dialogue")] // tells unity to add menu options

public class NPCDialogueData : ScriptableObject
{
    public string npcName;
    public List<DialogueNode> nodes;   // every line/branch in this NPC's conversation
    public int startNodeId = 0;        // which node the conversation opens on

    // look up a node by id used to jump to the next line after a player picks a response.
    public DialogueNode GetNode(int id)
    {
        return nodes.Find(n => n.nodeId == id);
    }
}

// one line of dialogue + the responses the player can choose from.
[System.Serializable]
public class DialogueNode
{
    public int nodeId;

    [TextArea(2, 4)]
    public string spanishLine;   // what the NPC says

    [TextArea(1, 2)]
    public string englishHint;   // optional translation/ can be used to store enlglish translation for Kyle

    public List<DialogueOption> options;   // player's possible responses
}

// displays players responses
[System.Serializable]
public class DialogueOption
{
    [TextArea(1, 2)]
    public string responseText;   // text shown on the button

    public int nextNodeId;        // which node to go to if picked
    public bool endsConversation; // if true, conversation ends after this choice
}