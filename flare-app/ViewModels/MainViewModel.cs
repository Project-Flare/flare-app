using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using flare_app.Models;
using flare_csharp;
namespace flare_app.ViewModels;

public partial class MainViewModel : ObservableObject
{

    // List should be taken from client class
    public MainViewModel()
    {
        Items = new ObservableCollection<User>();

        NewUsers = new ObservableCollection<User>
        {
            new User
            {
                UserName = "Petras",
            },
            new User
            {
                UserName = "Jonas",
            }
        };

        //ResetBackGroundColor(NewUsers);
        //ResetBackGroundColor(Items);

        foreach (var usr in Client.UserDiscoveryList)
        {
            User itm = new User
            {
                UserName = usr.Username
            };

            Items.Add(itm);
        }
    }

    [ObservableProperty]
    ObservableCollection<User> items;

    [ObservableProperty]
    ObservableCollection<User> newUsers;

    [RelayCommand]
    void AddUser(string s)
    {
        User? removeThis = Items.FirstOrDefault(u => u.UserName == s);

        if (removeThis != null)
        {
            Items.Remove(removeThis);
            NewUsers.Add(removeThis);
            ResetBackGroundColor(NewUsers);
            ResetBackGroundColor(Items);
        }
    }
    [RelayCommand]
    void RemoveUser(string s)
    {
        User? removeThis = NewUsers.FirstOrDefault(u => u.UserName == s);

        if (removeThis != null)
        {
            NewUsers.Remove(removeThis);
            Items.Add(removeThis);
            ResetBackGroundColor(NewUsers);
            ResetBackGroundColor(Items);
        }
    }

    [RelayCommand]
    void PerformSearchCommand(string query)
    {
        Items.Add(new User { UserName="something" });
        items.Add(new User { UserName = "something" });
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