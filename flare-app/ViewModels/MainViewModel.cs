using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using flare_app.Models;
using flare_app.Services;
using Grpc.Net.Client;
using flare_csharp;

namespace flare_app.ViewModels;

public partial class MainViewModel : ObservableObject
{
	readonly string _serverUrl = "https://rpc.f2.project-flare.net";
	public AsyncRelayCommand<string> AddUserCommand { get; }
    public AsyncRelayCommand<string> RemoveUserCommand { get; }
    public AsyncRelayCommand<string> PerformMyUserSearchCommand { get; }
    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand<string> AddUserOnPopCommand { get; }
    public AsyncRelayCommand<string> ChatDetailCommand { get; }

    private List<User>? initDiscoveryList;
    bool isRefreshing;
    bool refreshFirstTime = false;
    private UserService _userService;

    [ObservableProperty]
    string? text;

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
        AddUserCommand = new AsyncRelayCommand<string>(AddUser);
        RemoveUserCommand = new AsyncRelayCommand<string>(RemoveUser);
        PerformMyUserSearchCommand = new AsyncRelayCommand<string>(PerformMyUserSearch);
        RefreshCommand = new AsyncRelayCommand(Refresh);
        AddUserOnPopCommand = new AsyncRelayCommand<string>(AddUserOnPop);
        ChatDetailCommand = new AsyncRelayCommand<string>(ChatDetail);

        DiscoveryList = new ObservableCollection<User>();

        MyUsers = new ObservableCollection<MyContact>();

        if (!refreshFirstTime)
        {
            refreshFirstTime = true;
            Task.Run(Refresh);
        }

        _userService = new UserService(GrpcChannel.ForAddress(_serverUrl));
    }

    [ObservableProperty]
    ObservableCollection<MyContact> myUsers; 


    [ObservableProperty]
    ObservableCollection<User> discoveryList;



    /// <summary>
    /// Adds user into local database and into 'MyUsers' list.
    /// </summary>
    async Task AddUser(string? s)
    {
        // 'ContactOwner' should be the logged in user.
        var addThis = new MyContact { ContactUserName = s, ContactOwner = "TempUser1" };
        try
        {
            await LocalUserDBService.InsertContact(addThis);
            MyUsers.Add(addThis);
        }
        catch { }
    }

    /// <summary>
    /// Removes my contact from 'MyUsers' list and deletes from local database.
    /// </summary>
    async Task RemoveUser(string? s)
    {
        MyContact? removeThis = MyUsers.FirstOrDefault(u => u.ContactUserName == s && u.ContactOwner == "TempUser1");
        try
        {
            await LocalUserDBService.DeleteContact(removeThis);
            MyUsers.Remove(removeThis);
        }
        catch { }
        //await Refresh();
    }

    /// <summary>
    /// Searches for my contact from local database and loads it into 'MyUsers' list.
    /// </summary>
    async Task PerformMyUserSearch(string? query)
    {
        MyUsers.Clear();
        foreach (var itm in await LocalUserDBService.SearchMyContact(query, "TempUser1"))
        {
            MyUsers.Add(itm);
        }
    }

    /// <summary>
    /// Refreshes 'MyUsers' list with data from local database.
    /// </summary>
    async Task Refresh()
    {
        //[TODO]: implement user discovery page
        IsRefreshing = true;
        MyUsers.Clear();
        var list = await LocalUserDBService.GetAllMyContacts("TempUser1");
        foreach (var itm in list)
        {
            if (MyUsers.Contains(itm))
                continue;
            MyUsers.Add(itm);
        }
        IsRefreshing = false;
    }

    /// <summary>
    /// Adds user to local database and to 'MyUsers' list.
    /// </summary>
    async Task AddUserOnPop(string? username)
    {
        //HERE
        if (username is null)
            return;

        Flare.V1.User? foundUser = await _userService.GetUser(username);

        if (foundUser is null)
        {
            //[DEV_NOTES]: user not found display popup message
            return;
        }

		// [DEV_NOTES] TempUser1 should be changed to the user that is signed in
		var newContact = new MyContact { ContactUserName = foundUser.Username + " " + foundUser.IdPubKey, ContactOwner = "TempUser1" };
        try
        {
            // [DEV_NOTES]: check if the user is added
            await LocalUserDBService.InsertContact(newContact);
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
    async Task ChatDetail(string s)
    {
        await Shell.Current.GoToAsync($"//MainPage//ChatPage?Username={s}", animate: true);
    }
}