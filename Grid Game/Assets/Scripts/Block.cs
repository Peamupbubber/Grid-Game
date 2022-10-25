using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private Material black;

    [HideInInspector] public int num;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("Nich").GetComponent<GameManager>();
    }

    private void OnTriggerExit(Collider other)
    {
        gameManager.AddMaterial(other.GetComponent<DragAndDrop>().pieceToInput, num, black, false);
    }
}

