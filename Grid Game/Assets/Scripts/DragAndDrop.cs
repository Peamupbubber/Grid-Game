using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] private GameObject blackBlock;
    [SerializeField] private Material material;
    [SerializeField] private Material black;
    [SerializeField] private List<GameObject> blocks = new List<GameObject>();
    [HideInInspector] public GameObject[][,] pieces;
    [HideInInspector] public GameObject[,] rowFiller;
    [HideInInspector] public GameObject[,] colFiller;
    [HideInInspector] public GameObject[,] pieceToInput = { { } };

    private GameManager gameManager;
    private Vector3 originalPos = new Vector3();

    private bool selected;
    private bool entered;
    private int location;

    private void Awake()
    {
        originalPos = transform.position;
        gameManager = GameObject.Find("Nich").GetComponent<GameManager>();
        MakePieces();
    }
    private void Update()
    {
        DragAndDropRunner();
    }
    //Runs drag and drop
    private void DragAndDropRunner()
    {
        if (selected)
        {
            Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(cursorPos.x, cursorPos.y, transform.position.z);
        }
        if (Input.GetMouseButtonUp(0))
        {

            selected = false;
            if (entered && gameManager.CanAddPiece(pieceToInput, location / 8, location % 8))
            {
                gameManager.AddPieceRunner(pieceToInput, location);
                gameManager.instancePieces.Remove(gameObject);
                Destroy(gameObject);
            }
            else
            {
                transform.position = originalPos;
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }
    //Makes pieces
    private void MakePieces()
    {
        rowFiller = new GameObject[8, 1] { { blackBlock }, { blackBlock }, { blackBlock }, { blackBlock },
                                        { blackBlock }, { blackBlock }, { blackBlock }, { blackBlock } };

        colFiller = new GameObject[1, 8] { {blackBlock, blackBlock, blackBlock, blackBlock,
                                        blackBlock, blackBlock, blackBlock, blackBlock} };

        pieces = new GameObject[][,]
        {

        new GameObject[,] { { blocks[0] } },

        new GameObject[,] { { blocks[1], blocks[1] } },
        new GameObject[,] { { blocks[1] }, { blocks[1] } },

        new GameObject[,] { { blocks[2], blocks[2] }, { blocks[2], blocks[2] } },

        new GameObject[,] { { blocks[3], blocks[3] }, { blocks[3], blackBlock } },
        new GameObject[,] { { blocks[3], blocks[3] }, { blackBlock, blocks[3] } },

        new GameObject[,] { { blackBlock, blocks[4], blackBlock }, { blocks[4], blocks[4], blocks[4] } },
        new GameObject[,] { { blocks[4], blocks[4], blocks[4] }, { blackBlock, blocks[4], blackBlock } },
        new GameObject[,] { {  blocks[4], blackBlock }, {blocks[4], blocks[4] }, { blocks[4] , blackBlock } },
        new GameObject[,] { {  blackBlock, blocks[4] }, { blocks[4], blocks[4] }, { blackBlock, blocks[4] } },

        new GameObject[,] { { blackBlock, blocks[5], blocks[5] }, { blocks[5], blocks[5], blackBlock } },
        new GameObject[,] { { blocks[5], blocks[5], blackBlock }, { blackBlock, blocks[5], blocks[5] } },
        new GameObject[,] { { blocks[5], blackBlock }, {blocks[5], blocks[5] }, { blackBlock, blocks[5]} },
        new GameObject[,] { { blackBlock, blocks[5] }, {  blocks[5], blocks[5] }, {  blocks[5], blackBlock } },

        new GameObject[,] { { blocks[6], blocks[6], blocks[6] } },
        new GameObject[,] { { blocks[6] }, { blocks[6] }, { blocks[6] } },

        new GameObject[,] { { blocks[7], blocks[7], blocks[7], blocks[7] } },
        new GameObject[,] { { blocks[7] }, { blocks[7] }, { blocks[7] }, { blocks[7] } },

        new GameObject[,] { { blocks[8], blocks[8], blocks[8], blocks[8], blocks[8] } },
        new GameObject[,] { { blocks[8] }, { blocks[8] }, { blocks[8] }, { blocks[8] }, { blocks[8] } },

        new GameObject[,] { { blocks[9], blocks[9] }, { blocks[9], blackBlock }, { blocks[9], blackBlock } },
        new GameObject[,] { { blocks[9], blocks[9] }, { blackBlock, blocks[9] }, { blackBlock, blocks[9] } },
        new GameObject[,] { { blocks[9], blackBlock }, { blocks[9], blackBlock }, { blocks[9], blocks[9] } },
        new GameObject[,] { { blackBlock, blocks[9] }, { blackBlock, blocks[9] }, { blocks[9], blocks[9] } },
        new GameObject[,] { { blocks[9], blackBlock, blackBlock }, { blocks[9], blocks[9], blocks[9] } },
        new GameObject[,] { { blackBlock, blackBlock, blocks[9] }, { blocks[9], blocks[9], blocks[9] } },
        new GameObject[,] { { blocks[9], blocks[9], blocks[9] }, { blocks[9], blackBlock, blackBlock } },
        new GameObject[,] { { blocks[9], blocks[9], blocks[9] }, { blackBlock, blackBlock, blocks[9] } },

        new GameObject[,] { {  blocks[10], blocks[10], blocks[10] }, { blocks[10], blackBlock, blackBlock }, { blocks[10],  blackBlock, blackBlock } },
        new GameObject[,] { {  blocks[10], blocks[10], blocks[10] }, { blackBlock, blackBlock, blocks[10] }, {  blackBlock, blackBlock, blocks[10] } },
        new GameObject[,] { { blocks[10], blackBlock, blackBlock }, { blocks[10],  blackBlock, blackBlock }, {  blocks[10], blocks[10], blocks[10] } },
        new GameObject[,] { { blackBlock, blackBlock, blocks[10] }, {  blackBlock, blackBlock, blocks[10] }, { blocks[10], blocks[10], blocks[10] } },

        new GameObject[,] { { blocks[11], blocks[11], blocks[11] }, { blocks[11], blocks[11], blocks[11] }, { blocks[11], blocks[11], blocks[11] } },

        new GameObject[,] { { blocks[0] } },

        new GameObject[,] { { blocks[1], blocks[1] } },
        new GameObject[,] { { blocks[1] }, { blocks[1] } }
        };


    }
    //Allows for drag and drop
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && gameManager.isGameActive && !transform.GetChild(2).gameObject.activeSelf)
        {
            selected = true;
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        location = other.GetComponent<Block>().num;
        gameManager.AddMaterial(pieceToInput, location, material, false);

        entered = true;
    }
    private void OnTriggerExit(Collider other)
    {
        entered = false;
    }
}
