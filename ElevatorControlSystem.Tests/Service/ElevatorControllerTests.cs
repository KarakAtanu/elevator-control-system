using ElevatorControlSystem.Service.Services;

namespace ElevatorControlSystem.Tests.Service
{
	public class ElevatorControllerTests
	{
		[Fact]
		public void Constructor_InitializesElevatorProperties()
		{
			var controller = new ElevatorController(1, 0, 10);

			Assert.Equal(0, controller.CurrentFloor); // Elevator starts at minFloor (constructor sets CurrentFloor to minFloor)
			Assert.True(controller.IsIdle);
			Assert.Empty(controller.Destinations);
		}

		[Fact]
		public void IsIdle_ReturnsTrueWhenDirectionIsIdle()
		{
			var controller = new ElevatorController(1, 0, 10);
			Assert.True(controller.IsIdle);
		}

		[Fact]
		public void CurrentFloor_ReturnsElevatorCurrentFloor()
		{
			var controller = new ElevatorController(1, 0, 10);
			Assert.Equal(0, controller.CurrentFloor);
		}

		[Fact]
		public async Task AddFloorRequestAsync_ThrowsArgumentOutOfRangeException_ForInvalidFloor()
		{
			var controller = new ElevatorController(1, 0, 10);
			var token = CancellationToken.None;

			await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
				controller.AddFloorRequestAsync(11, token));
		}

		[Fact]
		public async Task AddFloorRequestAsync_DoesNotAddDuplicateFloor()
		{
			var controller = new ElevatorController(1, 0, 10);
			var token = CancellationToken.None;

			var task1 =  controller.AddFloorRequestAsync(5, token);
			var task2 = controller.AddFloorRequestAsync(5, token);

			Assert.Single(controller.Destinations);
		}

		[Fact]
		public async Task AddFloorRequestAsync_SetsIsRunningAndProcessesRequest()
		{
			var controller = new ElevatorController(1, 0, 10);
			var tokenSource = new CancellationTokenSource();

			// Add a floor request and cancel after a short delay to avoid waiting for Task.Delay
			var task = controller.AddFloorRequestAsync(5, tokenSource.Token);

			Assert.False(controller.IsIdle);
			await task;
			Assert.True(task.IsCompleted);
		}

		[Fact]
		public async Task AddFloorRequestAsync_DoesNotProcessIfCancelledImmediately()
		{
			var controller = new ElevatorController(1, 0, 10);
			var tokenSource = new CancellationTokenSource();
			tokenSource.Cancel();

			await controller.AddFloorRequestAsync(5, tokenSource.Token);

			// Should not add the request
			Assert.Empty(controller.Destinations);
		}
	}
}