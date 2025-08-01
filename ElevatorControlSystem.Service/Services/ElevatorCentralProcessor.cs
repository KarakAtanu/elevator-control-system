using ElevatorControlSystem.Common.Interfaces;
using ElevatorControlSystem.Common.Settings;
using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Service.Interfaces;
using ElevatorControlSystem.Service.Request;
using Microsoft.Extensions.Options;

namespace ElevatorControlSystem.Service.Services
{
	/// <summary>
	/// Coordinates and manages the operation of multiple elevators, including handling requests, assigning elevators, and
	/// processing queued tasks.
	/// </summary>
	/// <remarks>This class serves as the central processor for an elevator system. It validates incoming requests,
	/// assigns elevators based on the current state of the system, and processes requests asynchronously. The processor
	/// uses a background task to handle queued requests and ensures that multiple requests can be processed concurrently
	/// up to a predefined limit.</remarks>
	public class ElevatorCentralProcessor : IElevatorCentralProcessor
	{
		private const int NUMBER_OF_CONCURRENT_TASKS = 5;
		
		private readonly CancellationTokenSource _cts = new();
		private readonly List<Task> _workers = [];
		private readonly List<IElevatorController> _elevatorControllers;
		private readonly ElevatorSettings _settings;
		private readonly IRequestValidator _validator;
		private readonly IElevatorAssigner _elevatorAssigner;
		private readonly IRequestQueueManager _queueManager;
		private readonly IElevatorConsoleWriterService _consoleWriterService;
		private readonly SemaphoreSlim _semaphore = new(NUMBER_OF_CONCURRENT_TASKS);
		public ElevatorCentralProcessor(IOptions<ElevatorSettings> options,
										 IElevatorControllerFactory controllerFactory,
										 IRequestValidator validator,
										 IElevatorAssigner elevatorAssigner,
										 IRequestQueueManager queueManager,
										 IElevatorConsoleWriterService consoleWriterService)
		{
			_settings = options.Value;
			_validator = validator;
			_elevatorAssigner = elevatorAssigner;
			_queueManager = queueManager;
			_consoleWriterService = consoleWriterService;

			_elevatorControllers = controllerFactory.CreateControllers(_settings);
			_workers.Add(Task.Run(() => ProcessQueueAsync(_cts.Token)));
		}

		public void HandleRequest(ElevatorRequest request)
		{
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

			_consoleWriterService.Write($"[Elevator {elevator.Id}] Assigned for Request: [{request.Floor}] -> [{request.DestinationFloor}] -> [{request.Direction}]", elevator.Id);
			
			var internalRequests = GenerateInternalRequestsForController(request, elevator);
			await elevator.AddFloorRequestAsync(internalRequests, cancellationToken);
		}

		private static List<ElevatorControllerRequest> GenerateInternalRequestsForController(ElevatorRequest request, IElevatorController elevator) =>
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