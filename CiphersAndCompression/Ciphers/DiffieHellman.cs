using System;
using System.Collections.Generic;
using System.Text;

namespace CiphersAndCompression.Ciphers
{
    public class DiffieHellman
    {
        private static int PrimeNumber = 1021;
        private static int GeneratorNumber = 503;
        public static int GetSecretKey(int userSecretRandom, int destinyPublicKey)
        {
            int secretKey = destinyPublicKey;
            for (int i = 0; i < userSecretRandom; i++)
            {
                secretKey *= destinyPublicKey;
                secretKey %= PrimeNumber;
            }
            return secretKey;
        }

        public static int GetPublicKey(int userSecretRandom)
        {
            int publicKey = GeneratorNumber;
            for (int i = 0; i < userSecretRandom; i++)
            {
                publicKey *= GeneratorNumber;
                publicKey %= PrimeNumber;
            }
            return publicKey;
        }
    }
}
