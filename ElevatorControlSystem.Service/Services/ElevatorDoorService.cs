using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Service.Interfaces;

namespace ElevatorControlSystem.Service.Services
{
	public class ElevatorDoorService : IElevatorDoorService
	{
		public async Task OpenDoorsAsync(Elevator elevator, CancellationToken cancellationToken)
		{
			Console.WriteLine($"[Elevator {elevator.Id}] At floor {elevator.CurrentFloor} - Doors are opening");
			await Task.Delay(2000, cancellationToken);
			Console.WriteLine($"[Elevator {elevator.Id}] At floor {elevator.CurrentFloor} - Doors are closing");
		}
	}
}
