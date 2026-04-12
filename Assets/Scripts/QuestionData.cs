using UnityEngine;

[CreateAssetMenu(fileName = "New Question", menuName = "Quiz/Question")]
public class QuestionData : ScriptableObject
{
    [TextArea(3, 10)]
    public string questionText;    // نص السؤال
    public string[] answers;       // الـ 4 أجوبة (A, B, C, D)
    public int correctAnswerIndex; // رقم الجواب الصحيح (0 لـ A، 1 لـ B...)
}