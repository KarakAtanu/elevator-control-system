using ElevatorControlSystem.Domain.Models.Enums;

namespace ElevatorControlSystem.Service.Request
{
	public class ElevatorRequest
	{
		public int Floor { get; set; }
		public Direction Direction { get; set; }
		public int DestinationFloor { get; set; }
	}
}
