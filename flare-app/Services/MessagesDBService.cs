using SQLite;
using flare_app.Models;
using System;

namespace flare_app.Services
{
    class MessagesDBService
    {
        private const string MESSAGES = "messages3.db";
        static SQLiteAsyncConnection? _messagesConnection;

        public MessagesDBService()
        {
            //
        }

        /// <summary>
        /// Initiates whole local database, creates tables, initiates connection.
        /// </summary>
        async static Task Init()
        {
            if (_messagesConnection != null)
                return;

            _messagesConnection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, MESSAGES));
            await _messagesConnection.CreateTableAsync<Message>();
        }

        public static async Task<IEnumerable<Message>?> GetMessages(string keypair)
        {
            await Init();
            if (_messagesConnection is not null)
                return await _messagesConnection.Table<Message>()
                                             .Where(c => c.KeyPair == keypair)
                                             .ToListAsync();
            return null;

            //return Enumerable.Empty<Message>();
        }

        public static async Task InsertMessage(Message message)
        {
            await Init();
            if (_messagesConnection is not null)
                await _messagesConnection.InsertAsync(message);
        }

        /// <summary>
        /// Deletes all messages related with specified contact;
        /// </summary>
        public static async Task DeleteUserMessages(string contact)
        {
            await Init();
            if (_messagesConnection is not null)
            {
                foreach (var msg in await _messagesConnection.Table<Message>().ToListAsync())
                {
                    if (msg.KeyPair! == contact)
                        await _messagesConnection.DeleteAsync(msg);
                }
            }
        }
    }
}
