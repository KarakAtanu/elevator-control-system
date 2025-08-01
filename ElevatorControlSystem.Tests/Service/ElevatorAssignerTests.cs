using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Service.Interfaces;
using ElevatorControlSystem.Service.Request;
using ElevatorControlSystem.Service.Services;
using Moq;

namespace ElevatorControlSystem.Tests.Service
{
	public class ElevatorAssignerTests
	{
		private static Mock<IElevatorController> CreateElevator(int id, int floor, Direction direction)
		{
			var mock = new Mock<IElevatorController>();
			mock.SetupGet(e => e.Id).Returns(id);
			mock.SetupGet(e => e.CurrentFloor).Returns(floor);
			mock.SetupGet(e => e.Direction).Returns(direction);
			mock.SetupGet(e => e.IsIdle).Returns(direction == Direction.Idle);
			return mock;
		}

		[Fact]
		public void Assign_ReturnsUpwardElevator_WhenRequestIsUp_AndElevatorIsMovingUp()
		{
			// Arrange
			var request = new ElevatorRequest { Floor = 3, Direction = Direction.Up };
			var elevator1 = CreateElevator(1, 1, Direction.Up).Object;
			var elevator2 = CreateElevator(2, 5, Direction.Down).Object;
			var assigner = new ElevatorAssigner();

			// Act
			var result = assigner.Assign(request, new[] { elevator1, elevator2 });

			// Assert
			Assert.Equal(elevator1, result);
		}

		[Fact]
		public void Assign_ReturnsIdleElevator_WhenNoElevatorMovingUp()
		{
			// Arrange
			var request = new ElevatorRequest { Floor = 3, Direction = Direction.Up };
			var elevator1 = CreateElevator(1, 1, Direction.Down).Object;
			var elevator2 = CreateElevator(2, 2, Direction.Idle).Object;
			var assigner = new ElevatorAssigner();

			// Act
			var result = assigner.Assign(request, new[] { elevator1, elevator2 });

			// Assert
			Assert.Equal(elevator2, result);
		}

		[Fact]
		public void Assign_ReturnsDownwardElevator_WhenRequestIsDown_AndElevatorIsMovingDown()
		{
			// Arrange
			var request = new ElevatorRequest { Floor = 2, Direction = Direction.Down };
			var elevator1 = CreateElevator(1, 5, Direction.Down).Object;
			var elevator2 = CreateElevator(2, 1, Direction.Up).Object;
			var assigner = new ElevatorAssigner();

			// Act
			var result = assigner.Assign(request, new[] { elevator1, elevator2 });

			// Assert
			Assert.Equal(elevator1, result);
		}

		[Fact]
		public void Assign_ReturnsNull_WhenNoElevatorAvailable()
		{
			// Arrange
			var request = new ElevatorRequest { Floor = 2, Direction = Direction.Down };
			var assigner = new ElevatorAssigner();

			// Act
			var result = assigner.Assign(request, Array.Empty<IElevatorController>());

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void FindUpwardElevators_ReturnsElevatorsMovingUp_BelowOrAtRequestFloor()
		{
			// Arrange
			var request = new ElevatorRequest { Floor = 5, Direction = Direction.Up };
			var elevator1 = CreateElevator(1, 3, Direction.Up).Object;
			var elevator2 = CreateElevator(2, 6, Direction.Up).Object;
			var elevator3 = CreateElevator(3, 4, Direction.Up).Object;

			// Act
			var result = typeof(ElevatorAssigner)
				.GetMethod("FindUpwardElevators", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
				.Invoke(null, new object[] { request, new[] { elevator1, elevator2, elevator3 } }) as IEnumerable<IElevatorController>;

			// Assert
			Assert.Contains(elevator1, result);
			Assert.Contains(elevator3, result);
			Assert.DoesNotContain(elevator2, result);
		}

		[Fact]
		public void FindUpwardElevators_ReturnsIdleElevators_WhenNoUpwardCandidates()
		{
			// Arrange
			var request = new ElevatorRequest { Floor = 5, Direction = Direction.Up };
			var elevator1 = CreateElevator(1, 3, Direction.Down).Object;
			var elevator2 = CreateElevator(2, 6, Direction.Down).Object;
			var elevator3 = CreateElevator(3, 4, Direction.Idle).Object;

			// Act
			var result = typeof(ElevatorAssigner)
				.GetMethod("FindUpwardElevators", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
				.Invoke(null, new object[] { request, new[] { elevator1, elevator2, elevator3 } }) as IEnumerable<IElevatorController>;

			// Assert
			Assert.Contains(elevator3, result);
			Assert.DoesNotContain(elevator1, result);
			Assert.DoesNotContain(elevator2, result);
		}

		[Fact]
		public void FindDownwardElevators_ReturnsElevatorsMovingDown_AboveOrAtRequestFloor()
		{
			// Arrange
			var request = new ElevatorRequest { Floor = 2, Direction = Direction.Down };
			var elevator1 = CreateElevator(1, 5, Direction.Down).Object;
			var elevator2 = CreateElevator(2, 1, Direction.Down).Object;
			var elevator3 = CreateElevator(3, 3, Direction.Down).Object;

			// Act
			var result = typeof(ElevatorAssigner)
				.GetMethod("FindDownwardElevators", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
				.Invoke(null, new object[] { request, new[] { elevator1, elevator2, elevator3 } }) as IEnumerable<IElevatorController>;

			// Assert
			Assert.Contains(elevator1, result);
			Assert.Contains(elevator3, result);
			Assert.DoesNotContain(elevator2, result);
		}

		[Fact]
		public void FindDownwardElevators_ReturnsIdleElevators_WhenNoDownwardCandidates()
		{
			// Arrange
			var request = new ElevatorRequest { Floor = 2, Direction = Direction.Down };
			var elevator1 = CreateElevator(1, 5, Direction.Up).Object;
			var elevator2 = CreateElevator(2, 1, Direction.Up).Object;
			var elevator3 = CreateElevator(3, 3, Direction.Idle).Object;

			// Act
			var result = typeof(ElevatorAssigner)
				.GetMethod("FindDownwardElevators", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
				.Invoke(null, new object[] { request, new[] { elevator1, elevator2, elevator3 } }) as IEnumerable<IElevatorController>;

			// Assert
			Assert.Contains(elevator3, result);
			Assert.DoesNotContain(elevator1, result);
			Assert.DoesNotContain(elevator2, result);
		}

		[Fact]
		public void GetIdleElevators_ReturnsIdleElevators_OrderedByDistance()
		{
			// Arrange
			var request = new ElevatorRequest { Floor = 5, Direction = Direction.Up };
			var elevator1 = CreateElevator(1, 3, Direction.Idle).Object;
			var elevator2 = CreateElevator(2, 7, Direction.Idle).Object;
			var elevator3 = CreateElevator(3, 5, Direction.Idle).Object;

			// Act
			var result = typeof(ElevatorAssigner)
				.GetMethod("GetIdleElevators", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
				.Invoke(null, new object[] { request, new[] { elevator1, elevator2, elevator3 } }) as IEnumerable<IElevatorController>;

			var ordered = result.ToList();

			// Assert
			Assert.Equal(elevator3, ordered[0]); // closest
			Assert.Equal(elevator1, ordered[1]);
			Assert.Equal(elevator2, ordered[2]);
		}
	}
}