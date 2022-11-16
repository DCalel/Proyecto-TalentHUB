using System;
using System.Collections.Generic;
using System.Text;

namespace BibliotecaGenerica
{
    public class LZW
    {
        public List<int> Compress(string uncompressed)
        {
            int dictionarySize = 256;
            Dictionary<string,int> dictionary = new Dictionary<string, int>();

            for (int i = 0; i < dictionarySize; i++)
            {
                dictionary.Add("" + (char)i, i);
            }

            string w = "";

            List<int> result = new List<int>();

            foreach (char c in uncompressed.ToCharArray())
            {
                string wc = w + c;
                if (dictionary.ContainsKey(wc))
                {
                    w = wc;
                }
                else
                {
                    result.Add(dictionary[w]);
                    dictionary.Add(wc, dictionarySize++);
                    w = "" + c;
                }
            }

            if (!w.Equals(""))
            {
                result.Add(dictionary[w]);               
            }
            return result;
        }

        public string Decompress(List<int> compressed)
        {
            int dictionarySize = 256;
            Dictionary<int, string> dictionary = new Dictionary<int, string>();

            for (int i = 0; i < dictionarySize; i++)
            {
                dictionary.Add(i, "" + (char)i);
            }

            
            string w = "" + Convert.ToChar(compressed[0]);
            compressed.RemoveAt(0);

            StringBuilder result = new StringBuilder(w);
            foreach (int k in compressed)
            {
                string entry = "";
                if (dictionary.ContainsKey(k))
                {
                    entry = dictionary[k];
                }
                else if (k == dictionarySize)
                {
                    entry = w + w[0];
                }
                else
                {
                    break;
                }

                result.Append(entry);

                dictionary.Add(dictionarySize++, w + entry[0]);

                w = entry;
            }
            return result.ToString();
        }


    }
}
