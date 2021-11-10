using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace API.Models
{
    public class ChatDbSettings : IChatDbSettings
    { 
        public string ChatCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IChatDbSettings
    {
        string ChatCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
