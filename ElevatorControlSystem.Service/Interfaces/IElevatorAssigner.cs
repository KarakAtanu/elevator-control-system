using ElevatorControlSystem.Service.Request;

namespace ElevatorControlSystem.Service.Interfaces
{
	public interface IElevatorAssigner
	{
		IElevatorController? Assign(ElevatorRequest request, IReadOnlyList<IElevatorController> elevatorControllers);
	}
}