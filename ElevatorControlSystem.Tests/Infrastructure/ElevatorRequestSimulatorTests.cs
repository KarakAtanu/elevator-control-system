using ElevatorControlSystem.Common.Interfaces;
using ElevatorControlSystem.Common.Settings;
using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Infrastructure.Services;
using ElevatorControlSystem.Service.Interfaces;
using Microsoft.Extensions.Options;
using Moq;

namespace ElevatorControlSystem.Tests.Infrastructure
{
	public class ElevatorRequestSimulatorTests
	{
		[Fact]
		public async Task RunAsync_ProcessesValidRequests()
		{
			// Arrange
			var processorMock = new Mock<IElevatorCentralProcessor>();
			var consoleWriterMock = new Mock<IElevatorConsoleWriterService>();
			var settings = new ElevatorSettings
			{
				MinFloor = 1,
				MaxFloor = 5,
				BetweenUserActionsDelay = 1
			};
			var options = Options.Create(settings);
			var simulator = new ElevatorRequestSimulator(processorMock.Object, options, consoleWriterMock.Object);
			using var cts = new CancellationTokenSource();

			// Act
			var runTask = simulator.RunAsync(cts.Token);
			cts.CancelAfter(20); // Let it run briefly
			await runTask;

			// Assert
			processorMock.Verify(p => p.HandleRequest(It.Is<ElevatorRequest>(r =>
				r.Floor >= settings.MinFloor &&
				r.Floor <= settings.MaxFloor &&
				r.DestinationFloor >= settings.MinFloor &&
				r.DestinationFloor <= settings.MaxFloor &&
				r.Floor != r.DestinationFloor &&
				(r.Direction == Direction.Up || r.Direction == Direction.Down)
			)), Times.AtLeastOnce());

			consoleWriterMock.Verify(w => w.Write(
				It.Is<string>(msg =>
					msg.Contains("[User Action] Floor") &&
					(msg.Contains("Up") || msg.Contains("Down"))
				),
				null
			), Times.AtLeastOnce());
		}

		[Fact]
		public async Task RunAsync_HandlesOperationCanceledExceptionGracefully()
		{
			// Arrange
			var processorMock = new Mock<IElevatorCentralProcessor>();
			var consoleWriterMock = new Mock<IElevatorConsoleWriterService>();
			var settings = new ElevatorSettings
			{
				MinFloor = 1,
				MaxFloor = 3,
				BetweenUserActionsDelay = 1
			};
			var options = Options.Create(settings);
			var simulator = new ElevatorRequestSimulator(processorMock.Object, options, consoleWriterMock.Object);
			using var cts = new CancellationTokenSource();
			cts.Cancel();

			// Act
			await simulator.RunAsync(cts.Token);

			// Assert
			// No error message should be written
			consoleWriterMock.Verify(w => w.Write(
				It.Is<string>(msg => msg.Contains("Error")),
				null
			), Times.Never());
		}
	}
}