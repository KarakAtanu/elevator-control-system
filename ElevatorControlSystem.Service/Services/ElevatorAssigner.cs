using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Service.Interfaces;
using ElevatorControlSystem.Service.Request;

namespace ElevatorControlSystem.Service.Services
{
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
