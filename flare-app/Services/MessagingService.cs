using flare_csharp;
using Grpc.Net.Client;
using flare_app.Models;

namespace flare_app.Services;

internal class MessagingService
{
	public bool IsRunning { get; private set; }
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
		private set
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
	public void InitServices(string serverGrpcUrl, string serverWSUrl, Credentials credentials, GrpcChannel grpcChannel, IdentityStore identityStore)
	{
		MessageSendingService = new MessageSendingService(new Process<MSSState, MSSCommand>(MSSState.Initialized), serverGrpcUrl, credentials, grpcChannel, identityStore);
		MessageReceivingService = new MessageReceivingService(new Process<MRSState, MRSCommand>(MRSState.Initialized), serverWSUrl, credentials, identityStore);
		IsRunning = false;
	}

	public void StartServices()
	{
		if (_messageReceivingService is null || _messageSendingService is null)
			throw new InvalidOperationException("Service is not initialized");

		_messageReceivingService.StartService();
		_messageSendingService.StartService();
		// The tasks are initialized together with services
		_messageReceivingServiceTask!.Start();
		_messageSendingServiceTask!.Start();
		IsRunning = true;
	}

	public List<Message> FetchReceivedUserMessages(string senderUsername)
	{
		List<Message> messages = new List<Message>();
		if (_messageReceivingService is null)
			return messages;

		List<MessageReceivingService.InboundMessage> encryptedMessages = _messageReceivingService.FetchReceivedMessages(senderUsername);
		foreach (var encryptedMessage in encryptedMessages)
		{
			try
			{
                Message message = new Message();

                var envelope = encryptedMessage.Decrypt(_messageSendingService!.IdentityStore)!;

				message.Content = envelope.TextMessage.Content;
				message.Time = DateTime.UnixEpoch.AddMilliseconds(envelope.SenderTime);
                message.KeyPair = $"{_messageReceivingService.Credentials.Username}_{senderUsername}"; // wtf is this?
                message.Sender = senderUsername;
				message.Counter = envelope.MessageId;

				if (message.Sender != envelope.SenderUsername)
					throw new InvalidDataException();

                messages.Add(message);
            }
            catch (Exception ex)
			{
				continue;
			}
		}
		return messages;
	}
}
