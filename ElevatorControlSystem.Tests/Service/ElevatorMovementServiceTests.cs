using ElevatorControlSystem.Common.Interfaces;
using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Service.Services;
using Moq;

namespace ElevatorControlSystem.Tests.Service
{
	public class ElevatorMovementServiceTests
	{
		[Fact]
		public void MoveUp_IncrementsCurrentFloor_AndLogsMovement()
		{
			// Arrange
			var mockConsoleWriter = new Mock<IElevatorConsoleWriterService>();
			var service = new ElevatorMovementService(mockConsoleWriter.Object);
			var elevator = Elevator.Create(id: 1, minFloor: 0, maxFloor: 10);
			elevator.CurrentFloor = 5;

			// Act
			service.MoveUp(elevator);

			// Assert
			Assert.Equal(6, elevator.CurrentFloor);
			mockConsoleWriter.Verify(
				m => m.Write("[Elevator 1] Moving Up to floor 6", 1),
				Times.Once);
		}

		[Fact]
		public void MoveDown_DecrementsCurrentFloor_AndLogsMovement()
		{
			// Arrange
			var mockConsoleWriter = new Mock<IElevatorConsoleWriterService>();
			var service = new ElevatorMovementService(mockConsoleWriter.Object);
			var elevator = Elevator.Create(id: 2, minFloor: 0, maxFloor: 10);
			elevator.CurrentFloor = 5;

			// Act
			service.MoveDown(elevator);

			// Assert
			Assert.Equal(4, elevator.CurrentFloor);
			mockConsoleWriter.Verify(
				m => m.Write("[Elevator 2] Moving Down to floor 4", 2),
				Times.Once);
		}
	}
}
