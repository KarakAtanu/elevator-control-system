using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Service.Services;

namespace ElevatorControlSystem.Tests.Service
{
	public class FloorRequestQueueManagerTests
	{
		[Fact]
		public void AddRequest_UpDirection_AddsToUpQueue()
		{
			// Arrange
			var manager = new FloorRequestQueueManager();

			// Act
			manager.AddRequest(3, Direction.Up);

			// Assert
			Assert.True(manager.HasUpRequests());
			Assert.Equal(3, manager.GetNextUp());
		}

		[Fact]
		public void AddRequest_DownDirection_AddsToDownQueue()
		{
			// Arrange
			var manager = new FloorRequestQueueManager();

			// Act
			manager.AddRequest(5, Direction.Down);

			// Assert
			Assert.True(manager.HasDownRequests());
			Assert.Equal(5, manager.GetNextDown());
		}

		[Fact]
		public void GetNextUp_ReturnsLowestFloor()
		{
			// Arrange
			var manager = new FloorRequestQueueManager();
			manager.AddRequest(7, Direction.Up);
			manager.AddRequest(2, Direction.Up);
			manager.AddRequest(5, Direction.Up);

			// Act
			var nextUp = manager.GetNextUp();

			// Assert
			Assert.Equal(2, nextUp);
		}

		[Fact]
		public void GetNextDown_ReturnsHighestFloor()
		{
			// Arrange
			var manager = new FloorRequestQueueManager();
			manager.AddRequest(1, Direction.Down);
			manager.AddRequest(4, Direction.Down);
			manager.AddRequest(3, Direction.Down);

			// Act
			var nextDown = manager.GetNextDown();

			// Assert
			Assert.Equal(4, nextDown);
		}

		[Fact]
		public void HasUpRequests_ReturnsFalseWhenEmpty()
		{
			// Arrange
			var manager = new FloorRequestQueueManager();

			// Act
			var hasUp = manager.HasUpRequests();

			// Assert
			Assert.False(hasUp);
		}

		[Fact]
		public void HasDownRequests_ReturnsFalseWhenEmpty()
		{
			// Arrange
			var manager = new FloorRequestQueueManager();

			// Act
			var hasDown = manager.HasDownRequests();

			// Assert
			Assert.False(hasDown);
		}

		[Fact]
		public void RemoveUp_RemovesFloorFromUpQueue()
		{
			// Arrange
			var manager = new FloorRequestQueueManager();
			manager.AddRequest(6, Direction.Up);

			// Act
			manager.RemoveUp(6);

			// Assert
			Assert.False(manager.HasUpRequests());
			Assert.Null(manager.GetNextUp());
		}

		[Fact]
		public void RemoveDown_RemovesFloorFromDownQueue()
		{
			// Arrange
			var manager = new FloorRequestQueueManager();
			manager.AddRequest(8, Direction.Down);

			// Act
			manager.RemoveDown(8);

			// Assert
			Assert.False(manager.HasDownRequests());
			Assert.Null(manager.GetNextDown());
		}
	}
}
