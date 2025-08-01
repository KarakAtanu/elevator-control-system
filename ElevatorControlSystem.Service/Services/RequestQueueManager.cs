using System.Collections.Concurrent;
using ElevatorControlSystem.Service.Interfaces;
using ElevatorControlSystem.Service.Request;

namespace ElevatorControlSystem.Service.Services
{
	/// <summary>
	/// Manages a queue of elevator requests, providing thread-safe operations for adding and retrieving requests.
	/// </summary>
	/// <remarks>This class is designed to handle concurrent access to the queue, ensuring thread safety for both
	/// enqueue and dequeue operations. It is suitable for scenarios where multiple threads need to manage elevator
	/// requests in a coordinated manner.</remarks>
	public class RequestQueueManager : IRequestQueueManager
	{
		private readonly ConcurrentQueue<ElevatorRequest> _queue = new();

		public void Enqueue(ElevatorRequest request)
		{
			_queue.Enqueue(request);
		}

		public bool TryDequeue(out ElevatorRequest? request) =>
			_queue.TryDequeue(out request);
	}
}
