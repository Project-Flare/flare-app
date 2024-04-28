using SQLite;
using flare_app.Models;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Collections.ObjectModel;

namespace flare_app.Services;

public class LocalUserDBService
{
    private const string LOCAL_USERS = "local_AppDATA.db";
	static SQLiteAsyncConnection _localUserConnection;

    public LocalUserDBService()
    {
    }

    async static Task Init()
    {
        if(_localUserConnection != null)
            return;

        _localUserConnection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, LOCAL_USERS));
        await _localUserConnection.CreateTableAsync<LocalUser>();
        await _localUserConnection.CreateTableAsync<MyContact>();
    }

    public static async Task<IEnumerable<LocalUser>> GetAllLocalUsers()
    {
        await Init();
        return await _localUserConnection.Table<LocalUser>().ToListAsync();
    }

    public static async Task<LocalUser?> GetLocalUserByName(string UserName)
    {
        await Init();
        foreach (LocalUser usr in await GetAllLocalUsers())
        {
            if(usr.LocalUserName == UserName)
                return usr;
        }

        return null;
    }

    public static async Task InsertLocalUser(LocalUser? user)
    {
        await Init();
        await _localUserConnection.InsertAsync(user);
    }

    public static async Task DeleteLocalUser(LocalUser? user)
    {
        await Init();
        await _localUserConnection.DeleteAsync(user);
    }

    public static async Task DeleteLocalUserByName(string UserName)
    {
        await Init();
        foreach (LocalUser usr in await GetAllLocalUsers())
        {
            if (usr.LocalUserName == UserName)
            {
                await _localUserConnection.DeleteAsync(usr);
                return;
            }
        }
    }

    public static async Task<IEnumerable<MyContact>> GetAllContacts()
    {
        await Init();
        return await _localUserConnection.Table<MyContact>().ToListAsync();
    }

    public static async Task<IEnumerable<MyContact>> GetAllMyContacts(string ownerName)
    {
        await Init();
        return await _localUserConnection.Table<MyContact>()
                                 .Where(c => c.ContactOwner == ownerName)
                                 .ToListAsync();
    }

    public static async Task InsertContact(MyContact contact)
    {
        await Init();
        contact.OwnerContactPair = $"{contact.ContactUserName}_{contact.ContactOwner}";
        await _localUserConnection.InsertAsync(contact);
    }

    public static async Task DeleteContact(MyContact? contact)
    {
        await Init();
        await _localUserConnection.DeleteAsync(contact);
    }

    public static async Task DeleteContactByName(string UserName, string ownerName)
    {
        await Init();
        foreach (var itm in await GetAllContacts())
        {
            if (itm.ContactUserName == UserName && itm.ContactOwner == ownerName)
            {
                await DeleteContact(itm);
                return;
            }
        }
    }

    public static async Task<IEnumerable<MyContact>> SearchMyContact(string query, string owner)
    {
        await Init();
        return await _localUserConnection.Table<MyContact>()
														 .Where(c => c.ContactUserName.Contains(query) && c.ContactOwner == owner)
                                                         .ToListAsync();

    }
}
