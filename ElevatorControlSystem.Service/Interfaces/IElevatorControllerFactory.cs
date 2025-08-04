using ElevatorControlSystem.Common.Settings;

namespace ElevatorControlSystem.Service.Interfaces
{
	public interface IElevatorControllerFactory
	{
		List<IElevatorController> CreateControllers(ElevatorSettings settings);
		IElevatorController CreateController(ElevatorSettings settings, int id);
	}
}