using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Infrastructure.Interfaces;
using ElevatorControlSystem.Service.Interfaces;
using ElevatorControlSystem.Service.Request;

namespace ElevatorControlSystem.Infrastructure.Services
{
	public class ElevatorRequestSimulator : IElevatorRequestSimulator
	{
		private readonly IElevatorCentralProcessor _processor;
		private readonly Random _random = new();
		private readonly int _minFloor;
		private readonly int _maxFloor;

		public ElevatorRequestSimulator(IElevatorCentralProcessor processor, int minFloor, int maxFloor)
		{
			_processor = processor;
			_minFloor = minFloor;
			_maxFloor = maxFloor;
		}

		public async Task RunAsync(CancellationToken token)
		{
			var tick = 0;
			while (!token.IsCancellationRequested)
			{
				tick++;
				if (tick % 3 == 0) // every 30s, generate a random call
				{
					var request = GenerateRandomRequest();
					Console.WriteLine($"[User Action] {request}");
					_processor.HandleRequest(request, token);
				}

				await Task.Delay(10000, token); // 10 seconds per tick
			}
		}

		private ElevatorRequest GenerateRandomRequest()
		{
			var floor = _random.Next(_minFloor, _maxFloor + 1);
			Direction direction;

			if (floor == _minFloor)
			{
				direction = Direction.Up;
			}
			else if (floor == _maxFloor)
			{
				direction = Direction.Down;
			}
			else
			{
				direction = _random.Next(0, 2) == 0 ? Direction.Up : Direction.Down;
			}

			int destFloor;
			do
			{
				destFloor = _random.Next(_minFloor, _maxFloor + 1);
			} while (destFloor == floor);

			return new ElevatorRequest
			{
				Floor = floor,
				Direction = direction,
				DestinationFloor = destFloor
			};
		}
	}
}
