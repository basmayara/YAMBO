using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YAMBO.API;
using YAMBO.UI;
using System.Collections.Generic;

namespace YAMBO.Quiz
{
    /// <summary>
    /// Manages the quiz gameplay panel.
    /// Updated: sends time_taken for anti-cheat, shows explanation, uses HUDManager for balance.
    /// </summary>
    public class QuizManager : MonoBehaviour
    {
        [Header("Question")]
        public TextMeshProUGUI questionText;
        public TextMeshProUGUI categoryText;
        public TextMeshProUGUI difficultyText;
        public TextMeshProUGUI rewardText;

        [Header("Timer (optional)")]
        public Slider          timerSlider;
        public TextMeshProUGUI timerText;

        [Header("Answer Buttons (4)")]
        public Button[] answerButtons;

        [Header("Feedback")]
        public GameObject      feedbackPanel;
        public TextMeshProUGUI feedbackText;
        public TextMeshProUGUI explanationText;
        public Button          nextQuizButton;

        [Header("State")]
        public GameObject loadingPanel;

        private QuizQuestion _currentQuiz;
        private float        _questionStartTime;
        private float        _timeLimit;
        private bool         _answered;

        void Start()
        {
            feedbackPanel?.SetActive(false);
            nextQuizButton?.onClick.AddListener(LoadNewQuiz);
            LoadNewQuiz();
        }

        void Update()
        {
            if (_currentQuiz == null || _answered || _timeLimit <= 0f) return;

            float elapsed  = Time.time - _questionStartTime;
            float remaining = Mathf.Max(0f, _timeLimit - elapsed);

            if (timerSlider != null)
                timerSlider.value = remaining / _timeLimit;

            if (timerText != null)
                timerText.text = Mathf.CeilToInt(remaining) + "s";

            if (remaining <= 0f)
                OnTimerExpired();
        }

        public void LoadNewQuiz()
        {
            _answered = false;
            feedbackPanel?.SetActive(false);
            loadingPanel?. SetActive(true);

            foreach (var btn in answerButtons)
                btn.interactable = false;

            StartCoroutine(APIClient.Instance.GetRandomQuiz(OnQuizLoaded,
                e =>
                {
                    Debug.LogError("[Quiz] Load error: " + e);
                    loadingPanel?.SetActive(false);
                }));
        }

        private void OnQuizLoaded(QuizQuestion quiz)
        {
            _currentQuiz = quiz;
            loadingPanel?.SetActive(false);

            // Question header
            questionText.text   = quiz.question;
            if (categoryText   != null) categoryText.text   = quiz.category;
            if (difficultyText != null) difficultyText.text = quiz.difficulty;
            if (rewardText     != null) rewardText.text     = "+" + quiz.reward_currency + " C#";

            // Timer setup
            _timeLimit          = quiz.time_limit > 0 ? quiz.time_limit : 0f;
            _questionStartTime  = Time.time;
            if (timerSlider != null) timerSlider.value = 1f;

            // Shuffle answers using model helper
            string[] shuffled = quiz.GetShuffledAnswers();

            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (i < shuffled.Length)
                {
                    answerButtons[i].gameObject.SetActive(true);
                    answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = shuffled[i];
                    answerButtons[i].interactable = true;

                    string captured = shuffled[i];
                    answerButtons[i].onClick.RemoveAllListeners();
                    answerButtons[i].onClick.AddListener(() => OnAnswerSelected(captured));
                }
                else
                {
                    answerButtons[i].gameObject.SetActive(false);
                }
            }
        }

        private void OnAnswerSelected(string selectedAnswer)
        {
            if (_answered) return;
            _answered = true;

            foreach (var btn in answerButtons) btn.interactable = false;
            loadingPanel?.SetActive(true);

            int timeTaken = Mathf.RoundToInt(Time.time - _questionStartTime);

            StartCoroutine(APIClient.Instance.SubmitQuizAnswer(
                _currentQuiz.quiz_id,
                selectedAnswer,
                timeTaken,
                OnQuizSubmitted,
                e =>
                {
                    Debug.LogError("[Quiz] Submit error: " + e);
                    loadingPanel?.SetActive(false);
                }));
        }

        private void OnQuizSubmitted(QuizSubmitResponse response)
        {
            loadingPanel?.SetActive(false);
            feedbackPanel?.SetActive(true);

            if (response.is_correct)
            {
                feedbackText.text  = "Correct! +" + response.currency_earned + " C#";
                feedbackText.color = new Color(0.2f, 0.85f, 0.4f);
                HUDManager.Instance?.OnCurrencyEarned(response.currency_earned, response.new_balance);
            }
            else
            {
                feedbackText.text  = "Wrong! Answer: " + response.correct_answer;
                feedbackText.color = new Color(0.95f, 0.3f, 0.3f);
            }

            if (explanationText != null)
                explanationText.text = response.explanation ?? "";
        }

        private void OnTimerExpired()
        {
            if (_answered) return;
            _answered = true;
            foreach (var btn in answerButtons) btn.interactable = false;

            if (feedbackText != null)
            {
                feedbackText.text  = "Time's up!";
                feedbackText.color = Color.yellow;
            }
            feedbackPanel?.SetActive(true);
        }
    }
}