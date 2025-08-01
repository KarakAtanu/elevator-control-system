using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Service.Interfaces;

namespace ElevatorControlSystem.Service.Services
{
	public class ElevatorMovementService : IElevatorMovementService
	{
		public void MoveUp(Elevator elevator)
		{
			elevator.CurrentFloor++;
			Console.WriteLine($"[Elevator {elevator.Id}] Moving Up to floor {elevator.CurrentFloor}");
		}

		public void MoveDown(Elevator elevator)
		{
			elevator.CurrentFloor--;
			Console.WriteLine($"[Elevator {elevator.Id}] Moving Down to floor {elevator.CurrentFloor}");
		}
	}
}