using NUnit.Framework;
using FluentAssertions;
using UnityEngine;

namespace Tests
{
    public class GameFieldTests
    {
        private GameField gameField;

        [SetUp]
        public void Setup()
        {
            var go = new GameObject("GameField");
            gameField = go.AddComponent<GameField>();
            gameField.Awake();
            var dummyPrefab = new GameObject("DummyCellPrefab");
            dummyPrefab.AddComponent<CellView>();
            gameField.cellsPrefab = dummyPrefab;
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(gameField.gameObject);
        }

        [Test]
        public void GetEmptyPosition_ShouldReturnValidPosition_WhenGridIsEmpty()
        {
            Vector2Int pos = gameField.GetEmptyPosition();
            pos.x.Should().BeGreaterThanOrEqualTo(0);
            pos.y.Should().BeGreaterThanOrEqualTo(0);
        }

        [Test]
        public void CreateCell_ShouldAddCellToGrid()
        {
            CellNumber.cellNumbers.Clear();
            var two = ScriptableObject.CreateInstance<CellNumber>();
            two.number = 2;
            two.color = Color.white;
            two.textColor = Color.black;
            CellNumber.cellNumbers.Add(two);

            var cell = new Cell(new Vector2Int(0, 0), 2);
            gameField.CreateCell(cell, new Vector2Int(0, 0));
            gameField.cellGrid[0, 0].Should().NotBeNull();
            gameField.cellGrid[0, 0].Should().Be(cell);
        }

        [Test]
        public void RemoveCell_ShouldRemoveCellFromGrid()
        {
            CellNumber.cellNumbers.Clear();
            var two = ScriptableObject.CreateInstance<CellNumber>();
            two.number = 2;
            two.color = Color.white;
            two.textColor = Color.black;
            CellNumber.cellNumbers.Add(two);

            var cell = new Cell(new Vector2Int(0, 0), 2);
            gameField.CreateCell(cell, new Vector2Int(0, 0));
            gameField.RemoveCell(cell);
            gameField.cellGrid[0, 0].Should().BeNull();
        }
        [Test]
        public void ChangeCellPosition_ShouldUpdateGridCorrectly()
        {
            var cell = new Cell(new Vector2Int(0, 0), 2);
            gameField.cellGrid[0, 0] = cell;
            Vector2Int newPos = new Vector2Int(1, 1);
            gameField.ChangeCellPosition(new Vector2Int(0, 0), newPos);
            gameField.cellGrid[0, 0].Should().BeNull();
            gameField.cellGrid[1, 1].Should().Be(cell);
        }

        [Test]
        public void CreateInRandomPosition_ShouldPlaceCell_WhenEmptyPositionExists()
        {
            for (int x = 0; x < gameField.rows; x++)
            for (int y = 0; y < gameField.columns; y++)
                gameField.cellGrid[x, y] = null;
            gameField.CreateInRandomPosition();
            int count = 0;
            for (int x = 0; x < gameField.rows; x++)
            for (int y = 0; y < gameField.columns; y++)
                if (gameField.cellGrid[x, y] != null) count++;

            count.Should().Be(1);
        }
        
        [Test]
        public void Awake_ShouldInitializeCellGridWithCorrectDimensions()
        {
            gameField.rows = 5;
            gameField.columns = 5;
            gameField.Awake();
            gameField.cellGrid.GetLength(0).Should().Be(5);
            gameField.cellGrid.GetLength(1).Should().Be(5);
        }

        [Test]
        public void GetEmptyPosition_ShouldReturnNegative_WhenGridIsFull()
        {
            for (int x = 0; x < gameField.rows; x++)
            for (int y = 0; y < gameField.columns; y++)
                gameField.cellGrid[x, y] = new Cell(new Vector2Int(x, y), 2);
            Vector2Int pos = gameField.GetEmptyPosition();
            pos.x.Should().Be(-1);
            pos.y.Should().Be(-1);
        }
    }
}
