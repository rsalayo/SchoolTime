namespace AppBlocker
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Runtime.InteropServices;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Logging;
	using Microsoft.Win32;

	public class App
	{
		private const string RegKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options";
		private readonly IConfiguration config;
		private readonly ILogger<App> logger;

		public App(ILogger<App> logger, IConfiguration config)
		{
			this.logger = logger;
			this.config = config;
		}

		public void Activate()
		{
			this.logger.LogInformation("Activating AppBlocker");

			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return;
			}

			var toaster = Path.Combine(AppContext.BaseDirectory, "AppToaster.exe");
			var apps = this.config.GetSection("block")
				.AsEnumerable()
				.Where(x => x.Value != null)
				.Select(x => x.Value)
				.Reverse()
				.ToArray();

			foreach (var app in apps)
			{
				try
				{
					this.logger.LogInformation("  Blocking application {app}", app);
					this.logger.LogDebug("Using toaster {toaster}", toaster);

					foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(app)))
					{
						this.logger.LogDebug("  Terminating process {Name} with ID: {ID}", process.ProcessName, process.Id);
						try
						{
							process.Kill(true);
						}
						catch (Exception e)
						{
							this.logger.LogWarning(e, "Failed to terminate {process} (id)", process.ProcessName, process.Id);
						}
					}

					using var key = Registry.LocalMachine.CreateSubKey($"{RegKey}\\{app}");
					key?.SetValue("Debugger", toaster);
				}
				catch (Exception e)
				{
					this.logger.LogError(e, "Error activating AppBlocker");
				}
			}
		}

		public void Deactivate()
		{
			this.logger.LogInformation("Deactivating AppBlocker");

			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return;
			}

			var apps = this.config.GetSection("block")
				.AsEnumerable()
				.Where(x => x.Value != null)
				.Select(x => x.Value)
				.Reverse()
				.ToArray();

			foreach (var app in apps)
			{
				try
				{
					this.logger.LogInformation("  Unblocking application {app}", app);

					using var key = Registry.LocalMachine.CreateSubKey($"{RegKey}\\{app}");
					key?.DeleteValue("Debugger", false);
				}
				catch (Exception e)
				{
					this.logger.LogError(e, "Error deactivating AppBlocker");
				}
			}
		}
	}
}