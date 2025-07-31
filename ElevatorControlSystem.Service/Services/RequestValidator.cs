using ElevatorControlSystem.Common.Settings;
using ElevatorControlSystem.Domain.Models.Enums;
using ElevatorControlSystem.Service.Interfaces;
using ElevatorControlSystem.Service.Request;
using Microsoft.Extensions.Options;

namespace ElevatorControlSystem.Service.Services
{
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
				&& IsValidFloor(request.DestinationFloor);

		private bool IsValidFloor(int floor) =>
			floor >= _settings.MinFloor && floor <= _settings.MaxFloor;
	}
}
