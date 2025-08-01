using ElevatorControlSystem.Domain.Models.Enums;

namespace ElevatorControlSystem.Service.Interfaces
{
	public interface IFloorRequestQueueManager
	{
		void AddRequest(int floor, Direction direction);
		int? GetNextDown();
		int? GetNextUp();
		bool HasDownRequests();
		bool HasUpRequests();
		void RemoveDown(int floor);
		void RemoveUp(int floor);
	}
}