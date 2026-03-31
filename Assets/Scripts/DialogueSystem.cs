using UnityEngine;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour
{
    [System.Serializable]
    public class Dialogue
    {
        public string characterName;

        [TextArea(3, 10)]
        public string[] sentences;
    }

    private Queue<string> sentences;
    private string currentCharacter;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        sentences.Clear();
        currentCharacter = dialogue.characterName;

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        Debug.Log(currentCharacter + ": " + sentence);
    }

    void EndDialogue()
    {
        Debug.Log("End of dialogue.");
    }
}
