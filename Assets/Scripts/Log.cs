using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Log : MonoBehaviour
{
    [SerializeField] Text question;
    [SerializeField] Text answer;

    public void SetComponent()
    {
        question = transform.GetChild(0).GetComponent<Text>();
        answer = transform.GetChild(1).GetComponent<Text>();
    }

    public void SetAnchor()
    {
        RectTransform rect = GetComponent<RectTransform>();

        // ¾ÞÄ¿¸¦ Áß¾Ó »ó´ÜÀ¸·Î ¼³Á¤
        rect.anchorMin = new Vector2(0.5f, 1); // Áß¾Ó »ó´Ü
        rect.anchorMax = new Vector2(0.5f, 1); // Áß¾Ó »ó´Ü
    }

    public void SetQuestion(string question)
    {
        this.question.text = "Question : " + question;
    }

    public void SetAnswer(string answer)
    {
        this.answer.text = "Answer : " + answer;
    }
}
