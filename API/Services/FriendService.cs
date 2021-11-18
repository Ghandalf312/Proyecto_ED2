using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using API.Models;
using MongoDB.Driver;

namespace API.Services
{
    public class FriendService
    {
        private readonly IMongoCollection<Friend> _chat;

        public FriendService(IFriendDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _chat = database.GetCollection<Friend>(settings.InvitationCollectionName);
        }

        public List<Friend> Get() => _chat.Find(message => true).ToList();

        public Friend Get(string id) => _chat.Find(message => message.Id == id).FirstOrDefault();

        public Friend Create(Friend message)
        {
            _chat.InsertOne(message);
            return message;
        }

        public void Update(string id, Friend messageIn) => _chat.ReplaceOne(message => message.Id == id, messageIn);

        public void Remove(Friend messageIn) => _chat.DeleteOne(message => message.Id == messageIn.Id);

        public void Remove(string id) => _chat.DeleteOne(message => message.Id == id);

    }
}
