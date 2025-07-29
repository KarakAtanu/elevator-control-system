using ElevatorControlSystem.Domain.Models;

namespace ElevatorControlSystem.Tests.Domain
{
	public class ElevatorTests
	{
		[Fact]
		public void AddFloorRequest_ValidFloor_AddsToDestinations()
		{
			//Arrange
			var elevator = new Elevator(id: 1, minFloor: 0, maxFloor: 10);

			//Act
			elevator.AddFloorRequest(5);

			//Assert
			Assert.Contains(5, elevator.Destinations);
		}

		[Theory]
		[InlineData(-1)] // Below minFloor
		[InlineData(11)] // Above maxFloor
		public void AddFloorRequest_InvalidFloor_ThrowsArgumentOutOfRangeException(int requestedFloor)
		{
			//Arrange
			var elevator = new Elevator(id: 1, minFloor: 0, maxFloor: 10);

			//Act and Assert
			Assert.Throws<ArgumentOutOfRangeException>(() => elevator.AddFloorRequest(requestedFloor));
		}

		[Fact]
		public void AddFloorRequest_DuplicateFloor_DoesNotAddTwice()
		{
			//Arrange
			var elevator = new Elevator(id: 1, minFloor: 0, maxFloor: 10);

			//Act
			elevator.AddFloorRequest(3);
			elevator.AddFloorRequest(3);

			//Assert
			Assert.Equal(1, elevator.Destinations.Count(f => f == 3));
		}

		[Fact]
		public void AddFloorRequest_MinFloor_AddsToDestinations()
		{
			//Arrange
			var elevator = new Elevator(id: 1, minFloor: 0, maxFloor: 10);

			//Act
			elevator.AddFloorRequest(0);

			//Assert
			Assert.Contains(0, elevator.Destinations);
		}

		[Fact]
		public void AddFloorRequest_MaxFloor_AddsToDestinations()
		{
			//Arrange
			var elevator = new Elevator(id: 1, minFloor: 0, maxFloor: 10);

			//Act
			elevator.AddFloorRequest(10);

			//Assert
			Assert.Contains(10, elevator.Destinations);
		}

		[Fact]
		public void AddFloorRequest_ThreadSafety_MultipleThreads_NoDuplicates()
		{
			//Arrange
			var elevator = new Elevator(id: 1, minFloor: 0, maxFloor: 10);
			var floors = new[] { 2, 4, 6, 8, 2, 4, 6, 8 };
			var threads = new List<Thread>();

			//Act
			foreach (var floor in floors)
			{
				threads.Add(new Thread(() => elevator.AddFloorRequest(floor)));
			}

			threads.ForEach(t => t.Start());
			threads.ForEach(t => t.Join());

			//Assert
			Assert.Equal(4, elevator.Destinations.Count);
			Assert.Contains(2, elevator.Destinations);
			Assert.Contains(4, elevator.Destinations);
			Assert.Contains(6, elevator.Destinations);
			Assert.Contains(8, elevator.Destinations);
		}
	}
}