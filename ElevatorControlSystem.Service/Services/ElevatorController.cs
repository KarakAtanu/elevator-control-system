using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Service.Interfaces;

namespace ElevatorControlSystem.Service.Services
{
	public class ElevatorController : IElevatorController
	{
		private readonly Elevator _elevator;
		private readonly object _lock = new();
		private bool _isRunning = false;
		private bool _isDoorOpened = false;
		private int _destinationFloor = -1;

		private readonly SortedSet<int> _movingUpDestinationFloors = new();
		private readonly SortedSet<int> _movingDownDestinationFloors = new();

		public bool IsIdle => _elevator.Direction == Direction.Idle;
		public int CurrentFloor => _elevator.CurrentFloor;
		public int Id => _elevator.Id;
		public Direction Direction => _elevator.Direction;
		public ElevatorController(int id, int minFloor, int maxFloor)
		{
			_elevator = new Elevator(id, minFloor, maxFloor);
		}

		public async Task AddFloorRequestAsync(IReadOnlyList<ElevatorControllerRequest> floorRequests, CancellationToken cancellationToken)
		{
			lock (_lock)
			{
				foreach (var floorRequest in floorRequests)
				{
					if (floorRequest.Direction == Direction.Up)
					{
						_movingUpDestinationFloors.Add(floorRequest.Floor);
					}
					else
					{
						_movingDownDestinationFloors.Add(floorRequest.Floor);
					}
				}
			}

			if (_elevator.Direction == Direction.Idle)
			{
				_elevator.Direction = floorRequests[0].Direction;
			}

			if (!_isRunning)
			{
				_isRunning = true;
				await RunElevatorAsync(cancellationToken);
			}

		}

		private async Task RunElevatorAsync(CancellationToken cancellationToken)
		{
			while (true)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					_isRunning = false;
					_isDoorOpened = false;
					return;
				}

				await TransitionBetweenFloorsAsync(cancellationToken);
			}
		}

		private void SetElevatorDirectionAndDestinationFloor()
		{
			if (_elevator.Direction == Direction.Up)
			{
				if (_movingUpDestinationFloors.Count == 0)
				{
					if (_movingDownDestinationFloors.Count > 0)
					{
						_elevator.Direction = Direction.Down;
						_destinationFloor = _movingDownDestinationFloors.Last();
					}
					else
					{
						_elevator.Direction = Direction.Idle;
						_destinationFloor = -1;
					}
				}
				else
				{
					_destinationFloor = _movingUpDestinationFloors.First();
				}
			}
			else
			{
				if (_movingDownDestinationFloors.Count == 0)
				{
					if (_movingUpDestinationFloors.Count > 0)
					{
						_elevator.Direction = Direction.Up;
						_destinationFloor = _movingUpDestinationFloors.First();
					}
					else
					{
						_elevator.Direction = Direction.Idle;
						_destinationFloor = -1;
					}
				}
				else
				{
					_destinationFloor = _movingDownDestinationFloors.Last();
				}
			}
		}

		private async Task TransitionBetweenFloorsAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			lock (_lock)
			{
				SetElevatorDirectionAndDestinationFloor();
			}

			if (_elevator.Direction != Direction.Idle)
			{
				if (_elevator.CurrentFloor == _destinationFloor)
				{
					if (!_isDoorOpened)
					{
						await OpenElevatorDoorsAsync(cancellationToken);
					}

					lock (_lock)
					{
						if (_elevator.Direction == Direction.Up)
						{
							_movingUpDestinationFloors.Remove(_destinationFloor);
							if(_movingUpDestinationFloors.Count == 0)
							{
								_elevator.Direction = _movingDownDestinationFloors.Count == 0 ? Direction.Idle : Direction.Down;
							}
						}

						if (_elevator.Direction == Direction.Down)
						{
							_movingDownDestinationFloors.Remove(_destinationFloor);
							if (_movingDownDestinationFloors.Count == 0)
							{
								_elevator.Direction = _movingUpDestinationFloors.Count == 0 ? Direction.Idle : Direction.Up;
							}
						}
					}
				}

				if (_elevator.Direction == Direction.Down)
				{
					_elevator.CurrentFloor--;
					Console.WriteLine($"[Elevator {_elevator.Id}] Moving Down to floor {CurrentFloor}");
				}
				
				if( _elevator.Direction == Direction.Up)
				{
					_elevator.CurrentFloor++;
					Console.WriteLine($"[Elevator {_elevator.Id}] Moving Up to floor {CurrentFloor}");
				}

				await Task.Delay(2000, cancellationToken);
			}
		}

		private async Task OpenElevatorDoorsAsync(CancellationToken cancellationToken)
		{
			Console.WriteLine($"[Elevator {_elevator.Id}] At floor {CurrentFloor} - Doors are opening");
			await Task.Delay(2000, cancellationToken); // Simulate door open time
			Console.WriteLine($"[Elevator {_elevator.Id}] At floor {CurrentFloor} - Doors are closing");
		}
	}
}
