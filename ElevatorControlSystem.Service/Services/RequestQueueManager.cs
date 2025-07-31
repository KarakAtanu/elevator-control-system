using System.Collections.Concurrent;
using ElevatorControlSystem.Service.Interfaces;
using ElevatorControlSystem.Service.Request;

namespace ElevatorControlSystem.Service.Services
{
	public class RequestQueueManager : IRequestQueueManager
	{
		private readonly ConcurrentQueue<ElevatorRequest> _queue = new();

		public void Enqueue(ElevatorRequest request)
		{
			_queue.Enqueue(request);
		}

		public bool TryDequeue(out ElevatorRequest? request) =>
			_queue.TryDequeue(out request!);
	}
}
