using ElevatorControlSystem.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

var configuration = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.Build();

services.Configure<ElevatorSettings>(configuration.GetSection(nameof(ElevatorSettings)));

Console.ReadLine();
