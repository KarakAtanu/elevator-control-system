using ElevatorControlSystem.Common.Interfaces;
using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Service.Interfaces;

namespace ElevatorControlSystem.Service.Services
{
	/// <summary>
	/// Provides functionality to manage the opening and closing of elevator doors.
	/// </summary>
	/// <remarks>This service handles the asynchronous operation of opening and closing elevator doors.  It ensures
	/// that the doors remain open for a specified duration before closing.</remarks>
	public class ElevatorDoorService : IElevatorDoorService
	{
		private readonly IElevatorConsoleWriterService _consoleWriterService;

		public ElevatorDoorService(IElevatorConsoleWriterService consoleWriterService) 
		{
			_consoleWriterService = consoleWriterService;
		}
		public async Task OpenDoorsAsync(Elevator elevator, int doorsOpenCloseDelay, CancellationToken cancellationToken)
		{
			_consoleWriterService.Write($"[Elevator {elevator.Id}] At floor {elevator.CurrentFloor} - Doors are opening", elevator.Id);
			await Task.Delay(doorsOpenCloseDelay, cancellationToken);
			_consoleWriterService.Write($"[Elevator {elevator.Id}] At floor {elevator.CurrentFloor} - Doors are closing", elevator.Id);
		}
	}
}
