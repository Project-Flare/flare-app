using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace flare_app.ViewModel;

public partial class MainViewModel : ObservableObject
{
    // List should be taken from client class
    public MainViewModel()
    {
        Items = new ObservableCollection<string>
        {
            "Vienas",
            "Du",
            "Trys",
            "Keturi",
        };

        NewUsers = new ObservableCollection<string>
        {
            "Petras",
            "Jonas",
        };
    }
       

    [ObservableProperty]
    ObservableCollection<string> items;

    [ObservableProperty]
    ObservableCollection<string> newUsers;

    [RelayCommand]
    void AddUser(string s)
    {
        if(Items.Contains(s))
        {
            Items.Remove(s);
            NewUsers.Add(s);
        }
    }
    [RelayCommand]
    void RemoveUser(string s)
    {
        if (NewUsers.Contains(s))
        {
            NewUsers.Remove(s);
            Items.Add(s);
        }
    }
}
