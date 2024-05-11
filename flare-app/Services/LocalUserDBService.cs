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

    /// <summary>
    /// Initiates whole local database, creates tables, initiates connection.
    /// </summary>
    async static Task Init()
    {
        if(_localUserConnection != null)
            return;

        _localUserConnection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, LOCAL_USERS));
        await _localUserConnection.CreateTableAsync<LocalUser>();
        await _localUserConnection.CreateTableAsync<MyContact>();
        await _localUserConnection.CreateTableAsync<Message>();
    }

    /// <summary>
    /// Returns all saved local users in database.
    /// </summary>
    public static async Task<IEnumerable<LocalUser>> GetAllLocalUsers()
    {
        await Init();
        return await _localUserConnection.Table<LocalUser>().ToListAsync();
    }

    /// <summary>
    /// Returns a local user from database.
    /// </summary>
    /// <returns>Local user</returns>
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

    /// <summary>
    /// Inserts new 'LocalUser' into database.
    /// </summary>
    public static async Task InsertLocalUser(LocalUser? user)
    {
        await Init();
        await _localUserConnection.InsertAsync(user);
    }

    /// <summary>
    /// Deletes 'LocalUser' from database.
    /// </summary>
    public static async Task DeleteLocalUser(LocalUser? user)
    {
        await Init();
        await _localUserConnection.DeleteAsync(user);
    }

    /// <summary>
    /// Deletes 'LocalUser' from database by name parameter.
    /// </summary>
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

    // Contacts part...

    /// <summary>
    /// Returns ALL contacts from database.
    /// </summary>
    /// <returns>Absolutely every contact</returns>
    public static async Task<IEnumerable<MyContact>> GetAllContacts()
    {
        await Init();
        return await _localUserConnection.Table<MyContact>().ToListAsync();
    }

    /// <summary>
    /// Returns ALL MY contacts from database.
    /// </summary>
    /// <returns>All my contacts</returns>
    public static async Task<IEnumerable<MyContact>> GetAllMyContacts(string ownerName)
    {
        await Init();
        return await _localUserConnection.Table<MyContact>()
                                 .Where(c => c.ContactOwner == ownerName)
                                 .ToListAsync();
    }

    /// <summary>
    /// Inserts my contact into database.
    /// </summary>
    public static async Task InsertContact(MyContact contact)
    {
        await Init();
        contact.OwnerContactPair = $"{contact.ContactUserName}_{contact.ContactOwner}";
        await _localUserConnection.InsertAsync(contact);
    }

    /// <summary>
    /// Deletes my contact form database.
    /// </summary>
    public static async Task DeleteContact(MyContact? contact)
    {
        await Init();
        await _localUserConnection.DeleteAsync(contact);
    }

    /// <summary>
    /// Deletes my contact by his username and my username.
    /// </summary>
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

    /// <summary>
    /// Performs my contact search in database by name.
    /// </summary>
    public static async Task<IEnumerable<MyContact>> SearchMyContact(string query, string owner)
    {
        await Init();
        return await _localUserConnection.Table<MyContact>()
														 .Where(c => c.ContactUserName.Contains(query) && c.ContactOwner == owner)
                                                         .ToListAsync();
    }

	// Messages part...

	public static async Task<IEnumerable<Message>> GetMessages(string user, string contact)
	{
		await Init();
		return await _localUserConnection.Table<Message>()
								 .Where(c => c.KeyPair == $"{user}_{contact}")
								 .ToListAsync();
	}

	public static async Task InsertMessage(Message message)
	{
		await Init();
		await _localUserConnection.InsertAsync(message);
	}

	public static async Task DeleteMessage(Message message)
	{
		await Init();
		await _localUserConnection.DeleteAsync(message);
	}
}
