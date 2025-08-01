using ElevatorControlSystem.Common.Settings;
using ElevatorControlSystem.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorControlSystem.Service.Services
{
	/// <summary>
	/// Provides methods to create and configure instances of <see cref="IElevatorController"/>  based on the specified
	/// elevator settings.
	/// </summary>
	/// <remarks>This factory leverages dependency injection to resolve required services for creating  elevator
	/// controllers, such as floor request queue management, movement services, and door services.</remarks>
	public class ElevatorControllerFactory : IElevatorControllerFactory
	{
		private readonly IServiceProvider _serviceProvider;

		public ElevatorControllerFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}
		public List<IElevatorController> CreateControllers(ElevatorSettings settings)
		{
			return Enumerable.Range(1, settings.ElevatorCount)
				.Select(i =>
				{
					return CreateController(settings, i);
				})
				.Cast<IElevatorController>()
				.ToList();

		}

		public IElevatorController CreateController(ElevatorSettings settings, int id)
		{
			var queueManager = _serviceProvider.GetRequiredService<IFloorRequestQueueManager>();
			var movementService = _serviceProvider.GetRequiredService<IElevatorMovementService>();
			var doorService = _serviceProvider.GetRequiredService<IElevatorDoorService>();
			return new ElevatorController(id, settings.MinFloor, settings.MaxFloor, queueManager, movementService, doorService);
		}
	}
}
