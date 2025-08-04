using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Service.Interfaces;

namespace ElevatorControlSystem.Service.Services
{
	/// <summary>
	/// Provides functionality to find the most suitable elevator and assign an elevator to a given request based on the current 
	/// state of available elevators.
	/// </summary>
	/// <remarks>The <see cref="ElevatorAssigner"/> class evaluates the direction and floor of the elevator request,
	/// as well as the current state of the elevators, to determine the most suitable elevator to handle the request.
	/// Elevators that are already moving in the requested direction are prioritized, followed by idle elevators.</remarks>
	public class ElevatorAssigner : IElevatorAssigner
	{
		public IElevatorController? Assign(ElevatorRequest request, IReadOnlyList<IElevatorController> elevatorControllers)
		{
			var candidates = Enumerable.Empty<IElevatorController>();

			if (request.Direction == Direction.Up)
			{
				candidates = FindUpwardElevators(request, elevatorControllers);
			}
			else if (request.Direction == Direction.Down)
			{
				candidates = FindDownwardElevators(request, elevatorControllers);
			}

			return candidates.FirstOrDefault();
		}

		private static IEnumerable<IElevatorController> FindDownwardElevators(ElevatorRequest request,
																		IReadOnlyList<IElevatorController> elevatorControllers)
		{
			IEnumerable<IElevatorController> candidates = elevatorControllers
								.Where(e => e.Direction == Direction.Down && e.CurrentFloor >= request.Floor)
								.OrderBy(e => e.CurrentFloor - request.Floor);

			if (!candidates.Any())
			{
				candidates = GetIdleElevators(request, elevatorControllers);
			}

			return candidates;
		}

		private static IEnumerable<IElevatorController> GetIdleElevators(ElevatorRequest request,
																   IReadOnlyList<IElevatorController> elevatorControllers) =>
							elevatorControllers
							.Where(e => e.Direction == Direction.Idle)
							.OrderBy(e => Math.Abs(request.Floor - e.CurrentFloor));

		private static IEnumerable<IElevatorController> FindUpwardElevators(ElevatorRequest request,
																	  IReadOnlyList<IElevatorController> elevatorControllers)
		{
			IEnumerable<IElevatorController> candidates = elevatorControllers
								.Where(e => e.Direction == Direction.Up && e.CurrentFloor <= request.Floor)
								.OrderBy(e => request.Floor - e.CurrentFloor);

			if (!candidates.Any())
			{
				candidates = GetIdleElevators(request, elevatorControllers);
			}

			return candidates;
		}
	}
}
