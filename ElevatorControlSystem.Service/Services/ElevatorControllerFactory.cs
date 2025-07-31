using ElevatorControlSystem.Common.Settings;
using ElevatorControlSystem.Service.Interfaces;

namespace ElevatorControlSystem.Service.Services
{
	public class ElevatorControllerFactory : IElevatorControllerFactory
	{
		public List<IElevatorController> CreateControllers(ElevatorSettings settings)
		{
			return Enumerable.Range(1, settings.ElevatorCount)
				.Select(i => new ElevatorController(i, settings.MinFloor, settings.MaxFloor))
				.Cast<IElevatorController>()
				.ToList();
		}
	}
}
