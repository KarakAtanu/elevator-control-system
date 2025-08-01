using ElevatorControlSystem.Domain.Models;

namespace ElevatorControlSystem.Service.Interfaces
{
	public interface IElevatorDoorService
	{
		Task OpenDoorsAsync(Elevator elevator, CancellationToken cancellationToken);
	}
}