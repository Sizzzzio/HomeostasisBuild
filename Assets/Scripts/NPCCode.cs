using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCCode : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text npcNameText;        // Drag NPCName TMP text here
    public string[] dialogue;
    public string npcName;              // Type the NPC name here
    private int index;

    public GameObject continueButton;
    public float wordSpeed;
    public bool playerIsClose;

    [Header("NPC Image")]
    public Image npcImage;
    public Sprite npcSprite;

    void Start()
    {
        dialogueText.text = "";

        if (npcImage != null && npcSprite != null)
            npcImage.sprite = npcSprite;

        if (npcNameText != null)
            npcNameText.text = npcName;
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

    public void ResetDialogue()
    {
        zeroText();
        playerIsClose = false;
    }

    IEnumerator Typing()
    {
        // Clear text before typing new line
        dialogueText.text = "";
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