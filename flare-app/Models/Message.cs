using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flare_app.Models;
public class Message : IEquatable<Message>
{
    // This is a pair between me (logged on user) and my contact. This pair should look like: {LocalUserName}_{ContactUserName}.
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string? KeyPair { get; set; }
    public string? Sender { get; set; }
    public string? Content { get; set; }
    public DateTime Time { get; set; }

	public bool Equals(Message? other)
	{
		throw new NotImplementedException();
	}
}