using ElevatorControlSystem.Common.Settings;
using ElevatorControlSystem.Infrastructure.Interfaces;
using ElevatorControlSystem.Infrastructure.Services;
using ElevatorControlSystem.Service.Interfaces;
using ElevatorControlSystem.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

var configuration = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.Build();

services.Configure<ElevatorSettings>(configuration.GetSection(nameof(ElevatorSettings)));

services.AddSingleton<IElevatorCentralProcessor, ElevatorCentralProcessor>();
services.AddSingleton<IElevatorRequestSimulator, ElevatorRequestSimulator>();
services.AddScoped<IElevatorController, ElevatorController>();
services.AddScoped<IRequestQueueManager, RequestQueueManager>();
services.AddTransient<IElevatorControllerFactory, ElevatorControllerFactory>();
services.AddTransient<IElevatorAssigner, ElevatorAssigner>();
services.AddTransient<IRequestValidator, RequestValidator>();
services.AddTransient<IFloorRequestQueueManager, FloorRequestQueueManager>();
services.AddTransient<IElevatorMovementService, ElevatorMovementService>();
services.AddTransient<IElevatorDoorService, ElevatorDoorService>();


var serviceProvider = services.BuildServiceProvider();
var requestProcessor = serviceProvider.GetRequiredService<IElevatorCentralProcessor>();
var tokenSource = new CancellationTokenSource();

requestProcessor.HandleRequest(new ElevatorControlSystem.Service.Request.ElevatorRequest
{
	DestinationFloor = 9,
	Direction = ElevatorControlSystem.Domain.Models.Enums.Direction.Up,
	Floor = 0
}, tokenSource.Token);

Thread.Sleep(4000);

requestProcessor.HandleRequest(new ElevatorControlSystem.Service.Request.ElevatorRequest
{
	DestinationFloor = 1,
	Direction = ElevatorControlSystem.Domain.Models.Enums.Direction.Down,
	Floor = 6
}, tokenSource.Token);

Thread.Sleep(3000);
requestProcessor.HandleRequest(new ElevatorControlSystem.Service.Request.ElevatorRequest
{
	DestinationFloor = 5,
	Direction = ElevatorControlSystem.Domain.Models.Enums.Direction.Down,
	Floor = 3
}, tokenSource.Token);

Thread.Sleep(10000);
requestProcessor.HandleRequest(new ElevatorControlSystem.Service.Request.ElevatorRequest
{
	DestinationFloor = 7,
	Direction = ElevatorControlSystem.Domain.Models.Enums.Direction.Up,
	Floor = 3
}, tokenSource.Token);

Thread.Sleep(10000);

requestProcessor.HandleRequest(new ElevatorControlSystem.Service.Request.ElevatorRequest
{
	DestinationFloor = 3,
	Direction = ElevatorControlSystem.Domain.Models.Enums.Direction.Down,
	Floor = 8
}, tokenSource.Token);

Console.WriteLine("Completed...");


//t1 = controller.AddFloorRequestAsync(3, tokenSource.Token);
//await t1;
//Console.WriteLine("Done...");
Console.ReadLine();
