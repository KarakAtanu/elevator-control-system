using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Service.Interfaces;

namespace ElevatorControlSystem.Service.Services
{
	public class ElevatorFactory : IElevatorFactory
	{
		public Elevator Create(int id, int minFloor, int maxFloor) => new(id, minFloor, maxFloor);
	}
}