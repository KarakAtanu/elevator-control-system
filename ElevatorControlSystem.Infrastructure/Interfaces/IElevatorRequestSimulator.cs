namespace ElevatorControlSystem.Infrastructure.Interfaces
{
	public interface IElevatorRequestSimulator
	{
		Task RunAsync(CancellationToken token);
	}
}