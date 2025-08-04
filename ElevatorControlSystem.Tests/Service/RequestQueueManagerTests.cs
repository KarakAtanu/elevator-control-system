using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Service.Request;
using ElevatorControlSystem.Service.Services;

namespace ElevatorControlSystem.Tests.Service
{
	public class RequestQueueManagerTests
	{
		[Fact]
		public void Enqueue_ShouldAddRequestToQueue()
		{
			// Arrange
			var manager = new RequestQueueManager();
			var request = new ElevatorRequest
			{
				Floor = 1,
				Direction = Direction.Up,
				DestinationFloor = 5
			};

			// Act
			manager.Enqueue(request);

			// Assert
			bool dequeued = manager.TryDequeue(out var dequeuedRequest);
			Assert.True(dequeued);
			Assert.NotNull(dequeuedRequest);
			Assert.Equal(request.Floor, dequeuedRequest.Floor);
			Assert.Equal(request.Direction, dequeuedRequest.Direction);
			Assert.Equal(request.DestinationFloor, dequeuedRequest.DestinationFloor);
		}

		[Fact]
		public void TryDequeue_ShouldReturnFalse_WhenQueueIsEmpty()
		{
			// Arrange
			var manager = new RequestQueueManager();

			// Act
			bool result = manager.TryDequeue(out var request);

			// Assert
			Assert.False(result);
			Assert.Null(request);
		}

		[Fact]
		public void TryDequeue_ShouldReturnTrue_AndRemoveRequest_WhenQueueIsNotEmpty()
		{
			// Arrange
			var manager = new RequestQueueManager();
			var request = new ElevatorRequest
			{
				Floor = 2,
				Direction = Direction.Down,
				DestinationFloor = 0
			};
			manager.Enqueue(request);

			// Act
			bool result = manager.TryDequeue(out var dequeuedRequest);

			// Assert
			Assert.True(result);
			Assert.NotNull(dequeuedRequest);
			Assert.Equal(request.Floor, dequeuedRequest.Floor);
			Assert.Equal(request.Direction, dequeuedRequest.Direction);
			Assert.Equal(request.DestinationFloor, dequeuedRequest.DestinationFloor);
		}
	}
}
