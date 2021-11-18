using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using MongoDB.Driver;

namespace API.Services
{
    public class ContactService
    {
        private readonly IMongoCollection<Contact> _contacto;

        public ContactService(IContactDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _contacto = database.GetCollection<Contact>(settings.ContactCollectionName);
        }

        public List<Contact> Get() => _contacto.Find(message => true).ToList();

        public Contact Get(string id) => _contacto.Find(message => message.Id == id).FirstOrDefault();

        public Contact Create(Contact message)
        {
            _contacto.InsertOne(message);
            return message;
        }

        public void Update(string id, Contact messageIn) => _contacto.ReplaceOne(message => message.Id == id, messageIn);

        public void Remove(Contact messageIn) => _contacto.DeleteOne(message => message.Id == messageIn.Id);

        public void Remove(string id) => _contacto.DeleteOne(message => message.Id == id);
    }
}
