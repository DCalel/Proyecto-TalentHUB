using BibliotecaGenerica.DES.Boxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BibliotecaGenerica.DES
{
    public static class Operations
    {
        public static string StringToBits(string message)
        {
            Encoding iso = Encoding.GetEncoding("ISO-8859-1");

            return string.Join("", iso.GetBytes(message).Select(n => Convert.ToString(n, 2).PadLeft(8, '0')));
        }

        public static string BitsToString(string bits)
        {
            byte[] bytes = new byte[bits.Length / 8];
            int j = 0;
            while (bits.Length > 0)
            {             
                var result = Convert.ToByte(bits.Substring(0, 8), 2);
                bytes[j++] = result;
                if (bits.Length >= 8)
                {
                    bits = bits.Substring(8);
                }
            }
            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            return iso.GetString(bytes);
        }

        public static string MatrixOperation(int[] array, string value)
        {
            StringBuilder sb = new StringBuilder();
            int cont = 0;
            foreach (int pos in array)
            {
                cont++;
                sb.Append(value[pos - 1]);
            }
            return sb.ToString();
        }

        public static string Xor(string m1, string m2)
        {
            char[] arrayM1 = m1.ToCharArray();
            char[] arrayM2 = m2.ToCharArray();
            StringBuilder result = new StringBuilder();
            if (arrayM1.Length == arrayM2.Length)
            {
                for (int i = 0; i < arrayM1.Length; i++)
                {
                    result.Append(Convert.ToInt32(char.GetNumericValue(arrayM1[i])) ^ Convert.ToInt32(char.GetNumericValue(arrayM2[i])));
                }
            }
            return result.ToString();
        }

        public static string FeistelSbox(string data)
        {
            int ini = 0;
            int index = 1;
            int row;
            int col;
            StringBuilder sb = new StringBuilder();
            while (ini < data.Length)
            {
                string sub = data.Substring(ini + 1, 4);
                string sub2 = data[ini] + "" + data[ini + 5];
                col = Convert.ToInt32(sub, 2) - 1;
                row = Convert.ToInt32(sub2, 2) - 1;
                if (row < 0)
                {
                    row = 0;
                }
                if (col < 0)
                {
                    col = 0;
                }
                switch (index)
                {
                    case 1:
                        sb.Append(PadLeftZeros(Convert.ToString(Sbox.S1[row, col], 2), 4));
                        break;
                    case 2:
                        sb.Append(PadLeftZeros(Convert.ToString(Sbox.S2[row, col], 2), 4));
                        break;
                    case 3:
                        sb.Append(PadLeftZeros(Convert.ToString(Sbox.S3[row, col], 2), 4));
                        break;
                    case 4:
                        sb.Append(PadLeftZeros(Convert.ToString(Sbox.S4[row, col], 2), 4));
                        break;
                    case 5:
                        sb.Append(PadLeftZeros(Convert.ToString(Sbox.S5[row, col], 2), 4));
                        break;
                    case 6:
                        sb.Append(PadLeftZeros(Convert.ToString(Sbox.S6[row, col], 2), 4));
                        break;
                    case 7:
                        sb.Append(PadLeftZeros(Convert.ToString(Sbox.S7[row, col], 2), 4));
                        break;
                    case 8:
                        sb.Append(PadLeftZeros(Convert.ToString(Sbox.S8[row, col], 2), 4));
                        break;
                    default:
                        break;
                }
                ini += 6;
                index++;
            }
            return sb.ToString();
        }

        private static string PadLeftZeros(string inputString, int length)
        {
            if (inputString.Length >= length)
            {
                return inputString;
            }
            StringBuilder sb = new StringBuilder();
            while (sb.Length < length - inputString.Length)
            {
                sb.Append('0');
            }
            sb.Append(inputString);

            return sb.ToString();
        }

        public static string[] ReverseArray(string[] array)
        {
            string[] b = new string[array.Length];
            int j = array.Length;
            for (int i = 0; i < array.Length; i++)
            {
                b[j - 1] = array[i];
                j--;
            }
            return b;
        }

    }
}
