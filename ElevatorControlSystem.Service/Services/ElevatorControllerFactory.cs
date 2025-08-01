using ElevatorControlSystem.Common.Settings;
using ElevatorControlSystem.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorControlSystem.Service.Services
{
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
