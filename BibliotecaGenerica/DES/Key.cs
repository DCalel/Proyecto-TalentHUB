using System;
using System.Collections.Generic;
using System.Text;
using BibliotecaGenerica.DES.Boxes;


namespace BibliotecaGenerica.DES
{
    public class Key
    {
        public string[] GetKeys(string key)
        {
            string[] keys = new string[16];
            //Clear parity bits
            string orignalKey = Operations.MatrixOperation(Pbox.PC1, key);
            //The key is divided
            string left = orignalKey.Substring(0, 28);
            string right = orignalKey.Substring(28, 28);
            //Repeat 16 rounds
            for (int i = 1; i <= 16; i++)
            {
                if (i == 1 || i == 2 || i == 9 || i == 16)
                {
                    left = ShiftLeft(left, 1);
                    right = ShiftLeft(right, 1);
                }
                else
                {
                    left = ShiftLeft(left, 2);
                    right = ShiftLeft(right, 2);
                }

                keys[i - 1] = Operations.MatrixOperation(Pbox.PC2,left + right);
            }
            return keys;
        }

        private string ShiftLeft(string cd, int times)
        {
            int count = 1;
            char[] cdArray = cd.ToCharArray();
            while (count <= times)
            {
                char temp = cdArray[0];

                for (int i = 1; i < cdArray.Length; i++)
                {
                    cdArray[i - 1] = cdArray[i];
                }
                cdArray[cd.Length - 1] = temp;
                count++;
            }
            return string.Join("", cdArray);
        }

    }
}
