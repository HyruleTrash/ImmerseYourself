using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AreYouSmartEnoughGame : MiniGame
{
    [SerializeField]
    private GameObject controls;
    private bool controlsExplained = false;
    
    [FormerlySerializedAs("Timer")] [SerializeField]
    private Timer timer;
    
    [SerializeField]
    private TextMeshProUGUI questionText;
    [SerializeField]
    private TextMeshProUGUI answerText;
    
    [Serializable]
    public class Question
    {
        public string question;
        public string answer;
    }
    [SerializeField]
    private List<Question> questions = new();
    private List<Question> hadQuestions = new();
    private Question currentQuestion;
    private string currentAnswer; // user input
    
    private int hadQuestionsCount;
    [SerializeField]
    private int maxQuestionsPerRound = 4;
    
    private void Start()
    {
        gameId = MiniGames.AreYouSmartEnough;
        timer.onTimerEnd.AddListener(OnTimerTimeOut);
        controls.SetActive(false);
    }

    public override void StartMiniGame()
    {
        Debug.Log("Are you smart enough has been started..");
        
        // check state of had questions
        if (hadQuestions.Count >= questions.Count)
            hadQuestions.Clear();
        
        // Reset minigame data
        hadQuestionsCount = 0;
        
        // prompt that explains controls
        if (!controlsExplained)
        {
            controls.SetActive(true);
            controlsExplained = true;
        }
        else
        {
            TriggerQuestionPrompt();
        }
    }

    public override void MiniGameFinished()
    {
        timer.gameObject.SetActive(false);
        controls.SetActive(false);
        base.MiniGameFinished();
    }

    public void TriggerQuestionPrompt()
    {
        // on enter start timer, and prompt math question
        timer.Reset();
        timer.StartTimer();
        
        // retrieve question
        currentQuestion = questions[Random.Range(0, questions.Count)];
        while (hadQuestions.Contains(currentQuestion))
        {
            currentQuestion = questions[Random.Range(0, questions.Count)];
            if (!hadQuestions.Contains(currentQuestion))
                break;
        }
        
        // set question
        questionText.text = currentQuestion.question;
        
        // set question visibility
        questionText.gameObject.SetActive(true);
        answerText.gameObject.SetActive(true);
    }
    
    public void OnTimerTimeOut()
    {
        // TODO: Answer wrong trigger here
        Debug.Log("Timeout!");

        CheckAnswer();
    }

    private void Update()
    {
        if (Input.inputString == "")
            return;

        string[] numbers = {
            "0","1","2","3","4","5","6","7","8","9"
        };
        if (numbers.Contains(Input.inputString))
        {
            currentAnswer += Input.inputString;
            answerText.text = currentAnswer;
        }
        
        if (!Input.GetKeyDown(KeyCode.KeypadEnter)) return;
        CheckAnswer();
    }

    public void CheckAnswer()
    {
        // register had question
        hadQuestions.Add(currentQuestion);
        hadQuestionsCount++;
        
        // on answer the timer gets shorter
        timer.SetMaxTimeShorter();
        
        // check answer
        if (currentQuestion.answer == currentAnswer)
        {
            // TODO: do something
            Debug.Log("RightAnswer");
        }
        
        // reset current input
        currentAnswer = "";
        answerText.text = "";

        // check if round is over
        if (hadQuestionsCount >= maxQuestionsPerRound)
        {
            Debug.Log("Are you smart enough, round finished!");
            MiniGameFinished();
            return;
        }
        
        // trigger next question
        TriggerQuestionPrompt();
    }
}