using flare_csharp;
using flare_csharp.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ConsoleApp
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			ClientManager clientManager;
			AuthorizationService authorizationService;
			clientManager = new ClientManager("https://rpc.f2.project-flare.net");
			clientManager.Credentials.Username = "testing_user_0";
			clientManager.Credentials.Password = "190438934";
			clientManager.Credentials.Argon2Hash = "$argon2i$v=19$m=524288,t=3,p=4$dGVzdGluZ191c2VyXzBwcm9qZWN0LWZsYXJlLm5ldGVNVEhJaWl0NlNTcWZKdWg2UEovM3c$tHhA3AmlEH8ao3vypVV36eyzbKfuX2b5a+5OCdD0kJI";
			authorizationService = new AuthorizationService("https://rpc.f2.project-flare.net", clientManager.channel);
			authorizationService.StartService();
			authorizationService.ReceivedCredentialRequirements += PrintCredentialRequirements;
			authorizationService.ReceivedCredentialRequirements += (AuthorizationService.ReceivedRequirementsEventArgs eventArgs) =>
			{
				string? username = string.Empty;
				string? password = string.Empty;
				bool setComplete = false;
				while (!setComplete)
				{
					Console.WriteLine("Username");
					if (!authorizationService.UsernameValid(username))
					{
						username = Console.ReadLine();
						Console.WriteLine($"{authorizationService.EvaluateUsername(username)}");
					}
					else
					{
						Console.WriteLine(username);
					}

					Console.WriteLine("Password");
					if (!authorizationService.PasswordValid(password))
					{
						password = Console.ReadLine();
						Console.WriteLine($"{authorizationService.EvaluatePassword(password)}");
					}
					else
					{
						Console.WriteLine(password);
					}

					if (authorizationService.UsernameValid(username) && authorizationService.PasswordValid(password))
					{
						if (authorizationService.TrySetUsername(username) && authorizationService.TrySetPassword(password))
						{
							Console.WriteLine($"Your credentials:\n{authorizationService.Username}\n{authorizationService.Password}");
							setComplete = true;
						}
					}
				}
			};

			authorizationService.RegistrationToServerEvent += (AuthorizationService.RegistrationToServerEventArgs eventArgs) =>
			{
				Console.WriteLine("Registration to server: ");
			};

			Thread.Sleep(8000000);
		}
		private static void PrintCredentialRequirements(AuthorizationService.ReceivedRequirementsEventArgs eventArgs)
		{
			Console.WriteLine(
				$"Credential requirements:\n"
					+"\tPassword:\n"
						+ $"\t\tMax password length: {eventArgs.CredentialRequirements.ValidPasswordRules.MaxLength}\n"
						+ $"\t\tPassword encoding: {eventArgs.CredentialRequirements.ValidPasswordRules.Encoding}\n"
						+ $"\t\tBit entropy: {eventArgs.CredentialRequirements.ValidPasswordRules.Encoding}\n"
					+ "\tUsername:\n"
						+ $"\t\tMinimum length: {eventArgs.CredentialRequirements.ValidUsernameRules.MinLength}\n"
						+ $"\t\tMaximum length: {eventArgs.CredentialRequirements.ValidUsernameRules.MaxLength}\n"
						+ $"\t\tEncoding: {eventArgs.CredentialRequirements.ValidUsernameRules.Encoding}\n"
						+ $"\t\tFormat type: {eventArgs.CredentialRequirements.ValidUsernameRules.StringFormatType}\n");
			// the requirements are received and setting the password and username are now allowed
		}
	}
}
