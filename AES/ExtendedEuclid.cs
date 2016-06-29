using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary.AES
{
    public class ExtendedEuclid 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="baseN"></param>
        /// <returns>Mul inverse, -1 if no inv</returns>
        public int GetMultiplicativeInverse(int number, int baseN)
        {
            return Xtend_GCD(number, baseN);
        }
        public int Xtend_GCD(int m,int n) //should solve the problem m^-1 mod 26 
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

            int Q, A1 = 1, A2 = 0, A3 = n, B1 = 0, B2 = 1, B3 = m;
            int new_Q, new_A1 = 1, new_A2 = 0, new_A3 = n, new_B1 = 0, new_B2 = 1, new_B3 = m;
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
                B2 += n;

            if (B2 > 26)
                B2 = B2 % n;

            return B2;
        }
    }
}
