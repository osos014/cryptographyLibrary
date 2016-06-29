using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary.AES
{
    /// <summary>
    /// If the string starts with 0x.... then it's Hexadecimal not string
    /// </summary>
    public class AES : CryptographicTechnique
    {
        public override string Decrypt(string cipherText, string key)
        {
            byte[,] keyBytes = new byte[4, 4];
            byte[,] CipherBytes = new byte[4, 4];

            if (key[0] == '0' && key[1] == 'x')
            {
                for (int i = 0; i < 4 * 4; i++) //copying Keystring to my byte array
                {
                    string temp = key[i + (i + 2)] + "" + key[i + (i + 3)];
                    keyBytes[i % 4, i / 4] = Convert.ToByte(temp, 16);
                }
            }

            if (cipherText[0] == '0' && cipherText[1] == 'x')
            {
                for (int i = 0; i < 4 * 4; i++) //copying PTstring to my byte array
                {
                    string temp = cipherText[i + (i + 2)] + "" + cipherText[i + (i + 3)];
                    CipherBytes[i % 4, i / 4] = Convert.ToByte(temp, 16);
                }
            }
            AES_Os my_aes = new AES_Os(keyBytes, AES_Os.KeySize.Bits128);
            CipherBytes = my_aes.Decrypt(CipherBytes);

            string Decrypted_Message = "0x";
            for (int i = 0; i < 4 * 4; i++)
            {
                Decrypted_Message += CipherBytes[i % 4, i / 4].ToString("X2");
            }
            return Decrypted_Message;

        }

        public override string Encrypt(string plainText, string key)
        {


            byte[,] keyBytes = new byte[4, 4];
            byte[,] PlainBytes = new byte[4, 4];

            if (key[0] == '0' && key[1] == 'x')
            {
                for (int i = 0; i < 4 * 4; i++) //copying Keystring to my byte array
                {
                    string temp = key[i + (i + 2)] + "" + key[i + (i + 3)];
                    keyBytes[i % 4, i / 4] = Convert.ToByte(temp, 16);
                }
            }

            if (plainText[0] == '0' && plainText[1] == 'x')
            {
                for (int i = 0; i < 4 * 4; i++) //copying PTstring to my byte array
                {
                    string temp = plainText[i + (i + 2)] + "" + plainText[i + (i + 3)];
                    PlainBytes[i % 4, i / 4] = Convert.ToByte(temp, 16);
                }
            }

            AES_Os my_aes = new AES_Os(keyBytes, AES_Os.KeySize.Bits128);
            PlainBytes = my_aes.Encrypt(PlainBytes);
            string encrypted_message = "0x";
            for (int i = 0; i < 4 * 4; i++)
            {
                encrypted_message += PlainBytes[i % 4, i / 4].ToString("X2");
            }

            return encrypted_message;

        }
    }
}
