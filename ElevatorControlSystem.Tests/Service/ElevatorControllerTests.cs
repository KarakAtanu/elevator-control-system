using ElevatorControlSystem.Common.Settings;
using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Service.Interfaces;
using ElevatorControlSystem.Service.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace ElevatorControlSystem.Tests.Service
{
	public class ElevatorControllerTests
	{
		private ElevatorController CreateController(
			int id = 1,
			ElevatorSettings settings = null,
			Mock<IFloorRequestQueueManager> queueManagerMock = null,
			Mock<IElevatorMovementService> movementServiceMock = null,
			Mock<IElevatorDoorService> doorServiceMock = null)
		{
			settings ??= new ElevatorSettings
			{
				MinFloor = 1,
				MaxFloor = 10,
				BetweenFloorsDelay = 10,
				DoorsOpenCloseDelay = 10,
				ElevatorCount = 1,
				BetweenUserActionsDelay = 10
			};
			queueManagerMock ??= new Mock<IFloorRequestQueueManager>();
			movementServiceMock ??= new Mock<IElevatorMovementService>();
			doorServiceMock ??= new Mock<IElevatorDoorService>();

			var options = Options.Create(settings);

			return new ElevatorController(
				id,
				options,
				queueManagerMock.Object,
				movementServiceMock.Object,
				doorServiceMock.Object
			);
		}

		[Fact]
		public void IsIdle_ShouldReturnTrue_WhenDirectionIsIdle()
		{
			// Arrange
			var controller = CreateController();
			// Act
			var result = controller.IsIdle;
			// Assert
			Assert.True(result);
		}

		[Fact]
		public void CurrentFloor_ShouldReturnElevatorCurrentFloor()
		{
			// Arrange
			var controller = CreateController();
			// Act
			var result = controller.CurrentFloor;
			// Assert
			Assert.Equal(1, result); // Default MinFloor
		}

		[Fact]
		public void Id_ShouldReturnElevatorId()
		{
			// Arrange
			var controller = CreateController(id: 5);
			// Act
			var result = controller.Id;
			// Assert
			Assert.Equal(5, result);
		}

		[Fact]
		public void Direction_ShouldReturnElevatorDirection()
		{
			// Arrange
			var controller = CreateController();
			// Act
			var result = controller.Direction;
			// Assert
			Assert.Equal(Direction.Idle, result);
		}

		[Fact]
		public async Task AddFloorRequestAsync_ShouldAddRequestsAndRunElevator()
		{
			// Arrange
			var queueManagerMock = new Mock<IFloorRequestQueueManager>();
			var movementServiceMock = new Mock<IElevatorMovementService>();
			var doorServiceMock = new Mock<IElevatorDoorService>();
			var controller = CreateController(
				queueManagerMock: queueManagerMock,
				movementServiceMock: movementServiceMock,
				doorServiceMock: doorServiceMock
			);
			var requests = new List<ElevatorControllerRequest>
		{
			new ElevatorControllerRequest { Floor = 2, Direction = Direction.Up }
		};
			var tokenSource = new CancellationTokenSource();
			tokenSource.Cancel(); // Cancel immediately to exit RunElevatorAsync

			// Act
			await controller.AddFloorRequestAsync(requests, tokenSource.Token);

			// Assert
			queueManagerMock.Verify(q => q.AddRequest(2, Direction.Up), Times.Once);
		}

		[Fact]
		public async Task AddFloorRequestAsync_ShouldSetInitialDirection()
		{
			// Arrange
			var queueManagerMock = new Mock<IFloorRequestQueueManager>();
			var movementServiceMock = new Mock<IElevatorMovementService>();
			var doorServiceMock = new Mock<IElevatorDoorService>();
			var controller = CreateController(
				queueManagerMock: queueManagerMock,
				movementServiceMock: movementServiceMock,
				doorServiceMock: doorServiceMock
			);
			var requests = new List<ElevatorControllerRequest>
		{
			new ElevatorControllerRequest { Floor = 2, Direction = Direction.Up }
		};
			var tokenSource = new CancellationTokenSource();
			tokenSource.Cancel();

			// Act
			await controller.AddFloorRequestAsync(requests, tokenSource.Token);

			// Assert
			Assert.Equal(Direction.Up, controller.Direction);
		}

		[Fact]
		public async Task AddFloorRequestAsync_ShouldNotRunElevatorIfAlreadyRunning()
		{
			// Arrange
			var queueManagerMock = new Mock<IFloorRequestQueueManager>();
			var movementServiceMock = new Mock<IElevatorMovementService>();
			var doorServiceMock = new Mock<IElevatorDoorService>();
			var controller = CreateController(
				queueManagerMock: queueManagerMock,
				movementServiceMock: movementServiceMock,
				doorServiceMock: doorServiceMock
			);
			var requests = new List<ElevatorControllerRequest>
		{
			new ElevatorControllerRequest { Floor = 2, Direction = Direction.Up }
		};
			var tokenSource = new CancellationTokenSource();
			tokenSource.Cancel();

			// Act
			await controller.AddFloorRequestAsync(requests, tokenSource.Token);
			await controller.AddFloorRequestAsync(requests, tokenSource.Token);

			// Assert
			queueManagerMock.Verify(q => q.AddRequest(2, Direction.Up), Times.Exactly(2));
		}
	}
}