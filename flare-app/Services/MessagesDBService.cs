using SQLite;
using flare_app.Models;
using System;
using System.Collections.Generic;

namespace flare_app.Services
{
    class MessagesDBService
    {
        private const string MESSAGES = "messages3.db";
        static SQLiteAsyncConnection? _messagesConnection;

        public MessagesDBService()
        {
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

        public static async Task<IEnumerable<Message>> GetMessages(string keypair)
        {
            await Init();
            //if (_messagesConnection is not null)
            return await _messagesConnection.Table<Message>().Where(c => c.KeyPair == keypair).ToListAsync();

            //return Enumerable.Empty<Message>();
        }

        public static async Task InsertMessage(Message message)
        {
            await Init();
            await _messagesConnection.InsertAsync(message);
        }

        /*public static async Task DeleteMessage(Message message)
        {
            await Init();
            await _localUserConnection.DeleteAsync(message);
        }*/
    }
}
