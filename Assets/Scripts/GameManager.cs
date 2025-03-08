using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameField gameField;

    public Action<int> OnScoreChanged;
    public Action OnGameOver;
    
    public int score;
    private int highScore;
    private int oneMoveScore;
    
    private bool gameOver;
    
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI MaxScoreText;
    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SaveBestScore();
        LoadGame();
    }

    public void ProcessMove(Vector2Int direction)
    {
        Debug.Log("Обработка хода: " + direction);
        bool moved = MoveCells(direction);
        if (moved)
        {
            
            if (oneMoveScore!=0) UpdateScore(oneMoveScore);
            oneMoveScore = 0;
            gameField.CreateInRandomPosition();
            if (IsGameOver())
            {
                Debug.LogWarning("Игра окончена!");
                gameOver = true;
                OnGameOver?.Invoke();
            }
        }
    }

    private bool MoveCells(Vector2Int direction)
    {
        bool moved = false;
        if (direction.x < 0 || direction.y < 0)
        {
            for (int x = 0; x < gameField.rows; x++)
            {
                for (int y = 0; y < gameField.columns; y++)
                {
                    if (gameField.cellGrid[x, y] == null) continue;
                    if (MoveCell(direction, gameField.cellGrid[x, y]))
                        moved = true;
                }
            }
        }
        else
        {
            for (int x = gameField.rows - 1; x >= 0; x--)
            {
                for (int y = gameField.columns - 1; y >= 0; y--)
                {
                    if (gameField.cellGrid[x, y] == null) continue;
                    if (MoveCell(direction, gameField.cellGrid[x, y]))
                        moved = true;
                }
            }
        }

        return moved;
    }

    private bool MoveCell(Vector2Int direction, Cell cell)
    {
        bool moved = false;
        while (true)
        {
            Vector2Int newPos = cell.Position + direction;
            if (newPos.x >= 0 && newPos.x < gameField.columns && newPos.y >= 0 && newPos.y < gameField.rows)
            {
                Cell other = gameField.cellGrid[newPos.x, newPos.y];
                if (other == null)
                {
                    cell.SetPosition(newPos, false);
                    moved = true;
                    continue;
                }
                if (other.CellDescription.number == cell.CellDescription.number)
                {
                    other.SetValue(other.CellDescription.number * 2);
                    gameField.RemoveCell(cell);

                    oneMoveScore += other.CellDescription.number;

                    moved = true;
                }
            }

            break;
        }

        return moved;
    }


    private void UpdateScore(int addScore)
    {
        score += addScore;
        if(highScore < score) highScore = score;
        currentScoreText.text = $"Score: {score.ToString()}";
        MaxScoreText.text = $"Best: {highScore.ToString()}";
        Debug.Log("Счёт: " + score);
        SaveBestScore();
        OnScoreChanged?.Invoke(addScore);
    }

    private bool IsGameOver()
    {
        if (gameField.GetEmptyPosition().x >= 0)
            return false;
        for (int x = 0; x < gameField.rows; x++)
        {
            for (int y = 0; y < gameField.columns; y++)
            {
                Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
                foreach (var dir in dirs)
                {
                    Vector2Int neighborPos = new Vector2Int(x, y) + dir;
                    if (neighborPos.x < 0 || neighborPos.x >= gameField.columns ||
                        neighborPos.y < 0 || neighborPos.y >= gameField.rows) continue;
                    Cell neighbor = gameField.cellGrid[neighborPos.x, neighborPos.y];
                    Cell thisCell = gameField.cellGrid[x, y];
                    if (neighbor != null && thisCell != null && neighbor.CellDescription.number ==
                        thisCell.CellDescription.number)
                        return false;
                }
            }
        }

        return true;
    }

    public void ClearGameField()
    {
        for (int x = 0; x < gameField.rows; x++)
        {
            for (int y = 0; y < gameField.columns; y++)
            {
                if(gameField.cellGrid[x, y] != null)
                    gameField.RemoveCell(gameField.cellGrid[x, y]);
            }
        }

        score = 0;
    }
    public void ResetGame()
    {
        gameOver = false;
        ClearGameField();
        UpdateScore(0);
        gameField.CreateInRandomPosition();
        gameField.CreateInRandomPosition();
    }

    private void SaveBestScore()
    {
        string path = Application.persistentDataPath + "/bestScore.dat";
        highScore = 0;
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            highScore = (int)bf.Deserialize(file);
            file.Close();
        }
        if (score > highScore)
        {
            highScore = score;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);
            bf.Serialize(file, score);
            file.Close();
        }
    }

    public void SaveGame()
    {
        string path = Application.persistentDataPath + "/savegame.dat";
        if (gameOver)
        {
            File.Delete(path);
            return;
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);
        List<CellData> data = new List<CellData>();
        for (int x = 0; x < gameField.rows; x++)
        {
            for (int y = 0; y < gameField.columns; y++)
            {
                if (gameField.cellGrid[x, y] != null)
                {
                    Cell cell = gameField.cellGrid[x, y];
                    data.Add(new CellData(cell.Position, cell.CellDescription.number));
                }
            }
        }
        bf.Serialize(file, score);
        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/savegame.dat";
        Debug.Log(path);
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            int savedScore = (int)bf.Deserialize(file);
            List<CellData> data = (List<CellData>)bf.Deserialize(file);
            file.Close();

            ClearGameField();
            foreach (var cellData in data)
            {
                Vector2Int position = new Vector2Int(cellData.positionX, cellData.positionY);
                Cell cell = new Cell(position, cellData.value);
                gameField.CreateCell(cell, position);
            }
            UpdateScore(savedScore);
            
            if(data.Count == 0) ResetGame();
        }
        else
        {
            ResetGame();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}

