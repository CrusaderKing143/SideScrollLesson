using UnityEngine;
using UnityEngine.UI;

public static class GameOverPrompt
{
    public static void Show(string title = "GAME OVER", string detail = "GAME OVER")
    {
        if (Time.timeScale == 0f)
            return;

        Time.timeScale = 0f;


        GameObject canvasObject = GameObject.Find("GameOverPromptCanvas");

        CreateText(canvasObject.transform, title, 0);
        CreateText(canvasObject.transform, detail, 1);
    }

    static void CreateText(Transform parent, string content, int size)
    {
        parent.GetChild(size).gameObject.SetActive(true);
        parent.GetChild(size).GetComponent<Text>().text = content;

    }
}
