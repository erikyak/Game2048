using NUnit.Framework;
using FluentAssertions;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;
using System.Collections;

namespace Tests
{
    public class PlusScoreTests
    {
        private PlusScore plusScore;
        private GameObject plusScoreGO;
        private GameObject plusScoreUIPrefab;
        private GameManager gameManager;

        [SetUp]
        public void Setup()
        {
            var gameManagerGO = new GameObject("GameManager");
            gameManager = gameManagerGO.AddComponent<GameManager>();
            gameManager.Awake();
            plusScoreGO = new GameObject("PlusScore");
            plusScore = plusScoreGO.AddComponent<PlusScore>();
            plusScore.Start();
            plusScoreGO.SetActive(true);
            plusScoreUIPrefab = new GameObject("PlusScoreUI");
            plusScoreUIPrefab.AddComponent<TextMeshProUGUI>();
            plusScore.plusScoreUI = plusScoreUIPrefab;
            var canvasGO = new GameObject("Canvas");
            canvasGO.AddComponent<Canvas>();
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(plusScoreGO);
            Object.DestroyImmediate(plusScoreUIPrefab);
            Object.DestroyImmediate(gameManager.gameObject);
        }

        [UnityTest]
        public IEnumerator ChangeScore_ShouldInstantiatePlusScoreUIAndAnimateOut()
        {
            gameManager.OnScoreChanged?.Invoke(5);
            var texts = plusScoreGO.GetComponentsInChildren<TextMeshProUGUI>(true);
            texts.Should().HaveCount(1);
            texts[0].text.Should().Be("+5");
            yield return null;
        }
    }
}