using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using flare_app.Models;

namespace flare_app.ViewModel;

public partial class MainViewModel : ObservableObject
{

    // List should be taken from client class
    public MainViewModel()
    {
        Items = new ObservableCollection<User>
        {
            new User
            {
                UserName = "Vienas",
            },
            new User
            {
                UserName = "Du",
            },
            new User
            {
                UserName = "Trys",
            },
            new User
            {
                UserName = "Keturi",
            }
        };

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

        ResetBackGroundColor(NewUsers);
        ResetBackGroundColor(Items);
    }
       

    [ObservableProperty]
    ObservableCollection<User> items;

    [ObservableProperty]
    ObservableCollection<User> newUsers;

    [RelayCommand]
    void AddUser(string s)
    {
        User removeThis = Items.FirstOrDefault(u => u.UserName == s);

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
        User removeThis = NewUsers.FirstOrDefault(u => u.UserName == s);

        if (removeThis != null)
        {
            NewUsers.Remove(removeThis);
            Items.Add(removeThis);
            ResetBackGroundColor(NewUsers);
            ResetBackGroundColor(Items);
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