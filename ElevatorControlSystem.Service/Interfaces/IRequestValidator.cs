using ElevatorControlSystem.Domain.Models;

namespace ElevatorControlSystem.Service.Interfaces
{
	public interface IRequestValidator
	{
		bool IsValid(ElevatorRequest request);
	}
}