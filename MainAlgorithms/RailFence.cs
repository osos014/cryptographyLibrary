using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary
{
    public class RailFence : ICryptographicTechnique<string, int>
    {
        public int Analyse(string plainText, string cipherText)
        {
            // throw new NotImplementedException();
            cipherText = cipherText.ToLower();
            int k = 0;

            for (int i = 1; i < cipherText.Length; i++)
            {
                for (int j = 1; j < plainText.Length; j++)
                {
                    if (cipherText[i] == plainText[j] && cipherText[i] != plainText[j + 1])
                    {
                        return j;
                    }


                }

            }
            return k;
        }

        public string Decrypt(string cipherText, int key)
        {
            //throw new NotImplementedException();
            string cipher = "";
            int res;
            int index = 0;
            cipherText = cipherText.ToLower();
            cipherText = checkspace(cipherText);

            if (cipherText.Length % key == 0)
            {
                res = cipherText.Length / key;
            }
            else
            {
                res = ((cipherText.Length / key) + 1);
                cipherText += ' ';
            }
            char[,] arr = new char[key, res];
            for (int i = 0; i < key; i++)
            {
                for (int j = 0; j < res; j++)
                {
                    if (index > cipherText.Length - 1)
                    {
                        break;
                    }
                    arr[i, j] = cipherText[index];
                    index++;
                }
            }
            for (int i = 0; i < res; i++)
            {
                for (int j = 0; j < key; j++)
                {
                    cipher += arr[j, i];
                }
            }
            return checkspace(cipher);
        }
        public string checkspace(String plaintext)
        {
            string res = "";
            for (int i = 0; i < plaintext.Length; i++)
            {
                if (plaintext[i] != ' ')
                {
                    res += plaintext[i];
                }
            }
            return res;
        }

        public string Encrypt(string plainText, int key)
        {
            //throw new NotImplementedException();
            string cipher = "";
            int res;
            int index = 0;
            plainText = checkspace(plainText);
            if (plainText.Length % key == 0)
            {
                res = plainText.Length / key;
            }
            else
            {
                res = ((plainText.Length / key) + 1);
                plainText += ' ';
            }
            char[,] arr = new char[key, res];
            for (int i = 0; i < res; i++)
            {
                for (int j = 0; j < key; j++)
                {
                    if (index > plainText.Length - 1)
                    {
                        break;
                    }
                    arr[j, i] = plainText[index];
                    index++;
                }
            }
            for (int i = 0; i < key; i++)
            {
                for (int j = 0; j < res; j++)
                {
                    cipher += arr[i, j];
                }
            }
            return checkspace(cipher.ToUpper());
        }
    }
}
