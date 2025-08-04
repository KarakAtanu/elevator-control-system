using ElevatorControlSystem.Domain.Models.Enums;

namespace ElevatorControlSystem.Domain.Models
{
	public class ElevatorRequest
	{
		public int Floor { get; set; }
		public Direction Direction { get; set; }
		public int DestinationFloor { get; set; }
	}
}
