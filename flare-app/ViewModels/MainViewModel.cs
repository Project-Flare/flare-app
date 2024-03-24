using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using flare_app.Models;
using flare_csharp;
using System.Runtime.CompilerServices;
using flare_app.Services;
using flare_app.Views;
namespace flare_app.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public AsyncRelayCommand<string> AddUserCommand { get; }
    public AsyncRelayCommand<string> RemoveUserCommand { get; }
    public AsyncRelayCommand<string> PerformMyUserSearchCommand { get; }
    public AsyncRelayCommand RefreshCommand { get; }

    private List<User> initDiscoveryList;
    bool isRefreshing;
    bool refreshFirstTime = false;

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

        initDiscoveryList = new List<User>();

        DiscoveryList = new ObservableCollection<User>();
        MyUsers = new ObservableCollection<MyContact>();

        ReloadInitDiscoveryList();
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

    void ReloadInitDiscoveryList()
    {
        initDiscoveryList.Clear();
        foreach (var usr in Client.UserDiscoveryList)
        {
            User itm = new User
            {
                UserName = usr.Username
            };

            initDiscoveryList.Add(itm);
        }
        ReloadDiscoveryList();
    }

    void ReloadDiscoveryList()
    {
        DiscoveryList.Clear();
        foreach (var user in initDiscoveryList)
        {
            DiscoveryList.Add(user);
        }
    }

    async Task AddUser(string s) // Adds form discovery list to my user list
    {
        var addThis = new MyContact { ContactUserName = s, ContactOwner = Client.Username };
        try
        {
            await LocalUserDBService.InsertContact(addThis);
            MyUsers.Add(addThis);
        }
        catch { }
        //await Refresh();
    }

    async Task RemoveUser(string s)
    {
        MyContact removeThis = MyUsers.FirstOrDefault(u => u.ContactUserName == s && u.ContactOwner == Client.Username);
        try
        {
            await LocalUserDBService.DeleteContact(removeThis);
            MyUsers.Remove(removeThis);
        }
        catch { }
        //await Refresh();
    }

    [RelayCommand]
    void PerformDiscoverySearch(string query)
    {
        if (query == string.Empty)
        {
            ReloadDiscoveryList();
        }
        else
        {
            DiscoveryList.Clear();
            foreach (var user in initDiscoveryList)
            {
                if (user.UserName.ToLower().Contains(query.ToLower()))
                {
                    DiscoveryList.Add(user);
                }
            }
        }
    }

    async Task PerformMyUserSearch(string query)
    {
        MyUsers.Clear();
        foreach (var itm in await LocalUserDBService.SearchMyContact(query, Client.Username))
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
        var list = await LocalUserDBService.GetAllMyContacts(Client.Username);
        foreach (var itm in list)
        {
            if (MyUsers.Contains(itm))
                continue;
            MyUsers.Add(itm);
        }
        IsRefreshing = false;
    }
}