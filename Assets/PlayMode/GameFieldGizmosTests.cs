using NUnit.Framework;
using UnityEngine;
using FluentAssertions;
using System.Reflection;
using System.Collections.Generic;

public class GameFieldGizmosTests
{
    private GameField gameField;
    private List<(Vector3 pos, Vector3 size)> gizmoCalls = new List<(Vector3, Vector3)>();

    [SetUp]
    public void Setup()
    {
        GizmosWrapper.DrawCubeAction = (pos, size) => gizmoCalls.Add((pos, size));
        
        gameField = new GameObject().AddComponent<GameField>();
        gameField.rows = 2;
        gameField.columns = 2;
        gameField.Awake();
        
        var dummyPrefab = new GameObject("DummyCellPrefab");
        dummyPrefab.AddComponent<CellView>();
        gameField.cellsPrefab = dummyPrefab;
    }

    [Test]
    public void OnDrawGizmos_ShouldDrawCubesForCells()
    {
        gameField.CreateCell(new Cell(new Vector2Int(0, 0), 2), new Vector2Int(0, 0));
        gameField.CreateCell(new Cell(new Vector2Int(1, 1), 4), new Vector2Int(1, 1));

        InvokeOnDrawGizmos(gameField);

        gizmoCalls.Should().HaveCount(2);
        gizmoCalls[0].pos.Should().Be(new Vector3(0*70, 0*70, 0));
        gizmoCalls[1].pos.Should().Be(new Vector3(1*70, 1*70, 0));
    }
    [Test]
    public void OnDrawGizmos_ShouldNotDraw_WhenCellGridIsNull()
    {
        gizmoCalls.Clear();
        gameField.cellGrid = null;

        InvokeOnDrawGizmos(gameField);

        gizmoCalls.Should().BeEmpty();
    }

    private void InvokeOnDrawGizmos(GameField target)
    {
        var method = typeof(GameField)
            .GetMethod("OnDrawGizmos", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(target, null);
    }

    [TearDown]
    public void Teardown()
    {
        GizmosWrapper.DrawCubeAction = Gizmos.DrawCube;
        Object.DestroyImmediate(gameField.gameObject);
    }
}