using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary
{
    public class PlayFair : ICryptographicTechnique<string, string>
    {
        public string Analyse(string plainText, string cipherText)
        {
            throw new NotImplementedException();
        }

        public string Decrypt(string cipherText, string key)
        {
           
            string plain = trailencrypt(cipherText, key, 4);
            plain = plain.ToUpper();
            return removex(plain);
        }

        public string Encrypt(string plainText, string key)
        {
            plainText = plainText.ToLower();
            key = key.ToLower();
            return trailencrypt(plainText, key, 1).ToUpper();
        }

        private static string trailencrypt(string plaintext, string key, int direction)
        {

            // Character case doesn't matter
            key = key.ToUpper();
            plaintext = plaintext.ToUpper();

            String sb = "";

            // Initialize Matrix with key
            
            char[,] Matrix = new char[5, 5];
            int index = 0;
            for (int i = 0; i < 5; ++i)
            {
                for (int j = 0; j < 5; ++j)
                {
                    if (index < key.Length)
                    {
                        insert(Matrix, key[index++]);
                    }
                }
            }

            // Fill remaining characters in Matrix
            for (char i = 'A'; i <= 'Z'; ++i)
            {
                if (i != 'J')
                {
                    insert(Matrix, i);
                }
            }

            // Zabbat el plaintext
            StringBuilder ptsb = new StringBuilder(plaintext);

            for (int i = 1; i < ptsb.Length; i += 2)
            {
                if (ptsb[i] == ptsb[i - 1])
                {
                    ptsb.Insert(i, 'X');
                }
            }
            if (ptsb.Length % 2 != 0)
            {
                ptsb.Append('X');
            }

            // Geeb el ciphertext for every pair

            for (int i = 0; i < ptsb.Length; i += 2)
            {
                int[] x = find(Matrix, ptsb[i]);
                int[] y = find(Matrix, ptsb[i + 1]);
                int x1 = x[0], x2 = x[1], y1 = y[0], y2 = y[1];

                if (x1 == y1)
                {
                    sb += Matrix[x1, (x2 + direction) % 5];
                    sb += Matrix[y1, (y2 + direction) % 5];
                }
                else if (x2 == y2)
                {
                    sb += Matrix[(x1 + direction) % 5, x2];
                    sb += Matrix[(y1 + direction) % 5, y2];
                }
                else
                {
                    sb += Matrix[x1, y2];
                    sb += Matrix[y1, x2];
                }
            }

            return sb.ToString();
        }


        public string removex(String str1)
        {
            String res = "";
            str1 = str1.ToLower();
            if (str1[str1.Length - 1] == 'x')
            {
                for (int i = 0; i < str1.Length - 1; i++)
                {
                    res += str1[i];
                }
            }
            else
            {
                res += str1[0];
                for (int i = 1; i < str1.Length - 1; i++)
                {
                    if (str1[i] == 'x')
                    {
                        if (str1[i - 1] == str1[i + 1])
                        {
                            continue;
                        }

                    }
                    else
                        res += str1[i];
                }
                res += str1[str1.Length - 1];
            }

            return res;
        }


        private static void insert(char[,] Matrix, char alpha)
        {
            if (exists(Matrix, alpha))
            {
                return;
            }
            for (int i = 0; i < 5; ++i)
            {
                for (int j = 0; j < 5; ++j)
                {
                    if (Matrix[i, j] == '\u0000')
                    {
                        Matrix[i, j] = alpha;
                        return;
                    }
                }
            }
        }

        private static bool exists(char[,] Matrix, char alpha)
        {
            for (int i = 0; i < Matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < Matrix.GetLength(1); ++j)
                {
                    if (Matrix[i, j] == alpha)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static int[] find(char[,] Matrix, char alpha)
        {
            if (alpha == 'J')
            {
                return find(Matrix, 'I');
            }
            int[] position = new int[2];
            for (int i = 0; i < Matrix.Length; ++i)
            {
                for (int j = 0; j < Matrix.GetLength(0); ++j)
                {
                    if (Matrix[i, j] == alpha)
                    {
                        position[0] = i;
                        position[1] = j;
                        return position;
                    }
                }
            }
            return position;
        }

    }
}
