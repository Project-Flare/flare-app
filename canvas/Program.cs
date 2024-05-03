using Flare.V1;
using flare_csharp;
using Grpc.Core;
using Grpc.Net.Client;
using System.Net;

namespace canvas
{
	public class Program1
	{

		public async Task Run()
		{
			/*var serverUri = new Uri("https://rpc.f2.project-flare.net");
			var grpcChannel = GrpcChannel.ForAddress(serverUri);
			var authClient = new Auth.AuthClient(grpcChannel);
			await grpcChannel.ConnectAsync(CancellationToken.None);
			//var resp = await authClient.GetCredentialRequirementsAsync(new RequirementsRequest { });
			var credentials = new ClientCredentials(1024 * 64, 3);
			var process = new Process<ApplyToRegisterState>(ApplyToRegisterState.FetchingRequirements);
			process.ProcessThread = new Thread(() => ApplyToRegister(grpcChannel, new Auth.AuthClient(grpcChannel), process));
			process.ProcessThread.Start();
			Thread.Sleep(6000);
			Console.WriteLine("[REGISTER_PROC_STATE]: " + process.CurrentState);
		}*/
	}
	public class Program
	{
		public static async Task Main(string[] args)
		{
			Program1 pr = new Program1();
			await pr.Run();
			/*var clientManager = new ClientManager("https://rpc.f2.project-flare.net");
			var req = await clientManager.GetCredentialRequirementsAsync();*/
		}
	}
}	
