using ElevatorControlSystem.Common.Settings;
using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Service.Request;
using ElevatorControlSystem.Service.Services;
using Microsoft.Extensions.Options;

namespace ElevatorControlSystem.Tests.Service
{
	public class RequestValidatorTests
	{
		private RequestValidator CreateValidator(int minFloor = 1, int maxFloor = 10)
		{
			var settings = new ElevatorSettings
			{
				MinFloor = minFloor,
				MaxFloor = maxFloor
			};
			var options = Options.Create(settings);
			return new RequestValidator(options);
		}

		[Theory]
		[InlineData(Direction.Up, 1, 2, true)]
		[InlineData(Direction.Down, 2, 1, true)]
		[InlineData(Direction.Idle, 1, 2, false)]
		[InlineData(Direction.Up, 0, 2, false)] // Floor below min
		[InlineData(Direction.Up, 1, 11, false)] // Destination above max
		[InlineData(Direction.Up, 5, 5, false)] // Same floor
		public void IsValid_ReturnsExpected(Direction direction, int floor, int destination, bool expected)
		{
			// Arrange
			var validator = CreateValidator();
			var request = new ElevatorRequest
			{
				Floor = floor,
				Direction = direction,
				DestinationFloor = destination
			};

			// Act
			var result = validator.IsValid(request);

			// Assert
			Assert.Equal(expected, result);
		}

		[Theory]
		[InlineData(1, true)]
		[InlineData(10, true)]
		[InlineData(0, false)]
		[InlineData(11, false)]
		public void IsValidFloor_LogicMatchesIsValid(int floor, bool expected)
		{
			// Arrange
			var validator = CreateValidator();

			// Act
			// Since IsValidFloor is private, test its logic by creating requests that use the floor as both source and destination.
			var validRequest = new ElevatorRequest
			{
				Floor = floor,
				Direction = Direction.Up,
				DestinationFloor = floor + 1 <= 10 ? floor + 1 : floor - 1 // Ensure destination is in range
			};
			var result = validator.IsValid(validRequest);

			// Assert
			// If the floor is valid, IsValid should be true (unless destination is out of range or same as source)
			if (expected)
			{
				Assert.True(result);
			}
			else
			{
				Assert.False(result);
			}
		}
	}
}