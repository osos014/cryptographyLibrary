using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary
{
    public class AutokeyVigenere : ICryptographicTechnique<string, string>
    {
        public string Analyse(string plainText, string cipherText)
        {
            cipherText = cipherText.ToUpper();
            plainText = plainText.ToUpper();
            char[,] Opss = Create_Table();
            //string Key_Stream = Generate_Key_Stream_Repeat(CT, Key);
            string Key_Stream = "";
            string Key = "";
            for (int x = 0; x < cipherText.Length; x++)
            {
                for (int b = 0; b < 26; b++)
                {
                    if (Opss[plainText[x] - 'A', b] == cipherText[x])
                    {
                        Key_Stream += (char)(b + 'A');
                        break;
                    }
                }

            }
            int count = 0;
            bool Check = true;
            Key = Key_Stream.Substring(0, 3);
            for (int i = 3; i < Key_Stream.Length; i++)
            {
                if (plainText[count] == Key_Stream[i])
                {
                    Check = false;
                    count++;
                    continue;
                }
                else
                {
                    if (Check)
                    {
                        Key += Key_Stream[i];
                    }
                    else
                    {
                        for (int k = count; k >= 0; k++)
                        {
                            Key += Key_Stream[i - count];

                        }
                    }

                }
            }
            return Key;
        }

        public string Decrypt(string cipherText, string key)
        {
            cipherText = cipherText.ToUpper();
            key = key.ToUpper();
            char[,] Opss = Create_Table();
            // string Key_Stream = Generate_Key_Stream(PT, Key);
            string Plain = "";
            int counter = 0;
            for (int x = 0; x < cipherText.Length; x++)
            {
                for (int b = 0; b < 26; b++)
                {
                    if (x < key.Length)
                    {
                        if (Opss[b, key[x] - 'A'] == cipherText[x])
                        {
                            Plain += (char)(b + 'A');
                            break;
                        }
                    }
                    else
                    {
                        if (Opss[b, Plain[counter] - 'A'] == cipherText[x])
                        {
                            Plain += (char)(b + 'A');
                            counter++;
                            break;
                        }
                    }
                }
                //Cipher += Opss[PT[x] - 'A', Key_Stream[x] - 'A'];
            }
            return Plain;
        }

        public string Encrypt(string plainText, string key)
        {
            plainText = plainText.ToUpper();
            key = key.ToUpper();
            char[,] Opss = Create_Table();
            string Key_Stream = Generate_Key_Stream(plainText, key);
            string Cipher = "";

            for (int x = 0; x < plainText.Length; x++)
            {
                Cipher += Opss[plainText[x] - 'A', Key_Stream[x] - 'A'];
            }
            return Cipher;
        }
        public char[,] Create_Table()
        {
            char[,] arrr = new char[26, 26];
            for (int i = 0; i < 26; i++)
            {
                char Ch = 'A';
                char Ph;
                for (int j = 0; j < 26; j++)
                {
                    if ((Ch - 'A') + i > 25)
                    {
                        Ph = (char)((((Ch - 'A') + i) % 26) + 'A');
                        arrr[i, j] = Ph;
                        Ch++;
                        continue;
                    }
                    arrr[i, j] = (char)(((Ch - 'A') + i) + 'A');
                    Ch++;
                }

            }
            return arrr;
        }
        public string Generate_Key_Stream(string PT, string Key)
        {
            int x = PT.Length;
            int y = Key.Length;
            int z;

            if (y >= x)
            {
                return Key;
            }
            else
            {
                z = x - y;
                for (int w = 0; w < z; w++)
                {
                    Key += PT[w];
                }
                return Key;
            }

        }
    }
}
