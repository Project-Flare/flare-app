using System;
using Flare;


// git submodule update --init --recursive (REMEMBER TO UPDATE)
namespace Console
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("Hello, World!");
            Client client = new Client();
            
            await client.ConnectToServer();

            // TODO - challenge issuence, using Isopoh.Cryptography.Argon2 lib

            if (client.IsConnected)
                System.Console.WriteLine("Connected to server");
        }
    }
}
