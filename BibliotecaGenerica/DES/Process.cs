using System;
using System.Collections.Generic;
using System.Text;
using BibliotecaGenerica.DES.Boxes;

namespace BibliotecaGenerica.DES
{
    public class Process
    {
        Key Generatekeys = new Key();
        Feistel feistel = new Feistel();

        public string InitialPermutation(string bits)
        {
            return Operations.MatrixOperation(Pbox.IP, bits);
        }

        public string[] GetKeys(string key)
        {
            string keybits = Operations.StringToBits(key);
            return Generatekeys.GetKeys(keybits);
        }

        public string GetFeistel(string message, string key)
        {
            string left = message.Substring(0, 32);
            string right = message.Substring(32, 32);
            return feistel.FeistelProcess(left, right, key);
        }

        public string InversalPermutation(string bits)
        {
            return Operations.MatrixOperation(Pbox.INVERSE_IP, bits);
        }

        public string ExchangeSides(string message)
        {
            return message.Substring(32, 32) + message.Substring(0, 32);
        }

    }
}
