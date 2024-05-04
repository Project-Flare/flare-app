using flare_csharp;
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
			//await clientManager.LoginToServerAsync();
			var authorizationService = new AuthorizationService(new Process<ASState, ASCommand>(ASState.Connecting), "https://rpc.f2.project-flare.net", clientManager.channel);
			authorizationService.StartService();
			/*var messageSendingService = new MessagingSendingService(new Process<MSSState, MSSCommand>(MSSState.Connected), "https://rpc.f2.project-flare.net", clientManager.Credentials.AuthToken, clientManager.channel);
			messageSendingService.StartService();
			messageSendingService.SendMessage(new MessagingSendingService.Message("testing_user_0", "HEELOO THERE"));
			var messageReceivingService = new MessageReceivingService(new Process<MRSState, MRSCommand>(MRSState.Connecting), "wss://ws.f2.project-flare.net/", clientManager.Credentials.AuthToken);
			messageReceivingService.StartService();*/
			Thread.Sleep(8000000);
		}
	}
}
