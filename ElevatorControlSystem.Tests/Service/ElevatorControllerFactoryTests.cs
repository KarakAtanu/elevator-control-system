using ElevatorControlSystem.Common.Settings;
using ElevatorControlSystem.Service.Interfaces;
using ElevatorControlSystem.Service.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace ElevatorControlSystem.Tests.Service
{
	public class ElevatorControllerFactoryTests
	{
		[Fact]
		public void CreateControllers_ReturnsCorrectNumberOfControllers()
		{
			// Arrange
			var settings = new ElevatorSettings
			{
				ElevatorCount = 3,
				MinFloor = 1,
				MaxFloor = 10,
				BetweenFloorsDelay = 1000,
				DoorsOpenCloseDelay = 500,
				BetweenUserActionsDelay = 200
			};

			var serviceProviderMock = new Mock<IServiceProvider>();
			var optionsMock = new Mock<IOptions<ElevatorSettings>>();
			optionsMock.Setup(o => o.Value).Returns(settings);

			var queueManagerMock = new Mock<IFloorRequestQueueManager>();
			var movementServiceMock = new Mock<IElevatorMovementService>();
			var doorServiceMock = new Mock<IElevatorDoorService>();

			serviceProviderMock.Setup(sp => sp.GetService(typeof(IFloorRequestQueueManager)))
				.Returns(queueManagerMock.Object);
			serviceProviderMock.Setup(sp => sp.GetService(typeof(IElevatorMovementService)))
				.Returns(movementServiceMock.Object);
			serviceProviderMock.Setup(sp => sp.GetService(typeof(IElevatorDoorService)))
				.Returns(doorServiceMock.Object);

			var factory = new ElevatorControllerFactory(serviceProviderMock.Object, optionsMock.Object);

			// Act
			var controllers = factory.CreateControllers(settings);

			// Assert
			Assert.NotNull(controllers);
			Assert.Equal(settings.ElevatorCount, controllers.Count);
			for (int i = 0; i < controllers.Count; i++)
			{
				Assert.Equal(i + 1, controllers[i].Id);
			}
		}

		[Fact]
		public void CreateController_ReturnsControllerWithCorrectId()
		{
			// Arrange
			var settings = new ElevatorSettings
			{
				ElevatorCount = 2,
				MinFloor = 1,
				MaxFloor = 5,
				BetweenFloorsDelay = 1000,
				DoorsOpenCloseDelay = 500,
				BetweenUserActionsDelay = 200
			};

			var serviceProviderMock = new Mock<IServiceProvider>();
			var optionsMock = new Mock<IOptions<ElevatorSettings>>();
			optionsMock.Setup(o => o.Value).Returns(settings);

			var queueManagerMock = new Mock<IFloorRequestQueueManager>();
			var movementServiceMock = new Mock<IElevatorMovementService>();
			var doorServiceMock = new Mock<IElevatorDoorService>();

			serviceProviderMock.Setup(sp => sp.GetService(typeof(IFloorRequestQueueManager)))
				.Returns(queueManagerMock.Object);
			serviceProviderMock.Setup(sp => sp.GetService(typeof(IElevatorMovementService)))
				.Returns(movementServiceMock.Object);
			serviceProviderMock.Setup(sp => sp.GetService(typeof(IElevatorDoorService)))
				.Returns(doorServiceMock.Object);

			var factory = new ElevatorControllerFactory(serviceProviderMock.Object, optionsMock.Object);

			int testId = 2;

			// Act
			var controller = factory.CreateController(settings, testId);

			// Assert
			Assert.NotNull(controller);
			Assert.Equal(testId, controller.Id);
		}
	}
}