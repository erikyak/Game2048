using NUnit.Framework;
using FluentAssertions;
using UnityEngine;

namespace Tests
{
    public class CellDataTests
    {
        [Test]
        public void Constructor_ShouldSetCorrectValues()
        {
            Vector2 pos = new Vector2(1, 2);
            int value = 42;
            var cellData = new CellData(pos, value);
            cellData.positionX.Should().Be(1);
            cellData.positionY.Should().Be(2);
            cellData.value.Should().Be(42);
        }
    }
}