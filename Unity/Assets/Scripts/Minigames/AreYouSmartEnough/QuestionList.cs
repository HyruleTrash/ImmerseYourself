using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestionList", menuName = "Minigames/AreYouSmartEnough/QuestionList")]
public class QuestionList : ScriptableObject
{
    public List<Question> questions = new();

    public Question GetRandomQuestion(List<Question> hadQuestions)
    {
        int tries = 0;
        int maxTries = questions.Count;
        var currentQuestion = questions[Random.Range(0, questions.Count)];
        while (hadQuestions.Contains(currentQuestion))
        {
            currentQuestion = questions[Random.Range(0, questions.Count)];
            tries++;
            if (!hadQuestions.Contains(currentQuestion))
                break;
            if (tries > maxTries)
            {
                hadQuestions.Clear();
                currentQuestion = questions[0];
            }
        }
        return currentQuestion;
    }
}
