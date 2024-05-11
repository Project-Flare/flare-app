using SQLite;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace flare_app.Models;

// local device users
public class LocalUser
{
    [PrimaryKey, Unique]
    public string? LocalUserName { get; set; }
    public string? AuthToken { get; set; }
}

public class MyContact
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Indexed]
    public string? ContactOwner { get; set; }
    public string? ContactUserName { get; set; }
    [Unique]
    public string? OwnerContactPair { get; set; }
}

public class Message
{
	// This is a pair between me (logged on user) and my contact. This pair should look like: {LocalUserName}_{ContactUserName}.
	[PrimaryKey]
	public string? KeyPair { get; set; }
	[Indexed]
	public LocalUser? Sender { get; set; }
	public string? Content { get; set; }
	public DateTime Time { get; set; }
}

