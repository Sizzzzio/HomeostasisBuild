using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    // The name shown above the dialogue box (e.g. "Cloaked Automaton")
    public string npcName;

    // Optional face image shown next to the text
    public Sprite npcPortrait;

    // All the lines this NPC says, one after another
    // Press Space to go to the next line
    public DialogueLine[] lines;
