using System;
using UnityEngine;

public class DialogueLine
{
    public string speakerName;   

    [TextArea(2, 5)]
    public string text;          

    public bool hasChoice = false;
    public DialogueChoice[] choices; 
}

public class DialogueChoice
{
    public string choiceText;           
    public AlignmentType alignment;      
    public int alignmentStrength = 1;   
    public int nextLineIndex = -1;       
}
