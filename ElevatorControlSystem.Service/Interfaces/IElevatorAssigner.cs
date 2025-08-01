using ElevatorControlSystem.Domain.Models;

namespace ElevatorControlSystem.Service.Interfaces
{
	public interface IElevatorAssigner
	{
		IElevatorController? Assign(ElevatorRequest request, IReadOnlyList<IElevatorController> elevatorControllers);
	}
}