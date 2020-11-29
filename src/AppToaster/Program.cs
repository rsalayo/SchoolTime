namespace AppToaster
{
	using System;
	using System.Text;
	using Windows.UI.Notifications;

	internal class Program
	{
		private static void Main(string[] args)
		{
			const string title = "SchoolTime by Rome Salayo";
			const string msg = "This application has been blocked during school hours, please try again later.";

			const ToastTemplateType template = ToastTemplateType.ToastText02;
			var toastXml = ToastNotificationManager.GetTemplateContent(template);
			var toastTextElements = toastXml.GetElementsByTagName("text");
			toastTextElements[0].AppendChild(toastXml.CreateTextNode(title));
			toastTextElements[1].AppendChild(toastXml.CreateTextNode(msg));

			ToastNotificationManager
				.CreateToastNotifier("SchoolTime")
				.Show(new ToastNotification(toastXml));


			var sb = new StringBuilder();
			sb.AppendLine(@"  _________      .__                  ._____________.__                ");
			sb.AppendLine(@" /   _____/ ____ |  |__   ____   ____ |  \__    ___/|__| _____   ____  ");
			sb.AppendLine(@" \_____  \_/ ___\|  |  \ /  _ \ /  _ \|  | |    |   |  |/     \_/ __ \ ");
			sb.AppendLine(@" /        \  \___|   Y  (  <_> |  <_> )  |_|    |   |  |  Y Y  \  ___/ ");
			sb.AppendLine(@"/_______  /\___  >___|  /\____/ \____/|____/____|   |__|__|_|  /\___  >");
			sb.AppendLine(@"        \/     \/     \/                                     \/     \/ ");
			sb.AppendLine(@"                                                        By Romeo Salayo");
			Console.Title = title;
			Console.WriteLine(sb.ToString());
			Console.WriteLine();
			Console.WriteLine(msg);
			Console.WriteLine();
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}
	}
}