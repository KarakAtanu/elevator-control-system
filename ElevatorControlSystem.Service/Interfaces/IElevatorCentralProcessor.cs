using ElevatorControlSystem.Service.Request;

namespace ElevatorControlSystem.Service.Interfaces
{
	public interface IElevatorCentralProcessor
	{
		void HandleRequest(ElevatorRequest request);
	}
}