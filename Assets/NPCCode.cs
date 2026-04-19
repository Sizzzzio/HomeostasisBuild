using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCCode : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public string[] dialogue;
    private int index;

    public GameObject continueButton;
    public float wordSpeed;
    public bool playerIsClose;

    [Header("NPC Image")]
    public Image npcImage;              // Assign in Inspector
    public Sprite npcSprite;            // Assign your custom NPC sprite here

    void Start()
    {
        dialogueText.text = "";

        // Set custom NPC image if assigned
        if (npcImage != null && npcSprite != null)
            npcImage.sprite = npcSprite;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerIsClose)
        {
            if (dialoguePanel.activeInHierarchy)
                zeroText();
            else
            {
                dialoguePanel.SetActive(true);
                StartCoroutine(Typing());
            }
        }

        if (dialogueText.text == dialogue[index])
            continueButton.SetActive(true);
    }

    public void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
        continueButton.SetActive(false);
        StopAllCoroutines();
    }

    // Called by Player.cs on death to reset dialogue state
    public void ResetDialogue()
    {
        zeroText();
        playerIsClose = false;
    }

    IEnumerator Typing()
    {
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    public void NextLine()
    {
        continueButton.SetActive(false);

        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            zeroText();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerIsClose = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            zeroText();
        }
    }
}