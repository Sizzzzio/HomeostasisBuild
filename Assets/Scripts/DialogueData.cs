using UnityEngine;
 
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("NPC Info")]
    public string npcName;           
    public Sprite npcPortrait;       // Face image shown in dialogue box
 
    [Header("Conversation")]
    public DialogueLine[] lines;     // All the lines in this conversation, in order
}

// All the lines this NPC says, one after another.
    // Press Space to go to the next line.
    [TextArea(2, 4)]
    public string[] lines;
}
 
