using ElevatorControlSystem.Domain.Models.Enums;

namespace ElevatorControlSystem.Domain.Models
{
	public class Elevator
	{
		public int Id { get; private set; }
		public int MinFloor { get; private set; }
		public int MaxFloor { get; private set; }
		public Direction Direction { get; set; }
		public int CurrentFloor { get; set; }

		public Elevator(int id, int minFloor, int maxFloor)
		{
			Id = id;
			MinFloor = minFloor;
			MaxFloor = maxFloor;
			CurrentFloor = MinFloor;
			Direction = Direction.Idle;
		}

	}	
}
