using ElevatorControlSystem.Common.Settings;
using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Service.Interfaces;
using Microsoft.Extensions.Options;

namespace ElevatorControlSystem.Service.Services
{
	/// <summary>
	/// Manages the operation of an elevator, including handling floor requests, controlling movement, and managing door
	/// operations.
	/// </summary>
	/// <remarks>This class coordinates the behavior of an elevator by processing floor requests, determining the
	/// direction of movement, and interacting with services for movement and door control. It ensures that the elevator
	/// operates efficiently and handles requests in the correct order.</remarks>
	public class ElevatorController : IElevatorController
	{
		private readonly ElevatorSettings _elevatorSettings;
		private readonly Elevator _elevator;
		private readonly IFloorRequestQueueManager _queueManager;
		private readonly IElevatorMovementService _movementService;
		private readonly IElevatorDoorService _doorService;
		private readonly object _lock = new();
		private bool _isRunning = false;
		private bool _isDoorOpened = false;
		private int _destinationFloor = -1;

		public bool IsIdle => _elevator.Direction == Direction.Idle;
		public int CurrentFloor => _elevator.CurrentFloor;
		public int Id => _elevator.Id;
		public Direction Direction => _elevator.Direction;

		public ElevatorController(int id,
							IOptions<ElevatorSettings> options,
							IFloorRequestQueueManager queueManager,
							IElevatorMovementService movementService,
							IElevatorDoorService doorService)
		{
			_elevatorSettings = options.Value;
			_elevator = Elevator.Create(id, _elevatorSettings.MinFloor, _elevatorSettings.MaxFloor);
			_queueManager = queueManager;
			_movementService = movementService;
			_doorService = doorService;
		}

		public async Task AddFloorRequestAsync(IReadOnlyList<ElevatorControllerRequest> floorRequests, CancellationToken cancellationToken)
		{
			AddRequestsToQueue(floorRequests);
			SetInitialElevatorDirection(floorRequests);

			if (!_isRunning)
			{
				_isRunning = true;
				await RunElevatorAsync(cancellationToken);
			}
		}

		private void AddRequestsToQueue(IReadOnlyList<ElevatorControllerRequest> floorRequests)
		{
			foreach (var request in floorRequests)
			{
				_queueManager.AddRequest(request.Floor, request.Direction);
			}
		}

		private void SetInitialElevatorDirection(IReadOnlyList<ElevatorControllerRequest> floorRequests)
		{
			if (_elevator.Direction == Direction.Idle && floorRequests.Count > 0)
			{
				_elevator.Direction = floorRequests[0].Direction;
			}
		}

		private async Task RunElevatorAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				await ProcessElevatorStepAsync(cancellationToken);
			}
			ResetElevatorState();
		}

		private async Task ProcessElevatorStepAsync(CancellationToken cancellationToken)
		{
			lock (_lock)
			{
				UpdateDirectionAndDestination();
			}

			if (_elevator.Direction == Direction.Idle)
			{
				return;
			}

			if (IsAtDestinationFloor())
			{
				await HandleArrivalAtDestinationAsync(cancellationToken);
			}
			else
			{
				MoveElevator();
			}

			await Task.Delay(_elevatorSettings.BetweenFloorsDelay, cancellationToken);
		}

		private void UpdateDirectionAndDestination()
		{
			switch (_elevator.Direction)
			{
				case Direction.Up:
					HandleUpDirection();
					break;
				case Direction.Down:
					HandleDownDirection();
					break;
				default:
					break;
			}
		}

		private void HandleUpDirection()
		{
			if (!_queueManager.HasUpRequests())
			{
				if (_queueManager.HasDownRequests())
				{
					_elevator.Direction = Direction.Down;
					_destinationFloor = _queueManager.GetNextDown() ?? -1;
				}
				else
				{
					_elevator.Direction = Direction.Idle;
					_destinationFloor = -1;
				}
			}
			else
			{
				_destinationFloor = _queueManager.GetNextUp() ?? -1;
			}
		}

		private void HandleDownDirection()
		{
			if (!_queueManager.HasDownRequests())
			{
				if (_queueManager.HasUpRequests())
				{
					_elevator.Direction = Direction.Up;
					_destinationFloor = _queueManager.GetNextUp() ?? -1;
				}
				else
				{
					_elevator.Direction = Direction.Idle;
					_destinationFloor = -1;
				}
			}
			else
			{
				_destinationFloor = _queueManager.GetNextDown() ?? -1;
			}
		}

		private bool IsAtDestinationFloor() => _elevator.CurrentFloor == _destinationFloor;

		private async Task HandleArrivalAtDestinationAsync(CancellationToken cancellationToken)
		{
			if (!_isDoorOpened)
			{
				await _doorService.OpenDoorsAsync(_elevator, _elevatorSettings.DoorsOpenCloseDelay, cancellationToken);
				_isDoorOpened = true;
			}

			lock (_lock)
			{
				RemoveCurrentFloorRequest();
				UpdateDirectionAfterStop();
			}
		}

		private void RemoveCurrentFloorRequest()
		{
			if (_elevator.Direction == Direction.Up)
			{
				_queueManager.RemoveUp(_destinationFloor);
			}
			else if (_elevator.Direction == Direction.Down)
			{
				_queueManager.RemoveDown(_destinationFloor);
			}
		}

		private void UpdateDirectionAfterStop()
		{
			if (_elevator.Direction == Direction.Up && !_queueManager.HasUpRequests())
			{
				_elevator.Direction = _queueManager.HasDownRequests() ? Direction.Down : Direction.Idle;
			}
			else if (_elevator.Direction == Direction.Down && !_queueManager.HasDownRequests())
			{
				_elevator.Direction = _queueManager.HasUpRequests() ? Direction.Up : Direction.Idle;
			}
		}

		private void MoveElevator()
		{
			if (_elevator.Direction == Direction.Up)
			{
				_movementService.MoveUp(_elevator);
				_isDoorOpened = false;
			}
			else if (_elevator.Direction == Direction.Down)
			{
				_movementService.MoveDown(_elevator);
				_isDoorOpened = false;
			}
		}

		private void ResetElevatorState()
		{
			_isRunning = false;
			_isDoorOpened = false;
		}
	}
}