using System.Collections.Concurrent;
using ElevatorControlSystem.Common.Interfaces;

namespace ElevatorControlSystem.Common.Services
{
	/// <summary>
	/// Provides functionality for writing messages to the console with color-coded output based on elevator identifiers or
	/// message content.
	/// </summary>
	/// <remarks>This service assigns unique colors to messages associated with specific elevators and uses
	/// predefined colors for error messages or general messages. It ensures that each elevator is assigned a consistent
	/// color for easier identification in the console output.</remarks>
	public class ElevatorConsoleWriterService : IElevatorConsoleWriterService
	{
		private static readonly ConsoleColor[] ElevatorColors =
		[
			ConsoleColor.Cyan,
			ConsoleColor.Yellow,
			ConsoleColor.Green,
			ConsoleColor.Magenta,
			ConsoleColor.White,
		];

		private readonly ConcurrentDictionary<int, ConsoleColor> _elevatorColorMap = new();

		private int _nextColorIndex = 0;
		private readonly object _colorLock = new();

		public void Write(string message, int? elevatorId = null)
		{
			var containsElevator = message.Contains("[Elevator", StringComparison.OrdinalIgnoreCase);
			var containsError = message.Contains("Error in", StringComparison.OrdinalIgnoreCase);

			if (containsElevator && elevatorId.HasValue)
			{
				var color = GetOrAssignUniqueColor(elevatorId.Value);
				SetMessageColor(message, color);
			}
			else if (containsError)
			{
				SetMessageColor(message, ConsoleColor.DarkRed);
			}
			else
			{
				SetMessageColor(message, ConsoleColor.DarkBlue);
			}
		}

		private static void SetMessageColor(string message, ConsoleColor color)
		{
			var previousColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(message);
			Console.ForegroundColor = previousColor;
		}

		private ConsoleColor GetOrAssignUniqueColor(int elevatorId)
		{
			if (_elevatorColorMap.TryGetValue(elevatorId, out var color))
			{
				return color;
			}

			lock (_colorLock)
			{
				if (_elevatorColorMap.TryGetValue(elevatorId, out color))
				{
					return color;
				}

				if (_nextColorIndex >= ElevatorColors.Length)
				{
					throw new InvalidOperationException("Not enough unique colors for all elevators.");
				}

				color = ElevatorColors[_nextColorIndex];
				_elevatorColorMap[elevatorId] = color;
				_nextColorIndex++;
				return color;
			}
		}
	}
}