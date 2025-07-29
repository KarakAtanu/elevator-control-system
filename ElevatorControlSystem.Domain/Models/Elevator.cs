using ElevatorControlSystem.Domain.Models.Enums;

namespace ElevatorControlSystem.Domain.Models
{
	public class Elevator
	{
		private readonly int _id;
		private readonly int _minFloor;
		private readonly int _maxFloor;
		private readonly object _lock = new();
		private readonly Queue<int> _desinationFloors = new();
		public int Id => _id;
		public Direction Direction { get; private set; }
		public int CurrentFloor { get; private set; }
		public bool IsIdle => Direction == Direction.Idle;
		public IReadOnlyList<int> Destinations => _desinationFloors.ToList().AsReadOnly<int>();

		public Elevator(int id, int minFloor, int maxFloor)
		{
			_id = id;
			_minFloor = minFloor;
			_maxFloor = maxFloor;
			CurrentFloor = 0;
			Direction = Direction.Idle;
		}

		public void AddFloorRequest(int requestedFloor)
		{
			if(requestedFloor < _minFloor || requestedFloor > _maxFloor)
			{
				throw new ArgumentOutOfRangeException(nameof(requestedFloor));
			}
			else
			{
				lock (_lock)
				{
					if (!_desinationFloors.Contains(requestedFloor))
					{
						_desinationFloors.Enqueue(requestedFloor);
					}
				}
			}
		}

	}
}
