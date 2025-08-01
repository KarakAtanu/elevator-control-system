using ElevatorControlSystem.Domain.Models;

namespace ElevatorControlSystem.Service.Interfaces
{
	public interface IElevatorCentralProcessor
	{
		void HandleRequest(ElevatorRequest request);
	}
}