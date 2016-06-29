using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary
{
    public class Columnar : ICryptographicTechnique<string, List<int>>
    {
        public List<int> Analyse(string plainText, string cipherText)
        {
            plainText = plainText.ToUpper();
            cipherText = cipherText.ToUpper();
            List<int> key = new List<int>();
            int C = 3;
            while (true)
            {
                int Row = C++;
                int Col = cipherText.Length / Row;
                int counter = 0;
                char[,] Arr = new char[Row, Col];
                char[,] Arr2 = new char[Row, Col];
                for (int i = 0; i < Row; i++)
                {
                    for (int j = 0; j < Col; j++)
                    {
                        Arr[i, j] = plainText[counter];
                        counter++;
                    }
                }
                counter = 0;
                for (int i = 0; i < Col; i++)
                {
                    for (int j = 0; j < Row; j++)
                    {

                        Arr2[j, i] = cipherText[counter];
                        counter++;
                    }
                }

                bool che = true;
                for (int i = 0; i < Col; i++)
                {

                    for (int j = 0; j < Col; j++)
                    {
                        che = true;

                        for (int k = 0; k < Row; k++)
                        {
                            if (Arr[k, i] != Arr2[k, j])
                            {
                                che = false;
                                break;
                            }

                        }
                        if (che)
                        {
                            key.Add(j + 1);
                        }

                    }

                }
                if (che == false)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            while (key.Count < 10)
                key.Add(-1);
            return key;
        }

        public string Decrypt(string cipherText, List<int> key)
        {
            //string MPlain = Function(PT, Key);
            int Row = cipherText.Length / key.Count;
            int col = key.Count;
            string Plain = "";

            int counter = 0;
            char[,] Arr = new char[Row, col];
            char[,] Arr2 = new char[Row, col];
            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    Arr[j, i] = cipherText[counter];
                    counter++;
                }
            }
            for (int i = 1; i <= col; i++)
            {
                int c = key.IndexOf(i);
                for (int j = 0; j < Row; j++)
                {
                    Arr2[j, c] = Arr[j, i - 1];
                }
            }
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    Plain += Arr2[i, j];
                }
            }
            return Plain;
        }

        public string Encrypt(string plainText, List<int> key)
        {
            string MPlain = Function(plainText, key);
            int Row = MPlain.Length / key.Count;
            int col = key.Count;
            string cipher = "";

            int counter = 0;
            char[,] Arr = new char[Row, col];
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    Arr[i, j] = MPlain[counter];
                    counter++;
                }
            }
            for (int i = 1; i <= col; i++)
            {
                int c = key.IndexOf(i);
                for (int j = 0; j < Row; j++)
                {
                    cipher += Arr[j, c];
                }
            }
            return cipher;
        }
        public string Function(string PT, List<int> Key)
        {
            PT = PT.ToUpper();
            int x = PT.Length;
            if (x % Key.Count == 0)
            {
                return PT;
            }
            else
            {
                while (PT.Length % Key.Count != 0)
                {
                    PT += 'X';
                }
                return PT;
            }
        }
        static int Get_Key_Length(string PT, string CT)
        {
            //int counter = 1;
            int counter2 = 1;
            for (int i = 1; i < CT.Length; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    if (CT[i] != PT[j])
                    {

                        continue;
                    }
                    else
                    {
                        return counter2;
                    }
                }
                counter2++;

            }
            return -1;
        }
    }
}
