using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private DialogueSystem.Dialogue dialogue;
    [SerializeField] private DialogueSystem dialogueSystem;

    private void OnTriggerStay(Collider collision)
    {
        if (collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            dialogueSystem.StartDialogue(dialogue);
        }
    }
}
