using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    [SerializeField] private DialogueSystem.Dialogue dialogue;
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private float range = 2f;
    
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        
        if (distance <= range && Input.GetKeyDown(KeyCode.E))
        {
            dialogueSystem.StartDialogue(dialogue);
        }
    }
}
