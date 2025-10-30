using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityRawInput;
using Random = UnityEngine.Random;

public class AreYouSmartEnoughGame : MiniGame
{
    [SerializeField]
    private CalculatorInterprator calculatorInterprator;
    
    [SerializeField]
    private GameObject controls;
    
    [SerializeField]
    private Timer timer;
    
    [SerializeField]
    private TextMeshProUGUI questionText;
    [SerializeField]
    private TextMeshProUGUI answerText;
    
    [SerializeField]
    private ImageFadeOut wrongAnswerVisual;
    [SerializeField]
    private ImageFadeOut correctAnswerVisual;
    
    [SerializeField]
    private QuestionList questionList;
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
        calculatorInterprator = GetComponent<CalculatorInterprator>();
        HideAll();
    }

    public override void StartMiniGame(bool shouldShowControls)
    {
        base.StartMiniGame(shouldShowControls);
        Debug.Log($"Are you smart enough has been started.. showing controls: {shouldShowControls}");
        
        // check state of had questions
        if (hadQuestions.Count >= questionList.questions.Count)
            hadQuestions.Clear();
        
        // Reset minigame data
        hadQuestionsCount = 0;
        
        // prompt that explains controls
        if (shouldShowControls)
        {
            controls.SetActive(true);
        }
        else
        {
            TriggerQuestionPrompt();
        }
    }

    public override void MiniGameFinished()
    {
        HideAll();
        ResetAnswer();
        base.MiniGameFinished();
    }

    private void HideAll()
    {
        calculatorInterprator.enabled = false;
        timer.gameObject.SetActive(false);
        controls.SetActive(false);
        questionText.gameObject.SetActive(false);
        answerText.gameObject.SetActive(false);
    }

    public void TriggerQuestionPrompt()
    {
        timer.gameObject.SetActive(true);
        calculatorInterprator.enabled = true;
        
        // on enter start timer, and prompt math question
        timer.Reset();
        timer.StartTimer();
        
        // retrieve question
        currentQuestion = questionList.GetRandomQuestion(hadQuestions);
        
        // set question
        questionText.text = currentQuestion.question + " =";
        
        // set question visibility
        questionText.gameObject.SetActive(true);
        answerText.gameObject.SetActive(true);
    }
    
    public void OnTimerTimeOut()
    {
        wrongAnswerVisual.Appear();
        wrongAnswerVisual.TriggerFadeOut();
        
        Debug.Log("Timeout!");

        CheckAnswer();
    }

    private void Update()
    {
        if (!isGameRunning)
            return;
        
        if (answerText.text != "" && RawInput.IsKeyDown(RawKey.Return))
        {
            CheckAnswer();
            return;
        }
        
        if (calculatorInterprator.inputString == "")
            return;
        
        currentAnswer = calculatorInterprator.inputString;
        answerText.text = currentAnswer;
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
            correctAnswerVisual.Appear();
            correctAnswerVisual.TriggerFadeOut();
        }
        else
        {
            wrongAnswerVisual.Appear();
            wrongAnswerVisual.TriggerFadeOut();
        }
        
        // reset current input
        ResetAnswer();

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

    private void ResetAnswer()
    {
        currentAnswer = "";
        answerText.text = "";
        calculatorInterprator.ClearInputString();
    }
}