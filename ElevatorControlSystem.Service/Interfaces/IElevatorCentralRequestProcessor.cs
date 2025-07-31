using ElevatorControlSystem.Service.Request;

namespace ElevatorControlSystem.Service.Interfaces
{
	public interface IElevatorCentralRequestProcessor
	{
		void HandleRequest(ElevatorRequest request, CancellationToken cancellationToken);
	}
}