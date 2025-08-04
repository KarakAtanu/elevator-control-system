namespace ElevatorControlSystem.Common.Interfaces
{
	public interface IElevatorConsoleWriterService
	{
		void Write(string message, int? elevatorId = null);
	}
}