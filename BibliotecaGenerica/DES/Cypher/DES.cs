using System;
using System.Collections.Generic;
using System.Text;
using BibliotecaGenerica.DES;

namespace BibliotecaGenerica.DES.Cypher
{
    public class DES
    {
        readonly Process process = new Process();

        public string Encrypt(string message, string key)
        {
            string bits = Operations.StringToBits(message);
            string ip = process.InitialPermutation(bits);
     
            string[] keys = process.GetKeys(key);

            foreach (string k in keys)
            {
                ip = process.GetFeistel(ip, k);
            }

            string CipherText = process.ExchangeSides(ip);

            CipherText = process.InversalPermutation(CipherText);
            return CipherText;
        }
        
        public string Decrypt(string message, string key)
        {
            string bits = Operations.StringToBits(message);
            bits = bits.Replace(" ", String.Empty);
            string ip = process.InitialPermutation(bits);

            string[] keys = process.GetKeys(key);
            keys = Operations.ReverseArray(keys);
            
            foreach (string k in keys)
            {
                ip = process.GetFeistel(ip, k);
            }

            string CipherText = process.ExchangeSides(ip);

            CipherText = process.InversalPermutation(CipherText);
            return CipherText;
        }

    }
}
