using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary
{
    public class AES_Os
    {
        private int number_of_rounds;   // number of rounds. 10, 12, 14.
        private int block_size;        // block size in 32-bit words.  Always 4 for AES.  (128 bits).
        private int key_size;         // key size in 32-bit words.  4, 6, 8.  (128, 192, 256 bits).

        public enum KeySize { Bits128, Bits192, Bits256 };  // key size, in bits, for construtor


        private byte[,] key;     // the seed key. size will be 4 * keySize from ctor.
        private byte[] Sbox;   // Substitution box
        private byte[] iSbox;  // inverse Substitution box  
        private byte[,] Rcon;   // Round constants.
        private byte[,] State;  // State matrix //carries final output of each round.
        private byte[,] key_schedule_array;

        public AES_Os(byte[,] keybytes, KeySize keysize)
        {
            Set_blockSize_keySize_numberOfRounds(keysize);
            this.key = new byte[this.key_size, 4]; //creating my key as a 4 * 4 2D array for instance
            this.key = keybytes;

            Rcon_Build();
            static_RijandelInverse_array_direct_access(); //build Sbox
            BuildInvSbox();
            Key_Expansion();


        }

        public byte[,] Encrypt(byte[,] input)
        {
            this.State = new byte[4, 4]; //always 4*4 input 128-bit Plain Text
            this.State = input;

            //first round
            AddRoundKey(0);

            //main rounds
            for (int round = 1; round <= number_of_rounds - 1; round++)
            {
                SubBytes();
                this.State = shift_rows(this.State);
                this.State = mix_columns(this.State);
                AddRoundKey(round);
            }

            //last round
            SubBytes();
            this.State = shift_rows(this.State);
            AddRoundKey(number_of_rounds);

            return this.State;
        } //encryption
        public byte[,] Decrypt(byte[,] input)
        {
            this.State = new byte[4, 4];
            this.State = input;

            //first round
            AddRoundKey(number_of_rounds);

            //main rounds
            for (int round = number_of_rounds - 1; round > 0; round--)
            {
                inverse_shift_rows();
                Inverse_Sub_bytes();
                AddRoundKey(round);
                inv_mixColumns();
            }

            //last round
            inverse_shift_rows();
            Inverse_Sub_bytes();
            AddRoundKey(0);

            return this.State;
        } //Decryption
        private void AddRoundKey(int round)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.State[i, j] = (byte)((int)this.State[i, j] ^ (int)this.key_schedule_array[i, (round * 4) + j]);
                }
            }
        }
        private void Set_blockSize_keySize_numberOfRounds(KeySize keySize)
        {
            this.block_size = 4;     // block size always = 4 words = 16 bytes = 128 bits for AES

            if (keySize == KeySize.Bits128)
            {
                this.key_size = 4;   // key size = 4 words = 16 bytes = 128 bits
                this.number_of_rounds = 10;  // rounds for algorithm = 10
            }
            else if (keySize == KeySize.Bits192)
            {
                this.key_size = 6;   // 6 words = 24 bytes = 192 bits
                this.number_of_rounds = 12;
            }
            else if (keySize == KeySize.Bits256)
            {
                this.key_size = 8;   // 8 words = 32 bytes = 256 bits
                this.number_of_rounds = 14;
            }
        }  // SetNbNkNr()

        #region sub-bytes
        private void SubBytes()
        {
            for (int r = 0; r < 4; ++r)
            {
                for (int c = 0; c < 4; ++c)
                {
                    this.State[r, c] = this.Sbox[this.State[r, c]];
                }
            }
        }  // SubBytes

        #region s-box
        public void static_RijandelInverse_array_direct_access()
        {
            this.Sbox = new byte[] 
                     {
                        0x63, 0x7C, 0x77, 0x7B, 0xF2, 0x6B, 0x6F, 0xC5, 0x30, 0x01, 0x67, 0x2B, 0xFE, 0xD7, 0xAB, 0x76,
                        0xCA, 0x82, 0xC9, 0x7D, 0xFA, 0x59, 0x47, 0xF0, 0xAD, 0xD4, 0xA2, 0xAF, 0x9C, 0xA4, 0x72, 0xC0,
                        0xB7, 0xFD, 0x93, 0x26, 0x36, 0x3F, 0xF7, 0xCC, 0x34, 0xA5, 0xE5, 0xF1, 0x71, 0xD8, 0x31, 0x15,
                        0x04, 0xC7, 0x23, 0xC3, 0x18, 0x96, 0x05, 0x9A, 0x07, 0x12, 0x80, 0xE2, 0xEB, 0x27, 0xB2, 0x75,
                        0x09, 0x83, 0x2C, 0x1A, 0x1B, 0x6E, 0x5A, 0xA0, 0x52, 0x3B, 0xD6, 0xB3, 0x29, 0xE3, 0x2F, 0x84,
                        0x53, 0xD1, 0x00, 0xED, 0x20, 0xFC, 0xB1, 0x5B, 0x6A, 0xCB, 0xBE, 0x39, 0x4A, 0x4C, 0x58, 0xCF,
                        0xD0, 0xEF, 0xAA, 0xFB, 0x43, 0x4D, 0x33, 0x85, 0x45, 0xF9, 0x02, 0x7F, 0x50, 0x3C, 0x9F, 0xA8,
                        0x51, 0xA3, 0x40, 0x8F, 0x92, 0x9D, 0x38, 0xF5, 0xBC, 0xB6, 0xDA, 0x21, 0x10, 0xFF, 0xF3, 0xD2,
                        0xCD, 0x0C, 0x13, 0xEC, 0x5F, 0x97, 0x44, 0x17, 0xC4, 0xA7, 0x7E, 0x3D, 0x64, 0x5D, 0x19, 0x73,
                        0x60, 0x81, 0x4F, 0xDC, 0x22, 0x2A, 0x90, 0x88, 0x46, 0xEE, 0xB8, 0x14, 0xDE, 0x5E, 0x0B, 0xDB,
                        0xE0, 0x32, 0x3A, 0x0A, 0x49, 0x06, 0x24, 0x5C, 0xC2, 0xD3, 0xAC, 0x62, 0x91, 0x95, 0xE4, 0x79,
                        0xE7, 0xC8, 0x37, 0x6D, 0x8D, 0xD5, 0x4E, 0xA9, 0x6C, 0x56, 0xF4, 0xEA, 0x65, 0x7A, 0xAE, 0x08,
                        0xBA, 0x78, 0x25, 0x2E, 0x1C, 0xA6, 0xB4, 0xC6, 0xE8, 0xDD, 0x74, 0x1F, 0x4B, 0xBD, 0x8B, 0x8A,
                        0x70, 0x3E, 0xB5, 0x66, 0x48, 0x03, 0xF6, 0x0E, 0x61, 0x35, 0x57, 0xB9, 0x86, 0xC1, 0x1D, 0x9E,
                        0xE1, 0xF8, 0x98, 0x11, 0x69, 0xD9, 0x8E, 0x94, 0x9B, 0x1E, 0x87, 0xE9, 0xCE, 0x55, 0x28, 0xDF,
                        0x8C, 0xA1, 0x89, 0x0D, 0xBF, 0xE6, 0x42, 0x68, 0x41, 0x99, 0x2D, 0x0F, 0xB0, 0x54, 0xBB, 0x16
                     };
        }


        //this to get the inverse of the input after changing it from HEX to DECimal .. i.e "53 inverse = "CA 
        //reference : http://crypto.stackexchange.com/questions/26732/calculating-multiplicative-inverse-for-rijndael-s-box-using-eea
        //reference : http://crypto.stackexchange.com/questions/10996/how-are-the-aes-s-boxes-calculated
        public static uint RijndaelInverse(uint a)
        {
            uint old_s = 0; uint s = 1; uint new_s = 0;
            uint old_t = 0; uint t = 0; uint new_t = 1;
            uint old_r = 0x11B; uint r = 0x11B; uint new_r = a;

            while (new_r > 0)
            {
                var r_msb = (int)Math.Log(r, 2.0);
                var new_r_msb = (int)Math.Log(new_r, 2.0);

                int quotient = r_msb - new_r_msb;

                if (quotient >= 0)
                {
                    old_s = s;
                    s = new_s;
                    new_s = old_s ^ (s << quotient);

                    old_t = t;
                    t = new_t;
                    new_t = old_t ^ (t << quotient);

                    old_r = r;
                    r = new_r;
                    new_r = old_r ^ (r << quotient);
                }
                else
                {
                    new_s = s ^ new_s;
                    s = s ^ new_s;
                    new_s = s ^ new_s;

                    new_t = t ^ new_t;
                    t = t ^ new_t;
                    new_t = t ^ new_t;

                    new_r = r ^ new_r;
                    r = r ^ new_r;
                    new_r = r ^ new_r;
                }
            }

            if (r > 1) return 0;

            if (t > 0xFF) t ^= 0x11B;

            return t;
        } //to be completed as a whole s-box generation first method
        #endregion s-box
        #endregion sub-bytes


        #region shift_rows
        public byte[,] shift_rows(byte[,] A)
        {
            byte[,] resultant_matrix = new byte[A.GetLength(0), A.GetLength(1)];
            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < A.GetLength(1); j++)
                {
                    resultant_matrix[i, j] = A[i, (j + i) % 4];
                }
            }
            return resultant_matrix;
        }
        #endregion shift_rows


        #region mix_columns
        //reference : http://crypto.stackexchange.com/questions/2402/how-to-solve-mixcolumns
        public byte[,] mix_columns(byte[,] A)
        {

            byte[,] result_matrix = new byte[A.GetLength(0), A.GetLength(1)]; //kkont 3amlha 4,4
            for (byte i = 0; i < A.GetLength(0); i++) //kant i<4
            {
                for (byte j = 0; j < A.GetLength(1); j++) //kant j<4
                {
                    result_matrix[i, j] = find_rijandel_mix_columns_value(i, j, A); //if int .. the returned result % 256
                }
            }

            return result_matrix;
        }

        public byte find_rijandel_mix_columns_value(byte row, byte col, byte[,] A)
        {
            byte result = 0;
            byte temp = 0; //for the left shifts and MSBit check 
            byte _1B = 0x1B;
            bool MSB_Set = false;
            byte[,] rijandel_matrix = new byte[4, 4]
                {
                    {2,3,1,1},
                    {1,2,3,1},
                    {1,1,2,3},
                    {3,1,1,2}
                };

            for (int i = 0; i < 4; i++)
            {
                MSB_Set = false;
                if (rijandel_matrix[row, i] == 1)
                {
                    result ^= A[i, col];
                }
                else if (rijandel_matrix[row, i] == 2) //reference : check if MSB is set //http://stackoverflow.com/questions/2431732/checking-if-a-bit-is-set-or-not
                {
                    if ((A[i, col] & (1 << 7)) != 0) //checking if MSB is set //MSB is 7th bit //1 << 7 is like putting 1 at the 7th bit and the rest is zero then anding with my value
                        MSB_Set = true;

                    temp = (byte)(A[i, col] << 1);
                    if (MSB_Set)
                    {
                        result ^= (byte)(temp ^ _1B);
                    }
                    else
                    {
                        result ^= temp;
                    }

                }
                else if (rijandel_matrix[row, i] == 3)
                {
                    if ((A[i, col] & (1 << 7)) != 0) //checking if MSB is set //MSB is 7th bit //1 << 7 is like putting 1 at the 7th bit and the rest is zero then anding with my value
                        MSB_Set = true;

                    temp = (byte)(A[i, col] << 1);
                    if (MSB_Set)
                    {
                        result ^= (byte)(temp ^ _1B ^ A[i, col]);
                    }
                    else
                    {
                        result ^= (byte)(temp ^ A[i, col]);
                    }

                }
                //result ^= rijandel_matrix[row, i] + A[i, col];
            }
            return result;
        }
        #endregion mix_columns


        #region key_expansion

        public void Key_Expansion()
        {
            this.key_schedule_array = new byte[4, (number_of_rounds + 1) * block_size]; //40 columns * 4 rows for example in 128-bit encryption // the + 1 is because the key it self is included in that array alongwith the round keys

            for (int i = 0; i < key_size; i++) //copying the key to the first 4 column of the key schedule
            {
                key_schedule_array[i, 0] = key[i, 0];
                key_schedule_array[i, 1] = key[i, 1];
                key_schedule_array[i, 2] = key[i, 2];
                key_schedule_array[i, 3] = key[i, 3];
            }

            byte[,] temp = new byte[4, 1]; //to store temporarily the current word 

            for (int col = this.key_size; col < (number_of_rounds + 1) * block_size; col++) //loops through the whole expansion
            {
                temp[0, 0] = this.key_schedule_array[0, col - 1];
                temp[1, 0] = this.key_schedule_array[1, col - 1];
                temp[2, 0] = this.key_schedule_array[2, col - 1];
                temp[3, 0] = this.key_schedule_array[3, col - 1];

                if (col % key_size == 0)
                {
                    //Rotate Word
                    temp = Rotate_Word(temp);

                    //substitue word
                    temp[0, 0] = this.Sbox[temp[0, 0]];
                    temp[1, 0] = this.Sbox[temp[1, 0]];
                    temp[2, 0] = this.Sbox[temp[2, 0]];
                    temp[3, 0] = this.Sbox[temp[3, 0]];

                    //exclusive ORing with Rcon
                    temp[0, 0] = (byte)((int)temp[0, 0] ^ (int)this.Rcon[0, col / key_size]);
                    temp[1, 0] = (byte)((int)temp[1, 0] ^ (int)this.Rcon[1, col / key_size]);
                    temp[2, 0] = (byte)((int)temp[2, 0] ^ (int)this.Rcon[2, col / key_size]);
                    temp[3, 0] = (byte)((int)temp[3, 0] ^ (int)this.Rcon[3, col / key_size]);

                }
                //exclusive ORing with W(i-4)
                this.key_schedule_array[0, col] = (byte)((int)this.key_schedule_array[0, col - key_size] ^ (int)temp[0, 0]);
                this.key_schedule_array[1, col] = (byte)((int)this.key_schedule_array[1, col - key_size] ^ (int)temp[1, 0]);
                this.key_schedule_array[2, col] = (byte)((int)this.key_schedule_array[2, col - key_size] ^ (int)temp[2, 0]);
                this.key_schedule_array[3, col] = (byte)((int)this.key_schedule_array[3, col - key_size] ^ (int)temp[3, 0]);
            }


        }
        private byte[,] Rotate_Word(byte[,] A)
        {
            byte[,] result = new byte[4, 1];
            result[0, 0] = A[1, 0];
            result[1, 0] = A[2, 0];
            result[2, 0] = A[3, 0];
            result[3, 0] = A[0, 0];

            return result;
        }



        #endregion key_expansion


        #region Inverse_SBytes
        private void BuildInvSbox()
        {
            this.iSbox = new byte[] {  // populate the iSbox matrix
    /* 0     1     2     3     4     5     6     7     8     9     a     b     c     d     e     f */
    /*0*/  0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb,
    /*1*/  0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb,
    /*2*/  0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e,
    /*3*/  0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25,
    /*4*/  0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92,
    /*5*/  0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84,
    /*6*/  0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a, 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06,
    /*7*/  0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b,
    /*8*/  0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73,
    /*9*/  0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e,
    /*a*/  0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b,
    /*b*/  0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4,
    /*c*/  0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f,
    /*d*/  0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef,
    /*e*/  0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61,
    /*f*/  0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d };

        }  // BuildInvSbox()
        private void Inverse_Sub_bytes()
        {
            for (int r = 0; r < 4; ++r)
            {
                for (int c = 0; c < 4; ++c)
                {
                    this.State[r, c] = this.iSbox[this.State[r, c]];
                }
            }
        }

        #endregion Inverse_SBytes


        #region inverse_mix_Columns
        private void inv_mixColumns()
        {
            byte[,] temp = new byte[this.block_size, this.block_size];

            for (int r = 0; r < 4; ++r)  // copy State into temp[]
            {
                for (int c = 0; c < 4; ++c)
                {
                    temp[r, c] = this.State[r, c];
                }
            }

            for (int col = 0; col < 4; col++)
            {
                this.State[0, col] = (byte)((int)mixColumsMulBy0e(temp[0, col]) ^ (int)mixColumsMulBy0b(temp[1, col]) ^ (int)mixColumsMulBy0d(temp[2, col]) ^ (int)mixColumsMulBy09(temp[3, col]));
                this.State[1, col] = (byte)((int)mixColumsMulBy09(temp[0, col]) ^ (int)mixColumsMulBy0e(temp[1, col]) ^ (int)mixColumsMulBy0b(temp[2, col]) ^ (int)mixColumsMulBy0d(temp[3, col]));
                this.State[2, col] = (byte)((int)mixColumsMulBy0d(temp[0, col]) ^ (int)mixColumsMulBy09(temp[1, col]) ^ (int)mixColumsMulBy0e(temp[2, col]) ^ (int)mixColumsMulBy0b(temp[3, col]));
                this.State[3, col] = (byte)((int)mixColumsMulBy0b(temp[0, col]) ^ (int)mixColumsMulBy0d(temp[1, col]) ^ (int)mixColumsMulBy09(temp[2, col]) ^ (int)mixColumsMulBy0e(temp[3, col]));
            }
        }
        #endregion inverse_mix_Columns


        #region inverse_shift_rows
        private void inverse_shift_rows()
        {

            byte[,] temp = new byte[4, 4];

            for (int r = 0; r < 4; ++r)  // copy State into temp[]
            {
                for (int c = 0; c < 4; ++c)
                {
                    temp[r, c] = this.State[r, c];
                }
            }

            for (int row = 1; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    this.State[row, (row + col) % this.block_size] = temp[row, col]; //block size = 4
                }
            }

        }
        #endregion inverse_shift_rows

        public void Rcon_Build()
        {
            this.Rcon = new byte[4, 11]
            {
                {0x00,0x01,0x02,0x04,0x08,0x10,0x20,0x40,0x80,0x1b,0x36},
                {0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00},
                {0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00},
                {0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}
            };
        }


        #region Galios-Field-matricies
        private static byte mixColumsMulBy01(byte b)
        {
            return b;
        }

        private static byte mixColumsMulBy02(byte b)
        {
            if (b < 0x80) //high bit is set ? //if true means not set
                return (byte)(int)(b << 1);
            else
                return (byte)((int)(b << 1) ^ (int)(0x1b));
        } //the most important one

        private static byte mixColumsMulBy03(byte b)
        {
            return (byte)((int)mixColumsMulBy02(b) ^ (int)b);
        }

        private static byte mixColumsMulBy09(byte b)
        {
            return (byte)((int)mixColumsMulBy02(mixColumsMulBy02(mixColumsMulBy02(b))) ^ (int)b);  //it's like 2*2*2*1 -or- 2 xor 2 xor 2 xor 1
        }

        private static byte mixColumsMulBy0b(byte b)
        {
            return (byte)((int)mixColumsMulBy02(mixColumsMulBy02(mixColumsMulBy02(b))) ^
                           (int)mixColumsMulBy02(b) ^
                           (int)b);
        }

        private static byte mixColumsMulBy0d(byte b)
        {
            return (byte)((int)mixColumsMulBy02(mixColumsMulBy02(mixColumsMulBy02(b))) ^
                           (int)mixColumsMulBy02(mixColumsMulBy02(b)) ^
                           (int)(b));
        }

        private static byte mixColumsMulBy0e(byte b)
        {
            return (byte)((int)mixColumsMulBy02(mixColumsMulBy02(mixColumsMulBy02(b))) ^
                           (int)mixColumsMulBy02(mixColumsMulBy02(b)) ^
                           (int)mixColumsMulBy02(b));
        }

        #endregion Galios-Field-matricies
    }
}
