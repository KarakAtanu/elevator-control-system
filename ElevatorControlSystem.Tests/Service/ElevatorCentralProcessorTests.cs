using ElevatorControlSystem.Common.Interfaces;
using ElevatorControlSystem.Common.Settings;
using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Service.Interfaces;
using ElevatorControlSystem.Service.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace ElevatorControlSystem.Tests.Service
{
	public class ElevatorCentralProcessorTests
	{
		const string ASSIGNMENT_FOR_REQUEST = "Assigned for Request";

		private ElevatorCentralProcessor CreateProcessor(
			Mock<IRequestValidator>? validatorMock = null,
			Mock<IElevatorAssigner>? assignerMock = null,
			Mock<IRequestQueueManager>? queueManagerMock = null,
			Mock<IElevatorConsoleWriterService>? consoleWriterMock = null,
			Mock<IElevatorControllerFactory>? controllerFactoryMock = null,
			List<IElevatorController>? controllers = null)
		{
			var settings = new ElevatorSettings { MinFloor = 1, MaxFloor = 10, ElevatorCount = 2 };
			var options = Options.Create(settings);

			validatorMock ??= new Mock<IRequestValidator>();
			assignerMock ??= new Mock<IElevatorAssigner>();
			queueManagerMock ??= new Mock<IRequestQueueManager>();
			consoleWriterMock ??= new Mock<IElevatorConsoleWriterService>();
			controllerFactoryMock ??= new Mock<IElevatorControllerFactory>();

			controllers ??= new List<IElevatorController> { new Mock<IElevatorController>().Object };
			controllerFactoryMock.Setup(f => f.CreateControllers(It.IsAny<ElevatorSettings>())).Returns(controllers);

			return new ElevatorCentralProcessor(
				options,
				controllerFactoryMock.Object,
				validatorMock.Object,
				assignerMock.Object,
				queueManagerMock.Object,
				consoleWriterMock.Object
			);
		}

		[Fact]
		public void HandleRequest_InvalidRequest_DoesNotEnqueue()
		{
			// Arrange
			var validatorMock = new Mock<IRequestValidator>();
			var queueManagerMock = new Mock<IRequestQueueManager>();
			var request = new ElevatorRequest { Floor = 1, DestinationFloor = 2, Direction = Direction.Up };
			validatorMock.Setup(v => v.IsValid(request)).Returns(false);
			var processor = CreateProcessor(validatorMock, null, queueManagerMock);

			// Act
			processor.HandleRequest(request);

			// Assert
			queueManagerMock.Verify(q => q.Enqueue(It.IsAny<ElevatorRequest>()), Times.Never);
		}

		[Fact]
		public void HandleRequest_ValidRequest_EnqueuesRequest()
		{
			// Arrange
			var validatorMock = new Mock<IRequestValidator>();
			var queueManagerMock = new Mock<IRequestQueueManager>();
			var request = new ElevatorRequest { Floor = 1, DestinationFloor = 2, Direction = Direction.Up };
			validatorMock.Setup(v => v.IsValid(request)).Returns(true);
			var processor = CreateProcessor(validatorMock, null, queueManagerMock);

			// Act
			processor.HandleRequest(request);

			// Assert
			queueManagerMock.Verify(q => q.Enqueue(request), Times.Once);
		}

		// Indirectly test ProcessRequestAsync by simulating a dequeue and checking interactions
		[Fact]
		public async Task HandleRequest_ValidRequest_AssignsElevatorAndWritesConsole()
		{
			// Arrange
			var validatorMock = new Mock<IRequestValidator>();
			var assignerMock = new Mock<IElevatorAssigner>();
			var queueManagerMock = new Mock<IRequestQueueManager>();
			var consoleWriterMock = new Mock<IElevatorConsoleWriterService>();
			var elevatorMock = new Mock<IElevatorController>();
			elevatorMock.SetupGet(e => e.Id).Returns(1);
			elevatorMock.SetupGet(e => e.CurrentFloor).Returns(1);
			elevatorMock.Setup(e => e.AddFloorRequestAsync(It.IsAny<IReadOnlyList<ElevatorControllerRequest>>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);
			assignerMock.Setup(a => a.Assign(It.IsAny<ElevatorRequest>(), It.IsAny<IReadOnlyList<IElevatorController>>()))
				.Returns(elevatorMock.Object);

			var request = new ElevatorRequest { Floor = 1, DestinationFloor = 2, Direction = Direction.Up };
			validatorMock.Setup(v => v.IsValid(request)).Returns(true);

			// Simulate TryDequeue returning the request once, then false
			var dequeueQueue = new Queue<ElevatorRequest?>();
			dequeueQueue.Enqueue(request);
			queueManagerMock.Setup(q => q.TryDequeue(out It.Ref<ElevatorRequest?>.IsAny))
				.Returns((out ElevatorRequest? r) => { r = dequeueQueue.Count > 0 ? dequeueQueue.Dequeue() : null; return r != null; });

			var processor = CreateProcessor(validatorMock, assignerMock, queueManagerMock, consoleWriterMock, null, new List<IElevatorController> { elevatorMock.Object });

			// Act
			processor.HandleRequest(request);

			// Give time for background processing
			await Task.Delay(100);

			// Assert
			assignerMock.Verify(a => a.Assign(request, It.IsAny<IReadOnlyList<IElevatorController>>()), Times.AtLeastOnce);
			consoleWriterMock.Verify(c => c.Write(It.Is<string>(s => s.Contains(ASSIGNMENT_FOR_REQUEST
				)), 1), Times.AtLeastOnce);
			elevatorMock.Verify(e => e.AddFloorRequestAsync(It.IsAny<IReadOnlyList<ElevatorControllerRequest>>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
		}
	}
}