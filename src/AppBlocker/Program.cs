namespace AppBlocker
{
	using System;
	using System.Threading.Tasks;
	using CommandLine;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Serilog;

	internal class Program
	{
		public static IConfiguration Configuration;

		public class Options
		{
			[Option("activate", Required = false, HelpText = "Activate AppBlocker for configurerd applications")]
			public bool Activate { get; set; }

			[Option("deactivate", Required = false, HelpText = "Deactivate AppBlocker for configurerd applications")]
			public bool Deactivate { get; set; }
		}

		private static void Main(string[] args)
		{
			// Initialize generic IConfigurationRoot
			Configuration = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddYamlFile("Configurations/Main.yml", true)
				.Build();

			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console()
				.WriteTo.File("Log/SchoolTime.AppBlocker-.log",
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message}{NewLine}{Exception}",
					rollingInterval: RollingInterval.Day)
				.CreateLogger();

			// Initialize service collection
			var services = new ServiceCollection();
			ConfigureServices(services);
			var serviceProvider = services.BuildServiceProvider();

			Parser.Default.ParseArguments<Options>(args)
				.WithParsed(o =>
				{
					if (o.Activate)
					{
						serviceProvider.GetService<App>().Activate();
					}
					else if (o.Deactivate)
					{
						serviceProvider.GetService<App>().Deactivate();
					}
				});
		}

		private static void ConfigureServices(IServiceCollection services)
		{
			// Add access to generic IConfiguration
			services.AddSingleton(Configuration);

			// Add logging service
			services.AddLogging(configure => configure.AddSerilog());

			// Add app
			services.AddTransient<App>();
		}
	}
}