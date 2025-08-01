using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Service.Interfaces;

namespace ElevatorControlSystem.Service.Services
{
	/// <summary>
	/// Provides functionality to control the movement of elevators, allowing them to move up or down between floors.
	/// </summary>
	/// <remarks>This service is responsible for updating the current floor of the elevator and logging its
	/// movement. It assumes that the caller ensures the elevator's movement is within valid floor boundaries.</remarks>
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