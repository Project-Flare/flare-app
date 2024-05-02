using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using flare_app.Models;
using System.Runtime.CompilerServices;
using flare_app.Services;
using flare_app.Views;
using CommunityToolkit.Maui.Core.Views;
using System.Windows.Input;
namespace flare_app.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public AsyncRelayCommand<string> AddUserCommand { get; }
    public AsyncRelayCommand<string> RemoveUserCommand { get; }
    public AsyncRelayCommand<string> PerformMyUserSearchCommand { get; }
    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand<string> AddUserOnPopCommand { get; }
    public AsyncRelayCommand<string> ChatDetailCommand { get; }

    private List<User> initDiscoveryList;
    bool isRefreshing;
    bool refreshFirstTime = false;

    [ObservableProperty]
    string text;

    [ObservableProperty]
    User user;

    //private bool? _isHolding;
    private string? _contactUserNameToRemove;
    private bool? _userfound = false;

    public bool IsRefreshing
    {
        get { return isRefreshing; }
        set { isRefreshing = value; OnPropertyChanged(); }
    }

    // List should be taken from client class
    public MainViewModel()
    {
        AddUserCommand = new AsyncRelayCommand<string>(AddUser);
        RemoveUserCommand = new AsyncRelayCommand<string>(RemoveUser);
        PerformMyUserSearchCommand = new AsyncRelayCommand<string>(PerformMyUserSearch);
        RefreshCommand = new AsyncRelayCommand(Refresh);
        AddUserOnPopCommand = new AsyncRelayCommand<string>(AddUserOnPop);
        ChatDetailCommand = new AsyncRelayCommand<string>(ChatDetail);

        initDiscoveryList =
        [
            new User { UserName = "TempUser1", LastMessage="Labas", ProfilePicture="picture1.jpg", id=123456789 },
            new User { UserName = "TempUser2", LastMessage="Testas tekstas", ProfilePicture="picture2.jpg", id=147852369  },
        ];
 
        DiscoveryList = new ObservableCollection<User>();

        MyUsers = new ObservableCollection<MyContact>();

        // ReloadInitDiscoveryList();
        if (!refreshFirstTime)
        {
            refreshFirstTime = true;
            Task.Run(Refresh);
        }
        //ReloadInitMyUsers();
    }

    [ObservableProperty]
    ObservableCollection<MyContact> myUsers; // Observable.


    [ObservableProperty]
    ObservableCollection<User> discoveryList; // Observable.




    async Task AddUser(string? s) // Adds form discovery list to my user list
    {
        var addThis = new MyContact { ContactUserName = s, ContactOwner = "TempUser1" };
        try
        {
            await LocalUserDBService.InsertContact(addThis);
            MyUsers.Add(addThis);
        }
        catch { }
        //await Refresh();
    }

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


    async Task PerformMyUserSearch(string? query)
    {
        MyUsers.Clear();
        foreach (var itm in await LocalUserDBService.SearchMyContact(query, "TempUser1"))
        {
            MyUsers.Add(itm);
        }
    }

    /*void ResetBackGroundColor(ObservableCollection<User> list)
    {
        for (int i = 0; i < list.Count(); i++)
        {
            if (i % 2 == 0)
            {
                //{AppThemeBinding Light={StaticResource #D9D9D9} Dark={StaticResource #686868}}
                list[i].BackGroundColor = "DarkTile";
            }
            else
            {
                list[i].BackGroundColor = "LightStyle";
            }
        }
    }*/

    async Task Refresh()
    {
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
    async Task AddUserOnPop(string? s) // Adds form discovery list to my user list, when app restarts random written usernames are implemented aswell
    {
        var addThis = new MyContact { ContactUserName = s, ContactOwner = "TempUser1" };
        try
        {
            foreach (var user in initDiscoveryList)
            {
                if(user.UserName.Equals(s))
                {
                    await LocalUserDBService.InsertContact(addThis);
                    MyUsers.Add(addThis);
                }
            }
        }
        catch { }

        await Refresh();
    }

    async Task ChatDetail(string? s)
    {
        await Shell.Current.GoToAsync($"{nameof(ChatPage)}?Username={s}");
    }
}