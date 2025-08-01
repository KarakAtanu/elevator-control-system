using ElevatorControlSystem.Common.Settings;
using ElevatorControlSystem.Domain.Models;
using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Service.Interfaces;
using Microsoft.Extensions.Options;

namespace ElevatorControlSystem.Service.Services
{
	/// <summary>
	/// Provides functionality to validate elevator requests based on direction and floor constraints.
	/// </summary>
	/// <remarks>This class ensures that elevator requests meet the configured constraints, such as valid floor
	/// ranges and non-idle directions. It uses the settings provided via <see cref="ElevatorSettings"/> to determine the
	/// valid floor range.</remarks>
	public class RequestValidator : IRequestValidator
	{
		private readonly ElevatorSettings _settings;

		public RequestValidator(IOptions<ElevatorSettings> options)
		{
			_settings = options.Value;
		}

		public bool IsValid(ElevatorRequest request) =>
			request.Direction != Direction.Idle
				&& IsValidFloor(request.Floor)
				&& IsValidFloor(request.DestinationFloor)
				&& request.Floor != request.DestinationFloor;

		private bool IsValidFloor(int floor) =>
			floor >= _settings.MinFloor && floor <= _settings.MaxFloor;
	}
}
