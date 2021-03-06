using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;

namespace ChatDesign.Models
{
    public class Conversation
    {
        public List<Message> Messages { get; set; }
        public string Receiver { get; set; }
        public string CurrentUser { get; set; }
        public List<Message> Files { get; set; }

        public string SearchedMessage { get; set; }

        public Conversation(List<Message> messages, List<Message> files, string receiver, string currentuser)
        {
            if (messages == null)
            {
                Messages = new List<Message>();
            }
            else
            {
                Messages = messages;
            }
            if (files == null)
            {
                Files = new List<Message>();
            }
            else
            {
                Files = files;
            }
            Receiver = receiver;
            CurrentUser = currentuser;
        }

        public Conversation(List<Message> messages, string receiver, string searched)
        {
            if (messages == null)
            {
                Messages = new List<Message>();
            }
            else
            {
                Messages = messages;
            }
            Receiver = receiver;
            SearchedMessage = searched;
        }
    }
}
