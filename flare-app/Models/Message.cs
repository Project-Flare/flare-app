using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flare_app.Models;
public class Message : IEquatable<Message>, IComparable<Message>
{
    // This is a pair between me (logged on user) and my contact. This pair should look like: {LocalUserName}_{ContactUserName}.
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string? KeyPair { get; set; }
    public string? Sender { get; set; }
    public string? Content { get; set; }
    public DateTime Time { get; set; }
	public ulong Counter { get; set; }

	public int CompareTo(Message? other)
	{
		if (other is null)
			return -1;

		return Counter.CompareTo(other.Counter);
	}

	public bool Equals(Message? other)
	{
		if (other ==  null) return false;

        return Time == other.Time && Content == other.Content;
	}


}