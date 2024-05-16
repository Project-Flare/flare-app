using SQLite;
using flare_app.Models;

namespace flare_app.Services;

public class LocalUserDBService
{
    private const string LOCAL_USERS = "local_AppDATA.db";
    static SQLiteAsyncConnection? _localUserConnection;

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
    }

    /// <summary>
    /// Returns all saved local users in database.
    /// </summary>
    public static async Task<IEnumerable<LocalUser>> GetAllLocalUsers()
    {
        await Init();
        if(_localUserConnection is not null)
            return await _localUserConnection.Table<LocalUser>().ToListAsync();

        return Enumerable.Empty<LocalUser>();
	}

    /// <summary>
    /// Returns a local user from database.
    /// </summary>
    /// <returns>Local user</returns>
    public static async Task<LocalUser?> GetLocalUserByName(string UserName)
    {
        await Init();
        if (_localUserConnection is not null)
        {
            foreach (LocalUser usr in await _localUserConnection.Table<LocalUser>().ToListAsync())
            {
                if (usr.LocalUserName == UserName)
                    return usr;
            }
        }

        return null;
    }

    /// <summary>
    /// Inserts new 'LocalUser' into database.
    /// </summary>
    public static async Task InsertLocalUser(LocalUser? user)
    {
		await Init();
		if(_localUserConnection is not null)
            await _localUserConnection.InsertAsync(user);
    }

	/// <summary>
	/// Updates 'LocalUser' (his AuthToken).
	/// </summary>
    // Can be redesigned to be updated by only using user's primary key (LocalUserName).
	public static async Task UpdateLocalUserAuthToken(LocalUser user, string newAuthToken)
	{
		await Init();

        LocalUser updated = new LocalUser
        {
            LocalUserName = user.LocalUserName,
            AuthToken = newAuthToken,
            PublicKey = user.PublicKey,
            PrivateKey = user.PrivateKey
        };

        if (_localUserConnection is not null)
            await _localUserConnection.UpdateAsync(updated);
	}

	/// <summary>
	/// Deletes 'LocalUser' from database.
	/// </summary>
	public static async Task DeleteLocalUser(LocalUser? user)
    {
        await Init();
		if(_localUserConnection is not null)
            await _localUserConnection.DeleteAsync(user);
    }

	/// <summary>
	/// Deletes ALL 'LocalUser' from database.
	/// </summary>
	public static async Task DeleteAllLocalUsers()
	{
		await Init();
		//await _localUserConnection.DeleteAllAsync(new LocalUser { });
        if (_localUserConnection is not null)
            await _localUserConnection.ExecuteAsync("DELETE FROM LocalUser");
	}

    /// <summary>
    /// Deletes 'LocalUser' from database by name parameter.
    /// </summary>
    public static async Task DeleteLocalUserByName(string UserName)
    {
        await Init();
        if (_localUserConnection is not null)
        {
            foreach (LocalUser usr in await _localUserConnection.Table<LocalUser>().ToListAsync())
            {
                if (usr.LocalUserName == UserName)
                {
                    await _localUserConnection.DeleteAsync(usr);
                    return;
                }
            }
        }
    
    }

    // Contacts part...

    /// <summary>
    /// Returns ALL contacts from database.
    /// </summary>
    /// <returns>Absolutely every contact</returns>
    public static async Task<IEnumerable<MyContact>?> GetAllContacts()
    {
        await Init();
        if (_localUserConnection is not null)
            return await _localUserConnection.Table<MyContact>().ToListAsync();

        return null;
    }

    /// <summary>
    /// Returns ALL MY contacts from database.
    /// </summary>
    /// <returns>All my contacts</returns>
    public static async Task<IEnumerable<MyContact>> GetAllMyContacts(string ownerName)
    {
        await Init();
        if (_localUserConnection is not null)
            return await _localUserConnection.Table<MyContact>()
                                 .Where(c => c.ContactOwner == ownerName)
                                 .ToListAsync();

        return Enumerable.Empty<MyContact>();
    }

    /// <summary>
    /// Inserts my contact into database.
    /// </summary>
    public static async Task InsertContact(MyContact contact)
    {
        await Init();
        contact.OwnerContactPair = $"{contact.ContactOwner}_{contact.ContactUserName!}";
        if (_localUserConnection is not null)
            await _localUserConnection.InsertAsync(contact);
    }

    /// <summary>
    /// Deletes my contact form database.
    /// </summary>
    public static async Task DeleteContact(MyContact? contact)
    {
        await Init();
        if (_localUserConnection is not null)
            await _localUserConnection.DeleteAsync(contact);
    }

    /// <summary>
    /// Deletes my contact by his username and my username.
    /// </summary>
    public static async Task DeleteContactByName(string UserName, string ownerName)
    {
        await Init();
        if (_localUserConnection is not null)
        {
            foreach (var itm in await _localUserConnection.Table<MyContact>().ToListAsync())
            {
                if (itm.ContactUserName == UserName && itm.ContactOwner == ownerName)
                {
                    await DeleteContact(itm);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Performs my contact search in database by name.
    /// </summary>
    public static async Task<IEnumerable<MyContact>?> SearchMyContact(string? query, string owner)
    {
        await Init();
        if (_localUserConnection is not null)
        {
            if (query is not null)
                return await _localUserConnection.Table<MyContact>()
                                                         .Where(c => c.ContactUserName!.ToLower().Contains(query.ToLower()) && c.ContactOwner == owner)
                                                         .ToListAsync();
            else
                return await _localUserConnection.Table<MyContact>().Where(c => c.ContactOwner == owner).ToListAsync();
        }
        return null;
    }

}
