using ElevatorControlSystem.Common.Interfaces;
using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Service.Services;
using Moq;

namespace ElevatorControlSystem.Service.Tests.Services
{
	public class ElevatorDoorServiceTests
	{
		[Fact]
		public async Task OpenDoorsAsync_WritesOpenAndCloseMessages()
		{
			// Arrange
			var mockConsoleWriter = new Mock<IElevatorConsoleWriterService>();
			var service = new ElevatorDoorService(mockConsoleWriter.Object);
			var elevator = Elevator.Create(1, 0, 10);
			int delay = 10;
			var cancellationToken = CancellationToken.None;

			// Act
			await service.OpenDoorsAsync(elevator, delay, cancellationToken);

			// Assert
			mockConsoleWriter.Verify(
				x => x.Write("[Elevator 1] At floor 0 - Doors are opening", 1),
				Times.Once);
			mockConsoleWriter.Verify(
				x => x.Write("[Elevator 1] At floor 0 - Doors are closing", 1),
				Times.Once);
		}
	}
}