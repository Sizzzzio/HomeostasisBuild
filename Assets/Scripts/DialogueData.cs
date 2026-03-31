using UnityEngine;
 
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData: ScriptableObject
{
    [Header("NPC Info")]
    public string npcName;          
 
    [Header("Conversation")]
    public DialogueLine[] lines;     // All the lines in this conversation, in order
}
 
