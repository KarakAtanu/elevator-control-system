using ElevatorControlSystem.Domain.Models;

namespace ElevatorControlSystem.Service.Interfaces
{
	public interface IElevatorFactory
	{
		Elevator Create(int id, int minFloor, int maxFloor);
	}
}