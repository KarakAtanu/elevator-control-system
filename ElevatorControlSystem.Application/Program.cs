using ElevatorControlSystem.Common.Interfaces;
using ElevatorControlSystem.Common.Services;
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
services.AddSingleton<IElevatorConsoleWriterService, ElevatorConsoleWriterService>();
services.AddScoped<IElevatorController, ElevatorController>();
services.AddScoped<IRequestQueueManager, RequestQueueManager>();
services.AddTransient<IElevatorControllerFactory, ElevatorControllerFactory>();
services.AddTransient<IElevatorAssigner, ElevatorAssigner>();
services.AddTransient<IRequestValidator, RequestValidator>();
services.AddTransient<IFloorRequestQueueManager, FloorRequestQueueManager>();
services.AddTransient<IElevatorMovementService, ElevatorMovementService>();
services.AddTransient<IElevatorDoorService, ElevatorDoorService>();


var serviceProvider = services.BuildServiceProvider();
var simulator = serviceProvider.GetRequiredService<IElevatorRequestSimulator>();
var tokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (s, e) =>
{
	e.Cancel = true;
	tokenSource.Cancel();
};

Console.WriteLine("Elevator simulation started. Press Ctrl+C to exit.");

await simulator.RunAsync(tokenSource.Token);
Console.ReadLine();
