using NUnit.Framework;
using FluentAssertions;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tests
{
    public class GameManagerTests
    {
        private GameManager gameManager;
        private GameField gameField;
        private GameObject managerGO;
        
        private string bestScorePath;

        [SetUp]
        public void Setup()
        {
            
            bestScorePath = Application.persistentDataPath + "/bestScore.dat";
            if (File.Exists(bestScorePath))
                File.Delete(bestScorePath);
            CellNumber.cellNumbers.Clear();
            var two = ScriptableObject.CreateInstance<CellNumber>();
            two.number = 2;
            two.color = Color.white;
            two.textColor = Color.black;
            CellNumber.cellNumbers.Add(two);
            var four = ScriptableObject.CreateInstance<CellNumber>();
            four.number = 4;
            four.color = Color.gray;
            four.textColor = Color.white;
            CellNumber.cellNumbers.Add(four);
            var eight = ScriptableObject.CreateInstance<CellNumber>();
            eight.number = 8;
            eight.color = Color.gray;
            eight.textColor = Color.white;
            CellNumber.cellNumbers.Add(eight);

            managerGO = new GameObject("GameManager");
            gameManager = managerGO.AddComponent<GameManager>();

            var fieldGO = new GameObject("GameField");
            gameField = fieldGO.AddComponent<GameField>();
            gameManager.gameField = gameField;
            gameField.cellGrid = new Cell[gameField.rows, gameField.columns];
            
            var dummyPrefab = new GameObject("DummyCellPrefab");
            dummyPrefab.AddComponent<CellView>();
            gameField.cellsPrefab = dummyPrefab;
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(managerGO);
            Object.DestroyImmediate(gameField.gameObject);
        }

        [UnityTest]
        public IEnumerator ProcessMove_ShouldMergeCellsAndUpdateScore()
        {
            var cell1 = new Cell(new Vector2Int(0, 0), 2);
            var cell2 = new Cell(new Vector2Int(1, 0), 2);
            gameField.cellGrid[0, 0] = cell1;
            gameField.cellGrid[1, 0] = cell2;
            var dummyGO = new GameObject("DummyCellView");
            var cellView = dummyGO.AddComponent<CellView>();
            cell1.CellView = cellView;
            cell2.CellView = cellView;
            cell1.OnPositionChanged += gameField.ChangeCellPosition;
            cell2.OnPositionChanged += gameField.ChangeCellPosition;

            gameManager.score = 0;
            gameManager.ProcessMove(Vector2Int.right);
            yield return null;
            gameManager.score.Should().Be(4);
            Object.DestroyImmediate(dummyGO);
        }

        [UnityTest]
        public IEnumerator ProcessMove_ShouldInvokeGameOver_WhenNoMovesAvailable()
        {

            for (int x = 0; x < gameField.rows; x++)
            {
                for (int y = 0; y < gameField.columns; y++)
                {
                    int value = ((x + y) % 2 == 0) ? 2 : 4;
                    var cell = new Cell(new Vector2Int(x, y), value);
                    gameField.cellGrid[x, y] = cell;
                    var dummyGO = new GameObject("DummyCellView");
                    cell.CellView = dummyGO.AddComponent<CellView>();
                    cell.OnPositionChanged += gameField.ChangeCellPosition;
                }
            }

            bool gameOverInvoked = false;
            gameManager.OnGameOver += () => gameOverInvoked = true;

            gameManager.ProcessMove(Vector2Int.up);
            yield return null;

            gameOverInvoked.Should().BeTrue();
        }

        [UnityTest]
        public IEnumerator ResetGame_ShouldClearFieldAndCreateTwoCells()
        {

            var cell = new Cell(new Vector2Int(0, 0), 2);
            gameField.CreateCell(cell, new Vector2Int(0,0));
            gameManager.score = 10;

            gameManager.ResetGame();
            yield return null;

            int cellCount = 0;
            for (int x = 0; x < gameField.rows; x++)
            {
                for (int y = 0; y < gameField.columns; y++)
                {
                    if (gameField.cellGrid[x, y] != null)
                        cellCount++;
                }
            }
            cellCount.Should().Be(2);
            gameManager.score.Should().Be(0);
        }
        [Test]
        public void UpdateScore_ShouldUpdateScoreAndBestScore()
        {
            if (File.Exists(bestScorePath))
                File.Delete(bestScorePath);
            
            int initialScore = gameManager.score;
            gameManager.UpdateScore(10);
            gameManager.score.Should().Be(initialScore + 10);
            BinaryFormatter bf = new BinaryFormatter();
            using FileStream file = File.OpenRead(bestScorePath);
            int bestScore = (int)bf.Deserialize(file);
            bestScore.Should().Be(gameManager.score);
        }

        [Test]
        public void ClearGameField_ShouldRemoveAllCells()
        {
            for (int x = 0; x < gameField.rows; x++)
            {
                for (int y = 0; y < gameField.columns; y++)
                {
                    var cell = new Cell(new Vector2Int(x, y), 2);
                    gameField.CreateCell(cell,new Vector2Int(x,y));
                }
            }
            gameManager.ClearGameField();
            for (int x = 0; x < gameField.rows; x++)
            {
                for (int y = 0; y < gameField.columns; y++)
                {
                    gameField.cellGrid[x, y].Should().BeNull();
                }
            }
        }

        [UnityTest]
        public IEnumerator SaveGameAndLoadGame_ShouldPersistAndRestoreGameState()
        {
            var cell = new Cell(new Vector2Int(0, 0), 2);
            gameField.CreateCell(cell, new Vector2Int(0,0));
            gameManager.score = 50;
            gameManager.SaveGame();
            gameManager.ClearGameField();
            gameManager.LoadGame();
            yield return null;
            gameManager.score.Should().Be(50);
            gameField.cellGrid[0, 0].Should().NotBeNull();
        }

        [Test]
        public void IsGameOver_ShouldReturnTrue_WhenNoMovesAvailable()
        {
            gameManager.ClearGameField();
            for (int x = 0; x < gameField.rows; x++)
            {
                for (int y = 0; y < gameField.columns; y++)
                {
                    int value = ((x + y) % 2 == 0) ? 2 : 4;
                    var cell = new Cell(new Vector2Int(x, y), value);
                    gameField.CreateCell(cell, cell.Position);
                }
            }
            bool result = gameManager.IsGameOver();
            result.Should().BeTrue();
        }
        
        [Test]
        public void SaveBestScore_ShouldHandleFileCorruptionGracefully()
        {
            File.WriteAllText(bestScorePath, "corrupted data");
            gameManager.UpdateScore(10);
            gameManager.highScore.Should().Be(10);
        }

        [UnityTest]
        public IEnumerator ProcessMove_ShouldHandleMultipleMergesInSingleMove()
        {
            var cells = new[] {
                new Cell(new Vector2Int(0, 0), 2),
                new Cell(new Vector2Int(1, 0), 2),
                new Cell(new Vector2Int(2, 0), 4),
                new Cell(new Vector2Int(3, 0), 4)
            };
    
            foreach (var cell in cells)
                gameField.CreateCell(cell, cell.Position);
            gameManager.ProcessMove(Vector2Int.right);
            yield return null;

            gameManager.score.Should().Be(4 + 8);
        }
    }
}
