using UnityEngine;

public class Actor : MonoBehaviour
{
    public GameObject playerOb;
    public float speakRange;
    public string Name;
    public Dialogue Text;


    public void Start()
    {
        playerOb = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update()
    {
        if (customMathf.distanceBetweenPoints(gameObject.transform.position, playerOb.transform.position) <= speakRange && Input.GetKeyDown(KeyCode.Space))
        {
            SpeakTo();
        }
    }

    // Trigger dialogue for this actor
    public void SpeakTo()
    {
        Debug.LogError(Text.RootNode);
        DialogueManager.Instance.StartDialogue(Name, Text.RootNode);
    }
}
