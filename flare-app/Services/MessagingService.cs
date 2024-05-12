using flare_csharp;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flare_app.Services;

internal class MessagingService
{
	private static MessagingService _instance = new MessagingService();
	public static MessagingService Instance { get => _instance; }
	public MessageSendingService? MessageSendingService
	{
		get => _messageSendingService;
		set
		{
			if (value is null)
				return;
			_messageSendingService = value;
			_messageSendingServiceTask = new Task(value.RunServiceAsync);
		}
	}
	public MessageReceivingService? MessageReceivingService 
	{
		get => _messageReceivingService;
		set
		{
			if (value is null)
				return;
			_messageReceivingService = value;
			_messageReceivingServiceTask = new Task(value.RunServiceAsync);
		}
	}
	private MessageReceivingService? _messageReceivingService;
	private MessageSendingService? _messageSendingService;
	private Task? _messageSendingServiceTask;
	private Task? _messageReceivingServiceTask;
	public MessagingService() { }
	public void InitServices(string serverGrpcUrl, string serverWSUrl, string authToken, GrpcChannel grpcChannel)
	{
		Instance.MessageSendingService = new MessageSendingService(new Process<MSSState, MSSCommand>(MSSState.Initialized), serverGrpcUrl, authToken, grpcChannel);
		Instance.MessageReceivingService = new MessageReceivingService(new Process<MRSState, MRSCommand>(MRSState.Initialized), serverWSUrl, authToken);
	}
}
