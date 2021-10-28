using System;
using System.Collections.Generic;
using System.Text;

namespace CiphersAndCompression.Interfaces
{
    interface ICompressor
    {
        string CompressFile(string savingPath, string filePath, string name);
        string DecompressFile(string savingPath, string filePath, string name);
    }
}
