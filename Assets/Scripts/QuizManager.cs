using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    public static QuizManager instance;

    [Header("UI Elements")]
    public GameObject quizUI;
    public TextMeshProUGUI questionDisplayText;
    public Button[] answerButtons;
    public Button confirmButton;

    [Header("Questions Bank")]
    public List<QuestionData> allQuestions;

    private QuestionData currentQuestion;
    private int selectedAnswerIndex = -1;
    private Chest currentChest;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        quizUI.SetActive(false);
        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmAnswer);
    }

    public void ShowRandomQuiz(Chest chest)
    {
        if (allQuestions.Count == 0) return;

        currentChest = chest;
        int randomIndex = Random.Range(0, allQuestions.Count);
        currentQuestion = allQuestions[randomIndex];

        DisplayQuestion();
        quizUI.SetActive(true);
    }

    void DisplayQuestion()
    {
        questionDisplayText.text = currentQuestion.questionText;
        selectedAnswerIndex = -1;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].interactable = true;
            answerButtons[i].image.color = Color.white;

            // Mas7 l-listeners l-qdam bach may-t-jme3ouch
            answerButtons[i].onClick.RemoveAllListeners();

            int index = i;
            answerButtons[i].onClick.AddListener(() => SelectAnswer(index));

            // Isla7 l-ghalat dyal .Length blast .Count
            if (index < currentQuestion.answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[index];
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void SelectAnswer(int index)
    {
        selectedAnswerIndex = index;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].image.color = (i == index) ? Color.yellow : Color.white;
        }
    }

    public void ConfirmAnswer()
    {
        if (selectedAnswerIndex == -1) return;

        foreach (var btn in answerButtons) btn.interactable = false;

        if (selectedAnswerIndex == currentQuestion.correctAnswerIndex)
        {
            answerButtons[selectedAnswerIndex].image.color = Color.green;
            Debug.Log("Correct!");

            if (CoinManager.instance != null)
                CoinManager.instance.AddCoins(50);

            if (currentChest != null)
                currentChest.OpenChest();
        }
        else
        {
            answerButtons[selectedAnswerIndex].image.color = Color.red;
            Debug.Log("Wrong Answer!");

            if (currentChest != null)
                currentChest.CloseChestPermanently();
        }

        Invoke("HideQuiz", 1.2f);
    }

    void HideQuiz()
    {
        quizUI.SetActive(false);
    }
} // Akher bracket dyal l-Class