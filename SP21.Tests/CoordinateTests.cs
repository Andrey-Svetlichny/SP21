using NUnit.Framework;

namespace SP21.Tests
{
    [TestFixture]
    public class CoordinateTests
    {


        [Test]
        public void Coordinate_AssigAndGetValueX_Correct()
        {
            var coord = new Coordinate.Point(3, 5);
            Assert.AreEqual(3, coord.X);
        }

        [Test]
        public void Coordinate_AssigAndGetValueY_Correct()
        {
            var coord = new Coordinate.Point(3, 5);
            Assert.AreEqual(5, coord.Y);
        }

        [TestCase(3, 3, Coordinate.Direction.Left,  2, 3, TestName = "NormalCoordinate MoveLeft")]
        [TestCase(3, 3, Coordinate.Direction.Right, 4, 3, TestName = "NormalCoordinate MoveRight")]
        [TestCase(3, 3, Coordinate.Direction.Up,    3, 2, TestName = "NormalCoordinate MoveUp")]
        [TestCase(3, 3, Coordinate.Direction.Down,  3, 4, TestName = "NormalCoordinate MoveDown")]

        [TestCase( 0, 3, Coordinate.Direction.Left,  79, 2, TestName = "BoundCoordinate MoveLeft")]
        [TestCase(79, 3, Coordinate.Direction.Right,  0, 4, TestName = "BoundCoordinate MoveRight")]
        public void Coordinate_Move_Correct(int srcX, int srcY, Coordinate.Direction dir, int expectedX, int expectedY)
        {
            var coord = new Coordinate.Point(srcX, srcY);
            coord.Move(dir);
            var expected = new Coordinate.Point(expectedX, expectedY);
            Assert.AreEqual(expected, coord);
        }

        [TestCase(0, 0,               Coordinate.Direction.Up,   TestName = "BoundCoordinate MoveUp")]
        [TestCase(0, Coordinate.MaxY, Coordinate.Direction.Down, TestName = "BoundCoordinate MoveDown")]
        public void Coordinate_Move_Exception(int srcX, int srcY, Coordinate.Direction dir)
        {
            var coord = new Coordinate.Point(srcX, srcY);
            Assert.That(() => coord.Move(dir), Throws.ArgumentException);
        }
    }
}
