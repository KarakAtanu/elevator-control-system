using ElevatorControlSystem.Common.Settings;
using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Service.Interfaces;
using ElevatorControlSystem.Service.Request;
using Microsoft.Extensions.Options;

namespace ElevatorControlSystem.Service.Services
{
	public class ElevatorCentralRequestProcessor : IElevatorCentralRequestProcessor
	{
		private readonly CancellationTokenSource _cts = new();
		private readonly List<Task> _workers = new();
		private readonly List<IElevatorController> _elevatorControllers;
		private readonly ElevatorSettings _settings;
		private readonly IRequestValidator _validator;
		private readonly IElevatorAssigner _elevatorAssigner;
		private readonly IRequestQueueManager _queueManager;
		private const int NUMBER_OF_TASKS = 5;
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(NUMBER_OF_TASKS);
		public ElevatorCentralRequestProcessor(IOptions<ElevatorSettings> options,
										 IElevatorControllerFactory controllerFactory,
										 IRequestValidator validator,
										 IElevatorAssigner elevatorAssigner,
										 IRequestQueueManager queueManager)
		{
			_settings = options.Value;
			_validator = validator;
			_elevatorAssigner = elevatorAssigner;
			_queueManager = queueManager;
			_elevatorControllers = controllerFactory.CreateControllers(_settings);

			// Start a single background task to process requests
			_workers.Add(Task.Run(() => ProcessQueueAsync(_cts.Token)));
		}

		public void HandleRequest(ElevatorRequest request, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				_cts.Cancel();
				return;
			}

			if (!_validator.IsValid(request))
			{
				Console.WriteLine("Invalid elevator request.");
				return;
			}

			_queueManager.Enqueue(request);
		}

		private async Task ProcessQueueAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				if (_queueManager.TryDequeue(out var request))
				{
					await _semaphore.WaitAsync(cancellationToken);
					_ = Task.Run(async () =>
					{
						try
						{
							await ProcessRequestAsync(request!, cancellationToken);
						}
						finally
						{
							_semaphore.Release();
						}
					}, cancellationToken);
				}
			}
		}

		private async Task ProcessRequestAsync(ElevatorRequest request, CancellationToken cancellationToken)
		{
			var elevator = _elevatorAssigner.Assign(request, _elevatorControllers);

			if (elevator == null)
			{
				return;
			}

			Console.WriteLine($"[Elevator {elevator.Id}] assigned for request: [{request.Floor}] -> [{request.DestinationFloor}] -> [{request.Direction}]");
			var floorRequests = GenerateFloorRequestList(request, elevator);

			await elevator.AddFloorRequestAsync(floorRequests, cancellationToken);
		}

		private static List<ElevatorControllerRequest> GenerateFloorRequestList(ElevatorRequest request, IElevatorController elevator) =>
					[
						new()
						{
							Floor = request.Floor,
							Direction = GetDirection(request.Floor, elevator.CurrentFloor, request.Direction)
						},
						new()
						{
							Floor = request.DestinationFloor,
							Direction = GetDirection(request.DestinationFloor, request.Floor, request.Direction)
						}
					];

		private static Direction GetDirection(int requestedFloor, int currentFloor, Direction requestedDirection)
		{
			return requestedFloor > currentFloor ?
				Direction.Up
				: requestedFloor == currentFloor ?
					requestedDirection : Direction.Down;
		}
	}
}