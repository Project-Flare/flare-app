﻿using flare_csharp;
using flare_csharp.Services;

namespace ConsoleApp
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			var clientManager = new ClientManager("https://rpc.f2.project-flare.net");
			clientManager.Credentials.Username = "testing_user_0";
			clientManager.Credentials.Password = "190438934";
			clientManager.Credentials.Argon2Hash = "$argon2i$v=19$m=524288,t=3,p=4$dGVzdGluZ191c2VyXzBwcm9qZWN0LWZsYXJlLm5ldGVNVEhJaWl0NlNTcWZKdWg2UEovM3c$tHhA3AmlEH8ao3vypVV36eyzbKfuX2b5a+5OCdD0kJI";
			await clientManager.LoginToServerAsync();
			var messagingSendingService = new MessagingSendingService(new Process<MSSState, MSSCommand>(MSSState.Connected), "https://rpc.f2.project-flare.net");
			messagingSendingService.RunService(clientManager.channel);
			messagingSendingService.SendMessage(new MessagingSendingService.Message("testing_user_0", "HEELOO THERE", clientManager.Credentials.AuthToken));

			Thread.Sleep(8000000);
		}
	}
}
