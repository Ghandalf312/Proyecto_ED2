using System;
using System.Collections.Generic;
using System.Text;


namespace CiphersAndCompression.Interfaces
{
    interface IEncryptor
    {
        string EncryptString(string text, int key);
        string DecryptString(string text, int key);
    }
}
