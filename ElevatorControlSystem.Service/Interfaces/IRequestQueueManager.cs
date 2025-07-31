using ElevatorControlSystem.Service.Request;

namespace ElevatorControlSystem.Service.Interfaces
{
	public interface IRequestQueueManager
	{
		void Enqueue(ElevatorRequest request);
		bool TryDequeue(out ElevatorRequest? request);
	}
}