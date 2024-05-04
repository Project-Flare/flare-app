using flare_csharp;
using flare_csharp.Services;
using Grpc.Net.Client;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ConsoleApp
{
	internal class Program
	{
		static void Main(string[] args)
		{
			AuthorizationService authorizationService;
			// THIS MUST BE SINGLETON AND USED THROUGHOUT THE WHOLE PROJECT
			GrpcChannel channel = GrpcChannel.ForAddress("https://rpc.f2.project-flare.net");
			Credentials credentials = new Credentials();
			credentials.Username = "testing_user_0";
			credentials.Password = "u|O(h7gNUR'}@gmi=#5k^epf+i[UWyK'~cS.1qNx";
			authorizationService = new AuthorizationService("https://rpc.f2.project-flare.net", channel, credentials); // Add credentials (username and password need to be set, to login to server and don't go through registering routine
			//authorizationService = new AuthorizationService("https://rpc.f2.project-flare.net", channel, credentials: null); // Add credentials (username and password need to be set, to login to server and don't go through registering routine

			authorizationService.StartService();

			authorizationService.LoggedInToServerEvent += (AuthorizationService.LoggedInEventArgs eventArgs) =>
			{
				if (eventArgs.LoggedInSuccessfully)
					Console.WriteLine($"User {authorizationService.Username} logged in successfully");
				else
					Console.WriteLine($"User {authorizationService.Username} login failed because: {eventArgs.LoginFailureReason}"); // don't blindly throw an error to the user directly
			};
			
			
			// Won't fire
			authorizationService.ReceivedCredentialRequirements += PrintCredentialRequirements;
			authorizationService.ReceivedCredentialRequirements += (AuthorizationService.ReceivedRequirementsEventArgs eventArgs) =>
			{
				string? username = "testing_user_0";
				string? password = "u|O(h7gNUR'}@gmi=#5k^epf+i[UWyK'~cS.1qNx";
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
