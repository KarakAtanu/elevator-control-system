using ElevatorControlSystem.Common.Settings;

namespace ElevatorControlSystem.Service.Interfaces
{
	public interface IElevatorControllerFactory
	{
		List<IElevatorController> CreateControllers(ElevatorSettings settings);
	}
}