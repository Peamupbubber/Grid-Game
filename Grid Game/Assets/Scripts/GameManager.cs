using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField] private GameObject blackBlock;
	[SerializeField] private GameObject startScreen;
	[SerializeField] private GameObject gameOverScreen;
	[SerializeField] private Material black;
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private TextMeshProUGUI highScoreText;
	[SerializeField] private List<GameObject> pieces = new List<GameObject>();

	[HideInInspector] public List<GameObject> instancePieces;
	[HideInInspector] public bool isGameActive;

	private List<int> numsToReplaceRow = new List<int>();
	private List<int> numsToReplaceCol = new List<int>();

	private Vector3[] locations = { new Vector3(-3, 8, -0.5f), new Vector3(-6, 5.5f, -0.5f), new Vector3(-4.5f, 3, -0.5f) };
	private GameObject[,] grid;
	private GameObject[,] instanceGrid;

	private GameObject[,] rowFiller;
	private GameObject[,] colFiller;

	private bool goalMet = false;
	private bool hasBeenPlayed = false;
	private int score = 0;
	private int highScore = 0;

	public void StartGame()
	{
		
		if (hasBeenPlayed)
		{
			foreach (GameObject obj in instanceGrid)
			{
				Destroy(obj);
			}
			foreach (GameObject obj in instancePieces)
			{
				Destroy(obj);
			}
		}
		
		grid = new GameObject[8, 8];
		instanceGrid = new GameObject[8, 8];
		instancePieces = new List<GameObject>();
		startScreen.gameObject.SetActive(false);
		gameOverScreen.gameObject.SetActive(false);
		score = 0;
		UpdateScore(0);
		MakeFillers();
		MakeGrid();
		FillGrid();
		isGameActive = true;
		NewPieceList();
	}
	private void Update()
	{
		if (isGameActive)
		{
			TestForGameOver();
			ReplacePieceList();
		}
	}
	private void TestForGameOver()
	{
		int endCount = 0;
		bool canAdd = false;
		foreach (GameObject obj in instancePieces)
		{
			for (int i = 0; i <= grid.GetLength(0); i++)
			{
				for (int j = 0; j <= grid.GetLength(1); j++)
				{
					if (CanAddPiece(obj.GetComponent<DragAndDrop>().pieceToInput, i, j))
						canAdd = true;
				}
			}
			if (!canAdd)
			{
				endCount++;
				obj.transform.GetChild(2).gameObject.SetActive(true);

			}
			else
			{
				obj.transform.GetChild(2).gameObject.SetActive(false);
			}
			canAdd = false;
		}
		if (endCount == instancePieces.Count && endCount != 0)
		{
			GameOver();
		}
	}
	private void GameOver()
	{
		isGameActive = false;
		gameOverScreen.gameObject.SetActive(true);
		hasBeenPlayed = true;
	}
	private void UpdateScore(int points)
	{
		score += points;
		scoreText.text = "Score: " + score;
		if (highScore < score)
		{
			highScore = score;
			highScoreText.text = "High Score: " + highScore;
		}
	}
	//Replaces objs if there are none left
	private void ReplacePieceList()
	{
		if (instancePieces.Count == 0)
		{
			NewPieceList();
		}
	}
	//Create new piece options to choose from
	private void NewPieceList()
	{
		for (int i = 0; i < 3; i++)
		{
			int rand = Random.Range(0, pieces.Count);
			GameObject pieceToInput = pieces[rand];
			instancePieces.Add(Instantiate(pieceToInput, locations[i], pieceToInput.transform.rotation));
			instancePieces[i].GetComponent<DragAndDrop>().pieceToInput = instancePieces[i].GetComponent<DragAndDrop>().pieces[rand];
		}
	}

	private void MakeFillers()
	{
		rowFiller = new GameObject[8, 1] { { blackBlock }, { blackBlock }, { blackBlock }, { blackBlock },
										{ blackBlock }, { blackBlock }, { blackBlock }, { blackBlock } };

		colFiller = new GameObject[1, 8] { {blackBlock, blackBlock, blackBlock, blackBlock,
										blackBlock, blackBlock, blackBlock, blackBlock} };
	}
	//Makes grid
	private void MakeGrid()
	{
		for (int i = 0; i < grid.GetLength(0); i++)
		{
			for (int j = 0; j < grid.GetLength(1); j++)
			{
				grid[i, j] = blackBlock;
			}
		}
	}
	//Fill Grid
	public void FillGrid()
	{
		int num = 0;

		for (int i = 0; i < grid.GetLength(0); i++)
		{
			for (int j = 0; j < grid.GetLength(1); j++)
			{
				if (isGameActive)
				{
					Destroy(instanceGrid[i, j]);
				}
				grid[i, j].GetComponent<Block>().num = num;
				instanceGrid[i, j] = Instantiate(grid[i, j], new Vector3(grid[i, j].transform.position.x + j * 1.5f, grid[i, j].transform.position.y - i * 1.5f, grid[i, j].transform.position.z), grid[i, j].transform.rotation);
				num++;
			}
		}

	}
	// Places new block and refills
	public void AddPieceRunner(GameObject[,] input, int num)
	{
		AddPiece(input, num, false);

		TestForGoal();

		if (goalMet)
		{
			foreach (int i in numsToReplaceRow)
			{
				UpdateScore(10);
				AddPiece(rowFiller, i, true);
			}
			foreach (int i in numsToReplaceCol)
			{
				UpdateScore(10);
				AddPiece(colFiller, i, true);
			}
			numsToReplaceRow = new List<int>();
			numsToReplaceCol = new List<int>();
		}
		goalMet = false;
		FillGrid();
	}
	//Places by finding location and "shape"
	private void AddPiece(GameObject[,] input, int location, bool isFiller)
	{
		for (int i = 0; i < grid.GetLength(0); i++)
		{
			if (i == location / grid.GetLength(0))
			{
				for (int j = 0; j < grid.GetLength(1); j++)
				{
					if (j == location % grid.GetLength(1))
					{
						int temp = j;
						if (CanAddPiece(input, i, j))
						{
							for (int k = 0; k < input.GetLength(0); k++)
							{
								for (int l = 0; l < input.GetLength(1); l++)
								{
									//Allows row and col fillers to go through as well as replaces the black with green if it were to collide
									if (isFiller || grid[i, j].Equals(blackBlock) && !input[k, l].Equals(blackBlock))
									{
										if (!input[k, l].Equals(blackBlock))
											UpdateScore(1);
										grid[i, j] = input[k, l];
									}
									j++;
								}
								j = temp;
								i++;
							}
						}
						else
							return;
					}
				}
			}
		}
	}
	//Displays what would be the placement by changing the material
	public void AddMaterial(GameObject[,] input, int location, Material material, bool isFiller)
	{
		for (int i = 0; i < grid.GetLength(0); i++)
		{
			if (i == location / grid.GetLength(0))
			{
				for (int j = 0; j < grid.GetLength(1); j++)
				{
					if (j == location % grid.GetLength(1))
					{
						int temp = j;
						if (CanAddPiece(input, i, j))
						{
							for (int k = 0; k < input.GetLength(0); k++)
							{
								for (int l = 0; l < input.GetLength(1); l++)
								{
									if (isFiller || grid[i, j].Equals(blackBlock) && !input[k, l].Equals(blackBlock))
									{
										instanceGrid[i, j].GetComponent<MeshRenderer>().material = material;
									}
									j++;
								}
								j = temp;
								i++;
							}
						}
						else
							return;
					}
				}
			}
		}
	}
	//Test for the space required to add piece
	public bool CanAddPiece(GameObject[,] input, int row, int col)
	{
		if (input.GetLength(0) + row > grid.GetLength(0) || input.GetLength(1) + col > grid.GetLength(1))
			return false;
		int temp = col;
		for (int i = 0; i < input.GetLength(0); i++)
		{
			for (int j = 0; j < input.GetLength(1); j++)
			{
				if (!grid[row, col].Equals(blackBlock) && !input[i, j].Equals(blackBlock))
				{
					return false;
				}
				col++;
			}
			col = temp;
			row++;
		}
		return true;
	}
	//Test if there is a row to replace
	private void TestForGoal()
	{
		int countRow = 0;
		int countCol = 0;
		for (int i = 0; i < grid.GetLength(0); i++)
		{
			for (int j = 0; j < grid.GetLength(1); j++)
			{
				if (!grid[j, i].Equals(blackBlock))
					countRow++;
				if (!grid[i, j].Equals(blackBlock))
					countCol++;
			}
			if (countRow == 8)
			{
				numsToReplaceRow.Add(instanceGrid[0, i].GetComponent<Block>().num);
				goalMet = true;
			}
			if (countCol == 8)
			{
				numsToReplaceCol.Add(instanceGrid[i, 0].GetComponent<Block>().num);
				goalMet = true;
			}
			countRow = 0;
			countCol = 0;
		}
	}
}