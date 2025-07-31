using ElevatorControlSystem.Service.Request;

namespace ElevatorControlSystem.Service.Interfaces
{
	public interface IRequestValidator
	{
		bool IsValid(ElevatorRequest request);
	}
}