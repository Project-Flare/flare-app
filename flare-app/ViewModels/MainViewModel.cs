using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using flare_app.Models;
using flare_csharp;
namespace flare_app.ViewModels;

public partial class MainViewModel : ObservableObject
{

    private List<User> initMyUsers;
    private List<User> initDiscoveryList;

    // List should be taken from client class
    public MainViewModel()
    {
        initDiscoveryList = new List<User>();
        initMyUsers = new List<User> // sample list
        {
            new User
            {
                UserName = "Petras",
            },
            new User
            {
                UserName = "Jonas",
            },
            new User
            {
                UserName = "Baryga",
            },
            new User
            {
                UserName = "Gola",
            },
            new User
            {
                UserName = "UwU",
            },
            new User
            {
                UserName = "Opel_Zafira_2TDI",
            }
        };

        ReloadInitDiscoveryList();

        DiscoveryList = new ObservableCollection<User>();
        MyUsers = new ObservableCollection<User>();

        //ReloadDiscoveryList();
        ReloadMyUsers();
    }

    [ObservableProperty]
    ObservableCollection<User> myUsers; // Observable.
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
        foreach (var usr in initDiscoveryList)
        {
            DiscoveryList.Add(usr);
        }

        //ResetBackGroundColor(DiscoveryList);
    }

    void ReloadMyUsers()
    {
        foreach (var usr in initMyUsers)
        {
            MyUsers.Add(usr);
        }

        //ResetBackGroundColor(MyUsers);
    }

    [RelayCommand]
    void AddUser(string s) // Adds form discovery list to my user list
    {
        User? removeThis = DiscoveryList.FirstOrDefault(u => u.UserName == s);

        if (removeThis != null)
        {
            //DiscoveryList.Remove(removeThis); // to be decided
            MyUsers.Add(removeThis);
            //ResetBackGroundColor(NewUsers);
            //ResetBackGroundColor(DiscoveryList);
        }
    }

    [RelayCommand]
    void RemoveUser(string s)
    {
        User? removeThis = MyUsers.FirstOrDefault(u => u.UserName == s);

        if (removeThis != null)
        {
            MyUsers.Remove(removeThis);
            //DiscoveryList.Add(removeThis); // to be decided
            //ResetBackGroundColor(MyUsers);
            //ResetBackGroundColor(DiscoveryList);
        }
    }

    [RelayCommand]
    void PerformDiscoverySearch(string query)
    {
        if (query == string.Empty)
        {
            DiscoveryList.Clear();
            foreach (var user in initDiscoveryList)
            {
                DiscoveryList.Add(user);
            }

            //ResetBackGroundColor(DiscoveryList);
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

            //ResetBackGroundColor(DiscoveryList);
        }
    }

    [RelayCommand]
    void PerformMyUserSearch(string query)
    {
        if (query == string.Empty)
        {
            MyUsers.Clear();
            foreach (var user in initMyUsers)
            {
                MyUsers.Add(user);
            }

            //ResetBackGroundColor(MyUsers);
        }
        else
        {
            MyUsers.Clear();
            foreach (var user in initMyUsers)
            {
                if(user.UserName.ToLower().Contains(query.ToLower()))
                {
                    MyUsers.Add(user);
                }
            }

            //ResetBackGroundColor(MyUsers);
        }
    }

    void ResetBackGroundColor(ObservableCollection<User> list)
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
    }

}