using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    public GameObject dialogueBubble;

    [TextArea(2, 5)]
    public string dialogueText = "Тисни C, щоб стріляти шестернями, і допоможи полагодити ці зламані роботи! Повертайся, коли закінчиш.";

    void Start()
    {
        dialogueBubble.SetActive(false);
    }
}
