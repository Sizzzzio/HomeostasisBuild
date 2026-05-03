using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCCode : MonoBehaviour
{
    [Header("Shared UI — same for all NPCs")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text npcNameText;
    public Image npcImage;
    public GameObject continueButton;

    [Header("This NPC's data")]
    public string npcName;
    public Sprite npcSprite;
    public string[] dialogue;
    public float wordSpeed = 0.05f;

    private int index;
    private bool playerIsClose;

    // Static so only ONE npc can be active at a time across all instances
    private static NPCCode activeNPC = null;
    private static Coroutine activeTypingCoroutine = null;

    void Update()
    {
        if (!playerIsClose) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (dialoguePanel != null && dialoguePanel.activeInHierarchy)
            {
                // Only close if THIS npc is the active one
                if (activeNPC == this)
                    CloseDialogue();
            }
            else
            {
                OpenDialogue();
            }
        }

        // Show continue button when line is fully typed
        if (activeNPC == this && dialoguePanel != null && dialoguePanel.activeInHierarchy)
        {
            if (dialogueText != null && index < dialogue.Length &&
                dialogueText.text == dialogue[index])
            {
                if (continueButton != null)
                    continueButton.SetActive(true);
            }
        }
    }

    void OpenDialogue()
    {
        // Stop any previous NPC's coroutine first
        if (activeNPC != null && activeNPC != this)
            activeNPC.ForceClose();

        activeNPC = this;
        index = 0;

        if (npcNameText != null) npcNameText.text = npcName;
        if (npcImage != null && npcSprite != null) npcImage.sprite = npcSprite;

        if (continueButton != null)
        {
            continueButton.SetActive(false);
            Button btn = continueButton.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(NextLine);
            }
        }

        dialoguePanel.SetActive(true);
        StartTyping();
    }

    void StartTyping()
    {
        // Stop any running coroutine globally
        if (activeTypingCoroutine != null)
            StopCoroutine(activeTypingCoroutine);

        if (dialogueText != null) dialogueText.text = "";
        activeTypingCoroutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        if (continueButton != null) continueButton.SetActive(false);
        if (dialogueText != null) dialogueText.text = "";

        foreach (char letter in dialogue[index].ToCharArray())
        {
            if (dialogueText != null)
                dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        activeTypingCoroutine = null;
    }

    public void NextLine()
    {
        if (activeNPC != this) return;

        if (continueButton != null) continueButton.SetActive(false);

        if (index < dialogue.Length - 1)
        {
            index++;
            StartTyping();
        }
        else
        {
            CloseDialogue();
        }
    }

    void CloseDialogue()
    {
        ForceClose();
        activeNPC = null;
    }

    // Called when another NPC takes over
    public void ForceClose()
    {
        if (activeTypingCoroutine != null)
        {
            StopCoroutine(activeTypingCoroutine);
            activeTypingCoroutine = null;
        }
        if (dialogueText != null) dialogueText.text = "";
        if (continueButton != null) continueButton.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        index = 0;
    }

    public void ResetDialogue()
    {
        CloseDialogue();
        playerIsClose = false;
    }

    public void zeroText() => CloseDialogue();

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
            if (activeNPC == this)
                CloseDialogue();
        }
    }
}