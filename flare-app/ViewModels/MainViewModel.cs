using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using flare_app.Models;
using flare_app.Services;
using Grpc.Net.Client;
using flare_csharp;
using Org.BouncyCastle.Crypto.Parameters;

namespace flare_app.ViewModels;

public partial class MainViewModel : ObservableObject
{
	readonly string _serverUrl = "https://rpc.f2.project-flare.net";
	//public AsyncRelayCommand<string> AddUserCommand { get; }
    public AsyncRelayCommand<string> RemoveUserCommand { get; }
    public AsyncRelayCommand<string> PerformMyUserSearchCommand { get; }
    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand<string> AddUserOnPopCommand { get; }
    public AsyncRelayCommand<string> ChatDetailCommand { get; }

    bool isRefreshing;
    private UserService _userService;
    //private string LocalUsername = MessagingService.Instance.MessageSendingService!.Credentials.Username;
    string? LocalUsername;

    [ObservableProperty]
    string? text;

    [ObservableProperty]
    string _errorMessage;

    [ObservableProperty]
    User? user;


    public bool IsRefreshing
    {
        get { return isRefreshing; }
        set { isRefreshing = value; OnPropertyChanged(); }
    }

    // List should be taken from client class
    public MainViewModel()
    {
        // Relay commands that can be called from XAML or page's class.
        //AddUserCommand = new AsyncRelayCommand<string>(AddUser);
        RemoveUserCommand = new AsyncRelayCommand<string>(RemoveUser);
        PerformMyUserSearchCommand = new AsyncRelayCommand<string>(PerformMyUserSearch);
        RefreshCommand = new AsyncRelayCommand(Refresh);
        AddUserOnPopCommand = new AsyncRelayCommand<string>(AddUserOnPop);
        ChatDetailCommand = new AsyncRelayCommand<string>(ChatDetail);

        MyUsers = new ObservableCollection<MyContact>();

		_userService = new UserService(GrpcChannel.ForAddress(_serverUrl));
    }

    [ObservableProperty]
    ObservableCollection<MyContact> myUsers; 


    /// <summary>
    /// Removes my contact from 'MyUsers' list and deletes from local database.
    /// </summary>
    async Task RemoveUser(string? s)
    {
        if (s is null)
            return;

		MyContact? removeThis = MyUsers.FirstOrDefault(u => u.ContactUserName == s && u.ContactOwner == $"{LocalUsername}");
        if (removeThis is not null)
        {
            try
            {
                await MessagesDBService.DeleteUserMessages($"{LocalUsername}_{removeThis.ContactUserName}");
                await LocalUserDBService.DeleteContact(removeThis);
                MyUsers.Remove(removeThis);
            }
            catch { }
        }
        await Refresh();
    }

    /// <summary>
    /// Searches for my contact from local database and loads it into 'MyUsers' list.
    /// </summary>
    async Task PerformMyUserSearch(string? query)
    {
        MyUsers.Clear();
        var list = await LocalUserDBService.SearchMyContact(query, LocalUsername);

        if (list is null)
            return;

		foreach (var itm in list)
        {
            MyUsers.Add(itm);
        }
    }

    /// <summary>
    /// Refreshes 'MyUsers' list with data from local database.
    /// </summary>
    async Task Refresh()
    {

		LocalUsername = (await LocalUserDBService.GetAllLocalUsers()).First().LocalUserName!;
		//[TODO]: implement user discovery page
		IsRefreshing = true;
        MyUsers.Clear();
        MessagingService.Instance.MessageSendingService!.IdentityStore.Contacts.Clear();
        var list = await LocalUserDBService.GetAllMyContacts(LocalUsername);
        foreach (var itm in list)
        {
            itm.ContactUserName = itm.ContactUserName!;
            if (MyUsers.Contains(itm))
                continue;
			MyUsers.Add(itm);
            string username = itm.ContactUserName;
            string publicKey = itm.PublicKey!;
            Identity identity = new Identity();
            identity.Username = username;
            try
            {
                identity.PublicKey = (ECPublicKeyParameters)Crypto.GetPublicKeyFromDer(publicKey);
                MessagingService.Instance.MessageSendingService!.IdentityStore.Contacts.Add(username, identity);
			}
            catch (Exception ex)
            {
                // TODO: 
            }
        }

        IsRefreshing = false;
    }

    /// <summary>
    /// Adds user to local database and to 'MyUsers' list.
    /// </summary>
    async Task AddUserOnPop(string? username)
    {
        //HERE
        if (string.IsNullOrEmpty(username))
            return;

        Flare.V1.User? foundUser = await _userService.GetUser(username);
        if (foundUser is null)
            return;

        Identity identity = new Identity();
        identity.Username = foundUser.Username;
        try
        {
            identity.PublicKey = (ECPublicKeyParameters)Crypto.GetPublicKeyFromDer(foundUser.IdPubKey);
        }
        catch (FormatException)
        {
            //[DEV_NOTES]: user not found display popup message
            return;
		}

		MessagingService.Instance.MessageSendingService!.IdentityStore.Contacts.Add(foundUser.Username, identity);

        if (foundUser is null)
        {
            //[DEV_NOTES]: user not found display popup message
            return;
        }

		// [DEV_NOTES] TempUser1 should be changed to the user that is signed in
		var newContact = new MyContact { ContactUserName = foundUser.Username, ContactOwner = LocalUsername, PublicKey = foundUser.IdPubKey };
        try
        {
            // [DEV_NOTES]: check if the user is added
            await LocalUserDBService.InsertContact(newContact);
            newContact.ContactUserName = newContact.ContactUserName;
            MyUsers.Add(newContact);
        }
        catch 
        {
            // [DEV_NOTES]: resolve fucking error god damn it
        }

        await Refresh();
    }

    /// <summary>
    /// Navigates to chat page and passes username parameter within URI.
    /// NOTE: every time this method is called, new chat page is created.
    /// </summary>
    async Task ChatDetail(string? s)
    {
        await Shell.Current.GoToAsync($"//MainPage//ChatPage?Username={s!}", animate: true);
    }
}