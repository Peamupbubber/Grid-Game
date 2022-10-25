using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    private Button button;
    private GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.Find("Nich").GetComponent<GameManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(gameManager.StartGame);
    }
}
