using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary
{
    public class Ceaser : ICryptographicTechnique<string, int>
    {
        public string Encrypt(string plainText, int key)
        {
            //throw new NotImplementedException();
            string res = "";
            string ss = plainText.ToUpper();

            for (int i = 0; i < plainText.Length; i++)
            {
                int c = ((ss[i] - 'A') + key) % 26;
                res += (char)('A' + c);
            }
            return res.ToString();

        }

        public string Decrypt(string cipherText, int key)
        {
            //throw new NotImplementedException();
            string res = "";
            string ss = cipherText.ToUpper();

            for (int i = 0; i < cipherText.Length; i++)
            {
                int c = ((ss[i] - 'A') - key) % 26;
                if (c < 0)
                {
                    c += 26;

                }
                res += (char)('A' + c);
            }
            return res.ToString();
        }

        public int Analyse(string plainText, string cipherText)
        {
            //throw new NotImplementedException();
            cipherText = cipherText.ToUpper();
            plainText = plainText.ToUpper();
            int key = cipherText[0] - plainText[0];
            if (key < 0)
            {
                key += 26;
            }
            return key;
        }
    }
}
