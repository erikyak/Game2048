using NUnit.Framework;
using FluentAssertions;
using UnityEngine;
using System;


namespace Tests
{
    public class CellTests
    {
        [SetUp]
        public void Setup()
        {
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
        }

        [Test]
        public void SetValue_ShouldChangeValueAndFireEvent_WhenNewValueIsDifferent()
        {
            var cell = new Cell(new Vector2Int(0, 0), 2);
            int eventValue = 0;
            cell.OnValueChanged += (newValue) => eventValue = newValue;
            cell.SetValue(4);

            cell.CellDescription.number.Should().Be(4);
            eventValue.Should().Be(4);
        }

        [Test]
        public void SetValue_ShouldNotFireEvent_WhenNewValueIsSame()
        {
            var cell = new Cell(new Vector2Int(0, 0), 2);
            bool eventFired = false;
            cell.OnValueChanged += (_) => eventFired = true;
            cell.SetValue(2);

            cell.CellDescription.number.Should().Be(2);
            eventFired.Should().BeFalse();
        }

        [Test]
        public void SetPosition_ShouldFireEventWithCorrectParameters_WhenNotTeleporting()
        {
            var cell = new Cell(new Vector2Int(1, 1), 2);
            Vector2Int oldPos = Vector2Int.zero;
            Vector2Int newPos = Vector2Int.zero;
            cell.OnPositionChanged += (from, to) =>
            {
                oldPos = from;
                newPos = to;
            };

            cell.SetPosition(new Vector2Int(2, 2), false);

            oldPos.Should().Be(new Vector2Int(1, 1));
            newPos.Should().Be(new Vector2Int(2, 2));
            cell.Position.Should().Be(new Vector2Int(2, 2));
        }

        [Test]
        public void SetPosition_ShouldFireEventWithTeleportParameter_WhenTeleporting()
        {
            var cell = new Cell(new Vector2Int(1, 1), 2);
            Vector2Int fromPos = Vector2Int.zero;
            Vector2Int toPos = Vector2Int.zero;
            cell.OnPositionChanged += (from, to) =>
            {
                fromPos = from;
                toPos = to;
            };

            cell.SetPosition(new Vector2Int(3, 3), true);

            fromPos.Should().Be(Vector2Int.left); 
            toPos.Should().Be(new Vector2Int(3, 3));
            cell.Position.Should().Be(new Vector2Int(3, 3));
        }

    }
}
