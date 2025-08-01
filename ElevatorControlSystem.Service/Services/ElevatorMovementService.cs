using ElevatorControlSystem.Common.Interfaces;
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
		private readonly IElevatorConsoleWriterService _consoleWriterService;

		public ElevatorMovementService(IElevatorConsoleWriterService consoleWriterService)
		{
			_consoleWriterService = consoleWriterService;
		}

		public void MoveUp(Elevator elevator)
		{
			elevator.CurrentFloor++;
			_consoleWriterService.Write($"[Elevator {elevator.Id}] Moving Up to floor {elevator.CurrentFloor}", elevator.Id);
		}

		public void MoveDown(Elevator elevator)
		{
			elevator.CurrentFloor--;
			_consoleWriterService.Write($"[Elevator {elevator.Id}] Moving Down to floor {elevator.CurrentFloor}", elevator.Id);
		}
	}
}