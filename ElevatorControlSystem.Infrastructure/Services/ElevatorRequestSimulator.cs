using System.Threading;
using ElevatorControlSystem.Common.Settings;
using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Infrastructure.Interfaces;
using ElevatorControlSystem.Service.Interfaces;
using ElevatorControlSystem.Service.Request;
using Microsoft.Extensions.Options;

namespace ElevatorControlSystem.Infrastructure.Services
{
	/// <summary>
	/// Simulates user-generated elevator requests and sends them to the central processor for handling.
	/// </summary>
	/// <remarks>This class generates random elevator requests within a specified range of floors and sends them to
	/// the  <see cref="IElevatorCentralProcessor"/> for processing. The simulation runs continuously until the provided 
	/// <see cref="CancellationToken"/> signals cancellation. The delay between user actions is configurable through  the
	/// <see cref="ElevatorSettings"/> options.</remarks>
	public class ElevatorRequestSimulator : IElevatorRequestSimulator
	{
		private readonly IElevatorCentralProcessor _processor;
		private readonly Random _random = new();
		private readonly int _minFloor;
		private readonly int _maxFloor;
		private readonly int _delayBetweenUserActions;

		public ElevatorRequestSimulator(IElevatorCentralProcessor processor, IOptions<ElevatorSettings> options)
		{
			_processor = processor;
			_minFloor = options.Value.MinFloor;
			_maxFloor = options.Value.MaxFloor;
			_delayBetweenUserActions = options.Value.BetweenUserActionsDelay;
		}

		public async Task RunAsync(CancellationToken token)
		{
			try
			{
				while (!token.IsCancellationRequested)
				{
					var request = GenerateRandomRequest();
					Console.WriteLine($"[User Action] {request.Floor} --> {request.DestinationFloor} [{request.Direction}]");
					_processor.HandleRequest(request);

					await Task.Delay(_delayBetweenUserActions, token);
				}
			}
			catch (OperationCanceledException)
			{
				return;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception in {nameof(ElevatorRequestSimulator)}.{nameof(RunAsync)}: {ex.Message}");
			}
		}

		private ElevatorRequest GenerateRandomRequest()
		{
			var floor = _random.Next(_minFloor, _maxFloor + 1);
			int destFloor;
			do
			{
				destFloor = _random.Next(_minFloor, _maxFloor + 1);
			} while (destFloor == floor);

			var direction = floor < destFloor ? Direction.Up : Direction.Down;

			return new ElevatorRequest
			{
				Floor = floor,
				Direction = direction,
				DestinationFloor = destFloor
			};
		}
	}
}
