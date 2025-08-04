using ElevatorControlSystem.Domain.Models;

namespace ElevatorControlSystem.Service.Interfaces
{
	public interface IElevatorMovementService
	{
		void MoveDown(Elevator elevator);
		void MoveUp(Elevator elevator);
	}
}