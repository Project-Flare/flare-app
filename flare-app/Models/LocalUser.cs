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
    public string? PublicKey {  get; set; }
    public string? PrivateKey { get; set; }
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

