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
		public async Task OpenDoorsAsync(Elevator elevator, int doorsOpenCloseDelay, CancellationToken cancellationToken)
		{
			Console.WriteLine($"[Elevator {elevator.Id}] At floor {elevator.CurrentFloor} - Doors are opening");
			await Task.Delay(doorsOpenCloseDelay, cancellationToken);
			Console.WriteLine($"[Elevator {elevator.Id}] At floor {elevator.CurrentFloor} - Doors are closing");
		}
	}
}
