using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary
{
    public class Monoalphabetic : ICryptographicTechnique<string, string>
    {
        public string Analyse(string plainText, string cipherText)
        {
            cipherText = cipherText.ToLower();
            // throw new NotImplementedException();
            char[] key = new char[26];
            for (int j = 0; j < key.Length; j++)
            {
                key[j] = (char)(j * 1000);
            }
            //throw new NotImplementedException();
            for (int i = 0; i < cipherText.Length; i++)
            {
                int k = (plainText[i] - 97);
                key[k] = cipherText[i];
            }

            return new string(key);
        }
        public string Decrypt(string cipherText, string key)
        {
            //  throw new NotImplementedException();
            key = key.ToUpper();
            //throw new NotImplementedException();
            char[] chars = new char[cipherText.Length];
            for (int i = 0; i < cipherText.Length; i++)
            {
                for (int j = 0; j < key.Length; j++)
                {
                    if (key[j] == cipherText[i])
                    {
                        char h = (char)(j + 97);
                        chars[i] = h;
                    }
                }



            }
            return new string(chars);
        }

        public string Encrypt(string plainText, string key)
        {
            // throw new NotImplementedException();
            // throw new NotImplementedException();
            char[] chars = new char[plainText.Length];
            for (int i = 0; i < plainText.Length; i++)
            {
                if (plainText[i] == ' ')
                {
                    //  chars[i] = ' ';
                    continue;
                }
                else
                {
                    int j = plainText[i] - 97;
                    chars[i] = key[j];
                }
            }
            return new string(chars);
        }

        /// <summary>
        /// Frequency Information:
        /// E   12.51%
        /// T	9.25
        /// A	8.04
        /// O	7.60
        /// I	7.26
        /// N	7.09
        /// S	6.54
        /// R	6.12
        /// H	5.49
        /// L	4.14
        /// D	3.99
        /// C	3.06
        /// U	2.71
        /// M	2.53
        /// F	2.30
        /// P	2.00
        /// G	1.96
        /// W	1.92
        /// Y	1.73
        /// B	1.54
        /// V	0.99
        /// K	0.67
        /// X	0.19
        /// J	0.16
        /// Q	0.11
        /// Z	0.09
        /// </summary>
        /// <param name="cipher"></param>
        /// <returns>Plain text</returns>
        public string AnalyseUsingCharFrequency(string cipher)
        {
            cipher = cipher.ToLower();
            double[] counter = new double[26];
            char[] chaArray = new char[26];
            for (char i = 'a'; i <= 'z'; i++)
            {
                chaArray[i - 'a'] = i;
                for (int j = 0; j < cipher.Length; j++)
                {
                    if (i == cipher[j])
                    {
                        counter[i - 'a']++;
                    }
                }
                counter[i - 'a'] = counter[i - 'a'] / (double)cipher.Length;
            }
            Array.Sort(counter, chaArray);
            char[] Given_freq = { 'z', 'q', 'j', 'x', 'k', 'v', 'b', 'y', 'w', 'g', 'p', 'f', 'm', 'u', 'c', 'd', 'l', 'h', 'r', 's', 'n', 'i', 'o', 'a', 't', 'e' };
            string key = "";
            for (char i = 'a'; i <= 'z'; i++)
            {
                for (int j = 0; j < 26; j++)
                {
                    if (Given_freq[j] == i)
                    {
                        key += chaArray[j];
                    }
                }
            }
            string p = Decrypt(cipher, key);
            return p;
        }
    }
}
