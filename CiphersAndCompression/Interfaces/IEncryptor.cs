using System;
using System.Collections.Generic;
using System.Text;

namespace CiphersAndCompression.Interfaces
{
    interface IEncryptor
    {
        string EncryptString(string text, string key);
        string DecryptString(string text, string key);
    }
}
