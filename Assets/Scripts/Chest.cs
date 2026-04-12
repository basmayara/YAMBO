using UnityEngine;

public class Chest : MonoBehaviour
{
    private bool isInteractable = true;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // فاش كيقيس اللاعب الصندوق وهو مازال "interactable"
        if (other.CompareTag("Player") && isInteractable)
        {
            QuizManager.instance.ShowRandomQuiz(this);
        }
    }

    public void OpenChest()
    {
        isInteractable = false; // باش ما يقدش يعاود يحل الكويز
        if (anim != null)
        {
            anim.SetTrigger("Open"); // كيخدم سهم الـ Open اللي خارج من initial-box
        }
        Debug.Log("Correct Answer - Box Opening!");
    }

    public void CloseChestPermanently()
    {
        isInteractable = false; // كيتسد للأبد حيت الجواب غلط
        if (anim != null)
        {
            anim.SetTrigger("Close"); // كيخدم سهم الـ Close اللي خارج من initial-box
        }
        Debug.Log("Wrong Answer - Box Closing!");
    }
}