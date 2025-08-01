using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Service.Interfaces;

namespace ElevatorControlSystem.Service.Services
{
	/// <summary>
	/// Manages floor requests for an elevator system, maintaining separate queues for upward and downward requests.
	/// </summary>
	/// <remarks>This class provides thread-safe methods to add, retrieve, and remove floor requests for both upward
	/// and downward directions. It ensures that requests are processed in an ordered manner, with upward requests
	/// prioritized by the lowest floor and downward requests prioritized by the highest floor.</remarks>
	public class FloorRequestQueueManager : IFloorRequestQueueManager
	{
		private readonly SortedSet<int> _up = [];
		private readonly SortedSet<int> _down = [];
		private readonly object _lock = new();

		public void AddRequest(int floor, Direction direction)
		{
			lock (_lock)
			{
				if (direction == Direction.Up)
				{
					_up.Add(floor);
				}
				else if (direction == Direction.Down)
				{
					_down.Add(floor);
				}
			}
		}

		public int? GetNextUp() => _up.Count > 0 ? _up.Min : null;
		public int? GetNextDown() => _down.Count > 0 ? _down.Max : null;
		public bool HasUpRequests() => _up.Count > 0;
		public bool HasDownRequests() => _down.Count > 0;

		public void RemoveUp(int floor)
		{
			lock (_lock)
			{
				_up.Remove(floor);
			}
		}

		public void RemoveDown(int floor)
		{
			lock (_lock)
			{
				_down.Remove(floor);
			}
		}
	}
}
