using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary
{
    /// <summary>
    /// The List<int> is row based. Which means
    /// </summary>
    public class HillCipher : ICryptographicTechnique<string, string>, ICryptographicTechnique<List<int>, List<int>>
    {
        #region .
        const string key1="DCIF";
        #endregion .
        public string Analyse3By3Key(string plainText, string cipherText)
        {
            throw new NotImplementedException(); 
        }
        public List<int> Analyse3By3Key(List<int> plainText, List<int> cipherText)
        {
            throw new NotImplementedException();
        }
       
        public List<int> Analyse(List<int> plainText, List<int> cipherText)
        {
            for (int i = 0; i < plainText.Count; i += 2)
            {
                for (int j = i; j < plainText.Count; j += 2)
                {
                    List<int> cipher_matrix = new List<int> { cipherText[i], cipherText[i + 1], cipherText[j], cipherText[j + 1] };
                    List<int> plain_matrix = analysis_PT_inverse(new List<int> { plainText[i], plainText[i + 1], plainText[j], plainText[j + 1] });
                    if (plain_matrix.Count > 1)
                    {
                        List<int> key_matrix = new List<int>(2);
                        key_matrix.Add((cipher_matrix[0] * plain_matrix[0] + cipher_matrix[2] * plain_matrix[1]) % 26);
                        key_matrix.Add((cipher_matrix[0] * plain_matrix[2] + cipher_matrix[2] * plain_matrix[3]) % 26);
                        key_matrix.Add((cipher_matrix[1] * plain_matrix[0] + cipher_matrix[3] * plain_matrix[1]) % 26);
                        key_matrix.Add((cipher_matrix[1] * plain_matrix[2] + cipher_matrix[3] * plain_matrix[3]) % 26);
                        return key_matrix;
                    }
                }
            }
            
            throw new InvalidAnlysisException();
            
        }

        public string Analyse(string plainText, string cipherText)
        {
            List<int> PT = new List<int>();
            List<int> CT = new List<int>();

            plainText = plainText.ToUpper();
            cipherText = cipherText.ToUpper();

            for (int i = 0; i < plainText.Length; i++)
            {
                PT.Add(find_alphabet_index(plainText[i]));
            }

            for (int i = 0; i < cipherText.Length; i++)
            {
                CT.Add(find_alphabet_index(cipherText[i]));
            }

            List<int> key_result = new List<int>();
            key_result = Analyse(PT, CT);

            string key_string = "";
            for (int i = 0; i < key_result.Count; i++)
            {
                key_string += find_alphabet_index(key_result[i]);
            }
            return key_string;
        }

        public List<int> Decrypt(List<int> cipherText, List<int> key)
        {
            string cipher_Text = "", _key = "";
            List<int> plain_result_list = new List<int>();

            for (int i = 0; i < cipherText.Count; i++)
            {
                cipher_Text += find_alphabet_index(cipherText[i]);
            }
            for (int i = 0; i < key.Count; i++)
            {
                _key += find_alphabet_index(key[i]);
            }

            string plain_Text = Decrypt(cipher_Text, _key);
            for (int i = 0; i < plain_Text.Length; i++)
            {
                char c = plain_Text[i];
                plain_result_list.Add(find_alphabet_index(c));
            }
            return plain_result_list;
        }

        public string Decrypt(string cipherText, string key)
        {

            string cipher_Text = cipherText;
            cipher_Text = cipher_Text.ToUpper();

            int depth = (int)Math.Sqrt(key.Length);
            /* //successful
            if (key.Length % 2 == 0)
                depth = 2;
            if (key.Length % 3 == 0)
                depth = 3;
            */
            /* //successful
            int bb = 2;
            while (true)
            {
                if (key.Length % bb == 0)
                { depth = bb; break; }
                bb++;
            }
            */

            int[,] C_T_matrix = new int[depth, cipher_Text.Replace(" ", "").Length / depth];

            int to_loop_cipher_Txt = 0; // 3ayz amla el matrix bta3t el cipher text bel value bta3t el cipher text column-wise
            for (int i = 0; i < cipher_Text.Replace(" ", "").Length / depth; i++)
            {
                for (int j = 0; j < depth; j++)
                {
                    if (cipher_Text[to_loop_cipher_Txt] == ' ') to_loop_cipher_Txt++;

                    if (to_loop_cipher_Txt >= cipher_Text.Length)
                    {
                        C_T_matrix[j, i] = find_alphabet_index('X');
                        to_loop_cipher_Txt++;
                    }
                    else
                    {
                        C_T_matrix[j, i] = find_alphabet_index(cipher_Text[to_loop_cipher_Txt]);
                        to_loop_cipher_Txt++;
                    }
                }
            }


            int m = depth, n = depth;
            /*
            if (m != n)
            {
                Console.WriteLine("Invalid Matrix .. Non Square Matrix!");

            }
            */


            int[,] key_matrix = new int[m, n]; //key matrix
            key = key.ToUpper();
            for (int i = 0; i < m * n; i++)
                key_matrix[i / m, i % m] = find_alphabet_index(key[i]);


            Matrix mat03 = new Matrix(key_matrix);
            int determinant = 0;

            //2X2 matrix inverse 

            if (m == 2 && n == 2)
            {
                determinant = (mat03.matrix[0, 0] * mat03.matrix[1, 1]) - (mat03.matrix[1, 0] * mat03.matrix[0, 1]);
                int swap_temp = 0;
                swap_temp = mat03.matrix[0, 0];
                mat03.matrix[0, 0] = mat03.matrix[1, 1];
                mat03.matrix[1, 1] = swap_temp;
                mat03.matrix[0, 1] *= -1;
                mat03.matrix[1, 0] *= -1;

            }
            else if (m > 2 && n > 2) //larger than 2x2 square matrix
            {
                //Apply det
                //determinant = mat03.deterMatrix();

                for (int i = 0; i < mat03.countColumns(); i++) //they are 3 columns
                {
                    determinant += (int)Math.Pow(-1, i) * mat03.matrix[0, i] * mat03.minor_twoXtwo_matrix_deter(mat03.matrix, 0, i);
                } 
                 
            }

            if (determinant == 0)
            {
                Console.WriteLine("Invalid Matrix .. Non Invertible");

            }
            while (determinant < 0)
                determinant += 26;
            
            //get b value //check section 2 cryptography

            int xtended_GCD_determinant = Xtend_GCD(determinant);


            if (m == 2 && n == 2) //2*2
            {
                for (int i = 0; i < mat03.countRows(); i++)
                {
                    for (int j = 0; j < mat03.countColumns(); j++)
                    {
                        while (mat03.matrix[i, j] < 0)
                            mat03.matrix[i, j] += 26;

                        mat03.matrix[i, j] *= xtended_GCD_determinant;
                        mat03.matrix[i, j] %= 26;
                    }
                }
            }

            else //3*3
            {
                int[,] temp_final_matrix = new int[mat03.countRows(), mat03.countColumns()];
                for (int i = 0; i < mat03.countRows(); i++)
                {
                    for (int j = 0; j < mat03.countColumns(); j++)
                    {
                        //Apply rule kij ={b x (-1)i+j * Dij mod 26} mod 26 //Dij=>indicates determinant of ij
                        temp_final_matrix[i, j] = (xtended_GCD_determinant * (int)Math.Pow(-1, i + j) * mat03.minor_twoXtwo_matrix_deter(mat03.matrix, i, j)) % 26;
                        while (temp_final_matrix[i, j] < 0)
                            temp_final_matrix[i, j] += 26;
                    }
                }
                //transpose the matrix
                for (int i = 0; i < mat03.countRows(); i++)
                {
                    for (int j = 0; j < mat03.countColumns(); j++)
                    {
                        mat03.matrix[i, j] = temp_final_matrix[j, i];
                    }
                }
            }
            int[,] result_matrix = matrix_multiplication(mat03.matrix, C_T_matrix);

            //view result matrix as plain text
            char[,] plain_matrix = new char[result_matrix.GetLength(0), result_matrix.GetLength(1)];
            plain_matrix = cipher_text_fun(result_matrix);
            return view_cipher_as_plain(plain_matrix, cipher_Text);



        }

        public List<int> Encrypt(List<int> plainText, List<int> key)
        {
            string plain_text = "", _key = "";
            List<int> cipher_result_list = new List<int>();
            for (int i = 0; i < plainText.Count; i++)
            {
                plain_text += find_alphabet_index(plainText[i]);
            }
            for (int i = 0; i < key.Count; i++)
            {
                _key += find_alphabet_index(key[i]);
            }

            string cipher_text = Encrypt(plain_text, _key);
            for (int i = 0; i < cipher_text.Length; i++)
            {
                char c = cipher_text[i];
                cipher_result_list.Add(find_alphabet_index(c));
            }
            return cipher_result_list;
        }

        public string Encrypt(string plainText, string key)
        {

            string Plain_Text = plainText;
            Plain_Text = Plain_Text.ToUpper();
            int depth = (int)Math.Sqrt(key.Length);

            /*//successful
            int bb = 2;
            while (true)
            {
                if (key.Length % bb == 0)
                { depth = bb; break; }
                bb++;
            }
            */


            int[,] P_T_matrix = new int[depth, Plain_Text.Replace(" ", "").Length / depth]; //the length of 

            int to_loop_Plain_Txt = 0; // 3ayz amla el matrix bta3t el plain text bel value bta3t el plain text column-wise
            for (int i = 0; i < Plain_Text.Replace(" ", "").Length / depth; i++)
            {
                for (int j = 0; j < depth; j++)
                {
                    if (Plain_Text[to_loop_Plain_Txt] == ' ') to_loop_Plain_Txt++;

                    if (to_loop_Plain_Txt >= Plain_Text.Length)
                    {
                        P_T_matrix[j, i] = find_alphabet_index('X');
                        to_loop_Plain_Txt++;
                    }
                    else
                    {
                        P_T_matrix[j, i] = find_alphabet_index(Plain_Text[to_loop_Plain_Txt]);
                        to_loop_Plain_Txt++;
                    }
                }
            }


            int m = depth, n = depth;


            key = key.ToUpper();
            int[,] key_matrix = new int[m, n]; //key matrix
            for (int i = 0; i < m * n; i++)
                key_matrix[i / m, i % m] = find_alphabet_index(key[i]); //function linear

            //if 2 matrices are nXn and pXm and n != p //invalid operation
            if (key_matrix.GetLength(1) != P_T_matrix.GetLength(0))
            {
                return "Invalid Matrices!";
            }
            else
            {
                int[,] result_matrix = new int[P_T_matrix.GetLength(0), key_matrix.GetLength(1)];
                result_matrix = matrix_multiplication(key_matrix, P_T_matrix);

                char[,] cipher_matrix = new char[result_matrix.GetLength(0), result_matrix.GetLength(1)];
                cipher_matrix = cipher_text_fun(result_matrix);

                return view_cipher_as_plain(cipher_matrix, Plain_Text);
            }



        }






        #region my_methods


        public List<int> analysis_PT_inverse(List<int> key)
        {
            List<int> resultant_Key = new List<int>();
            int det = (key[0] * key[3] - key[1] * key[2]) % 26;
            while (det < 0) 
                det += 26;
            int b = 0; 

            for (int i = 2; i < 26; i++) //check what is in the range of 26 if there is a multiplicative inverse of determinant
            {
                if (((i * det) % 26) == 1)
                {
                    b = i;
                    break;
                }
            }

            if (det != 0 && b != 0)
            { //2X2 matrix inverse is ==> b * [a,b,c,d] -> b * [d,-b,-c,a]
                resultant_Key.Add(b * key[3]);
                resultant_Key.Add(b * (-1) * key[1]);
                resultant_Key.Add(b * (-1) * key[2]);
                resultant_Key.Add(b * key[0]);
                //normalize the data into the 26 range 
                for (int m = 0; m < resultant_Key.Count; m++)
                {
                    resultant_Key[m] = resultant_Key[m] % 26;
                    while (resultant_Key[m] < 0)
                        resultant_Key[m] += 26;
                }
                return resultant_Key;
            }
            else
            {
                return new List<int> { -1 };
            }

        }
        public int Xtend_GCD(int m) //should solve the problem m^-1 mod 26 
        {
            /* //rules
             * Q=(int)A3/B3
             * A1 = B1(i-1)
             * A2 = B2(i-1)
             * A3 = B3(i-1)
             * B1 = A1(i-1) - Q(i)B1(i-1)
             * B2 = A2(i-1) - Q(i)B2(i-1)
             * B3 = A3(i-1) % B3(i-1) //stop when B3 = 1
             */

            int Q, A1 = 1, A2 = 0, A3 = 26, B1 = 0, B2 = 1, B3 = m;
            int new_Q, new_A1 = 1, new_A2 = 0, new_A3 = 26, new_B1 = 0, new_B2 = 1, new_B3 = m;
            while (true)
            {
                if (B3 == 1 || B3 == 0) //termination condition
                    break;
                new_Q = A3 / B3;
                new_A1 = B1;
                new_A2 = B2;
                new_A3 = B3;
                new_B1 = A1 - (new_Q * B1);
                new_B2 = A2 - (new_Q * B2);
                new_B3 = A3 % B3;

                Q = new_Q;
                A1 = new_A1;
                A2 = new_A2;
                A3 = new_A3;
                B1 = new_B1;
                B2 = new_B2;
                B3 = new_B3;

            }
            while (B2 < 0)
                B2 += 26;

            if (B2 > 26)
                B2 = B2 % 26;

            return B2;
        }



        //check //https://www.youtube.com/watch?v=XJfdMGQVmaA //for matrix multiplication methodology used
        public int[,] matrix_multiplication(int[,] A, int[,] B)
        {
            int A_len = A.Length, B_len = B.Length;
            
            int[,] result_matrix = new int[A.GetLength(0), B.GetLength(1)]; //a matrix with size of the first matrix rows and the second matrix columns
            int dimension_Len = A.GetLength(1); //to be the stopping condition of each riemann sum between a row in first matrix and a column in the second matrix

            for (int i = 0; i < result_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < result_matrix.GetLength(1); j++)
                {
                    result_matrix[i, j] = find_matrix_elem_val(j, i, A, B, dimension_Len) % 26;
                    //Console.Write(result_matrix[i, j] + " ");
                }
                //Console.WriteLine();
            }
            return result_matrix;
        }

        public int find_matrix_elem_val(int row, int col, int[,] A, int[,] B, int dimension_Len)
        {
            int summation = 0;
            for (int i = 0; i < dimension_Len; i++)
            {
                summation += A[col, i] * B[i, row];
            }
            return summation;
        }

        //to find the character index in the alphabet
        public int find_alphabet_index(char c)
        {
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(); //an easy way to make an alphabet char array map.
            for (int i = 0; i < alpha.Length; i++)
                if (alpha[i] == c) return i;
            return -1; // if it's invalid character for instance
        }

        public char find_alphabet_index(int index)
        {
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(); //an easy way to make an alphabet char array map.
            return alpha[index];
        }


        public char[,] cipher_text_fun(int[,] result_matrix)
        {
            char[,] cipher_matrix = new char[result_matrix.GetLength(0), result_matrix.GetLength(1)];
            for (int i = 0; i < cipher_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < cipher_matrix.GetLength(1); j++)
                {
                    cipher_matrix[i, j] = find_alphabet_index(result_matrix[i, j]);
                    //Console.Write(cipher_matrix[j, i] + " ");
                }

            }

            return cipher_matrix;

        }

        public string view_cipher_as_plain(char[,] cipher_text, string plain_text)
        {
            string final_output_cipher = "";
            int P_T_count = 0;

            for (int i = 0; i < cipher_text.GetLength(1); i++)
                for (int j = 0; j < cipher_text.GetLength(0); j++)
                {
                    if (plain_text[P_T_count] == ' ')
                        final_output_cipher += " " + cipher_text[j, i];
                    else
                        final_output_cipher += cipher_text[j, i];
                    P_T_count++;
                }

            return final_output_cipher;
        }

        #endregion my_methods

        #region Matrix_class 
        public class Matrix
        {
            private int row_matrix; //number of rows for matrix
            private int column_matrix; //number of colums for matrix
            public int[,] matrix; //holds values of matrix itself

            //create r*c matrix and fill it with data passed to this constructor
            public Matrix(int[,] double_array)
            {
                matrix = double_array;
                row_matrix = matrix.GetLength(0);
                column_matrix = matrix.GetLength(1);
                Console.WriteLine("Contructor which sets matrix size {0}*{1} and fill it with initial data executed.", row_matrix, column_matrix);
            }

            //returns total number of rows
            public int countRows()
            {
                return row_matrix;
            }

            //returns total number of columns
            public int countColumns()
            {
                return column_matrix;
            }

            //returns value of an element for a given row and column of matrix
            public int readElement(int row, int column)
            {
                return matrix[row, column];
            }

            //sets value of an element for a given row and column of matrix
            public void setElement(int value, int row, int column)
            {
                matrix[row, column] = value;
            }

            /*
            public int deterMatrix()
            {
                int det = 0;
                int value = 0;
                int i, j, k;

                i = row_matrix;
                j = column_matrix;
                int n = i;

                if (i != j)
                {
                    Console.WriteLine("determinant can be calculated only for sqaure matrix!");
                    return det;
                }

                for (i = 0; i < n - 1; i++)
                {
                    for (j = i + 1; j < n; j++)
                    {
                        det = (this.readElement(j, i) / this.readElement(i, i));

                        for (k = i; k < n; k++)
                        {
                            value = this.readElement(j, k) - det * this.readElement(i, k);

                            this.setElement(value, j, k);
                        }
                    }
                }
                det = 1;
                for (i = 0; i < n; i++)
                    det = det * this.readElement(i, i);

                return det;
            }
            */


            public int minor_twoXtwo_matrix_deter(int[,] m, int row, int column)
            {
                int[,] temp_2x2_mat = new int[2, 2];
                int temp_i = 0, temp_j = 0;
                
                for (int i = 0; i < m.GetLength(0); i++)
                {
                    if (i == row) continue;
                    for (int j = 0; j < m.GetLength(1); j++)
                    {
                        if (j == column) continue;
                        temp_2x2_mat[temp_i, temp_j] = m[i, j];

                        temp_j++;
                    }
                    temp_j = 0;
                    temp_i++;
                }

                //minor matrix determinant
                int deter = ((temp_2x2_mat[0, 0] * temp_2x2_mat[1, 1]) - (temp_2x2_mat[0, 1] * temp_2x2_mat[1, 0]));
                return deter;
            }
        }
        #endregion Matrix_class
    }
}
