using ElevatorControlSystem.Domain.Models.Enums;

namespace ElevatorControlSystem.Domain.Models
{
	public class ElevatorControllerRequest
	{
		public int Floor { get; set; }
		public Direction Direction { get; set; }
	}
}
