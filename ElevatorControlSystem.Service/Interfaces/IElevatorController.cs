using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Domain.Models.Enums;

namespace ElevatorControlSystem.Service.Interfaces
{
	public interface IElevatorController
	{
		bool IsIdle { get; }
		int CurrentFloor { get; }
		int Id { get; }
		Direction Direction { get; }
		Task AddFloorRequestAsync(IReadOnlyList<ElevatorControllerRequest> floorRequests, CancellationToken cancellationToken);
	}
}
