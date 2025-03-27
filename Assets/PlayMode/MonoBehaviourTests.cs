using FluentAssertions;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;
using System.Collections;
using System.IO;
namespace Tests
{
    public class MonoBehaviourTests
    {
        private GameManager gameManager;
        private GameField gameField;
        private GameObject managerGO;
        private GameObject fieldGO;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            
            string path = Application.persistentDataPath + "/savegame.dat";
            if(File.Exists(path))
                File.Delete(path);
            CellNumberLoader loader = new GameObject("CellNumberLoader").AddComponent<CellNumberLoader>();
            var cellNumber2 = ScriptableObject.CreateInstance<CellNumber>();
            cellNumber2.number = 2;
            cellNumber2.color = Color.white;
            cellNumber2.textColor = Color.black;
            CellNumber.cellNumbers.Add(cellNumber2);

            var cellNumber4 = ScriptableObject.CreateInstance<CellNumber>();
            cellNumber4.number = 4;
            cellNumber4.color = Color.gray;
            cellNumber4.textColor = Color.white;
            CellNumber.cellNumbers.Add(cellNumber4);
            loader.Awake();
            
            
            fieldGO = new GameObject("GameField");
            gameField = fieldGO.AddComponent<GameField>();
            gameField.rows = 4;
            gameField.columns = 4;
            gameField.cellGrid = new Cell[4, 4];
            var dummyPrefab = new GameObject("DummyCellPrefab");
            dummyPrefab.AddComponent<CellView>();
            gameField.cellsPrefab = dummyPrefab;

            managerGO = new GameObject("GameManager");
            managerGO.AddComponent<GameManager>().gameField = gameField;
            gameManager = managerGO.GetComponent<GameManager>();

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator Teardown()
        {
            Object.DestroyImmediate(managerGO);
            Object.DestroyImmediate(fieldGO);
            yield return null;
        }

        [UnityTest]
        public IEnumerator RestartProcess_ShouldShowGameOverScreen_OnGameOverEvent()
        {
            gameManager.score = 100;
            GameManager.Instance = gameManager;

            var inputGO = new GameObject("InputHandler");
            var inputHandler = inputGO.AddComponent<InputHandler>();

            var rpGO = new GameObject("RestartProcess");
            var restartProcess = rpGO.AddComponent<RestartProcess>();
            restartProcess.scoreText = new GameObject("ScoreText").AddComponent<TextMeshProUGUI>();
            restartProcess.inputHandler = inputHandler;
            yield return null;

            gameManager.OnGameOver?.Invoke();
            yield return null;

            restartProcess.gameObject.activeSelf.Should().BeTrue();
            restartProcess.scoreText.text.Should().Be($"Score: {gameManager.score}");

            Object.Destroy(inputGO);
            Object.Destroy(rpGO);
        }

        [UnityTest]
        public IEnumerator CellView_ShouldUpdateValueAndPosition()
        {
            var canvasGO = new GameObject("Canvas");
            canvasGO.AddComponent<Canvas>();

            var cellGO = new GameObject("CellView");
            var cellView = cellGO.AddComponent<CellView>();
            cellView.valueText = cellGO.AddComponent<TextMeshProUGUI>();
            cellView.cellImage = cellGO.AddComponent<UnityEngine.UI.Image>();

            var cell = new Cell(new Vector2Int(0, 0), 2);
            gameField.CreateCell(cell, new Vector2Int(0, 0));

            cellView.Init(cell);
            yield return null;

            cell.SetValue(4);
            yield return null;

            cellView.valueText.text.Should().Be("4");
            cellView.valueText.color.Should().Be(cell.CellDescription.textColor);

            cell.SetPosition(new Vector2Int(1, 1), false);
            yield return new WaitForSeconds(0.2f);

            cellView.transform.localPosition.Should().Be(
                new Vector3(1 * 166.2f, 1 * 166.2f, 0)
            );

            Object.Destroy(cellGO);
            Object.Destroy(canvasGO);
        }

        [UnityTest]
        public IEnumerator InputHandler_ShouldProcessSwipeRight()
        {
            var inputHandler = new GameObject("InputHandler").AddComponent<InputHandler>();
            GameManager.Instance = gameManager;
            inputHandler.ProcessSwipe(new Vector2Int(100, 0));
            LogAssert.Expect(LogType.Log, "Движение вправо");
            yield return null;

            Object.Destroy(inputHandler.gameObject);
        }
        [UnityTest]
        public IEnumerator InputHandler_ShouldProcessSwipeLeft()
        {
            var inputHandler = new GameObject("InputHandler").AddComponent<InputHandler>();
            GameManager.Instance = gameManager;
            inputHandler.ProcessSwipe(new Vector2Int(-100, 0));
            LogAssert.Expect(LogType.Log, "Движение влево");
            yield return null;
            Object.Destroy(inputHandler.gameObject);
        }
        [UnityTest]
        public IEnumerator InputHandler_ShouldProcessSwipeUp()
        {
            var inputHandler = new GameObject("InputHandler").AddComponent<InputHandler>();
            GameManager.Instance = gameManager;
            inputHandler.ProcessSwipe(new Vector2Int(0, 100));
            LogAssert.Expect(LogType.Log, "Движение вверх");
            yield return null;
            Object.Destroy(inputHandler.gameObject);
        }
        [UnityTest]
        public IEnumerator InputHandler_ShouldProcessSwipeDown()
        {
            var inputHandler = new GameObject("InputHandler").AddComponent<InputHandler>();
            GameManager.Instance = gameManager;
            inputHandler.ProcessSwipe(new Vector2Int(0, -100));
            LogAssert.Expect(LogType.Log, "Движение вниз");
            yield return null;

            Object.Destroy(inputHandler.gameObject);
        }
    }
}
