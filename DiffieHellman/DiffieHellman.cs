using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using SecurityLibrary;

namespace SecurityLibrary.DiffieHellman
{
    public class DiffieHellman 
    {
        public List<int> GetKeys(int q, int alpha, int xa, int xb)
        {
            
            //throw new NotImplementedException();
            List<int> result = new List<int>();
            
            BigInteger Ya = 0, Yb = 0, Ka = 0, Kb = 0;
            BigInteger alpha_ = alpha, q_ = q, xa_ = xa, xb_ = xb;


            Ya = power(Convert.ToUInt64(alpha), Convert.ToUInt64(xa)) % Convert.ToUInt64(q);
            Yb = power(Convert.ToUInt64(alpha), Convert.ToUInt64(xb)) % Convert.ToUInt64(q);

            Ka = power(Yb, Convert.ToUInt64(xa)) % Convert.ToUInt64(q);
            Kb = power(Ya, Convert.ToUInt64(xb)) % Convert.ToUInt64(q);
            byte[] xKA = Ka.ToByteArray();
            byte[] xKB = Kb.ToByteArray();
            result.Add(xKA[0]);
            result.Add(xKB[0]);
            return result;
             

            /*
            int Ya = 0, Yb = 0, Ka = 0, Kb = 0;
            Ya = Get_Power_Mod(alpha, xa, q);
            Yb = Get_Power_Mod(alpha, xb, q);

            Ka = Get_Power_Mod(Yb, xa, q);
            Kb = Get_Power_Mod(Ya, xb, q);

            return new List<int> { Ka, Kb };
             */
        }

        public int Get_Power_Mod(int Rkm, int Power, int Mod)
        {
            int Temp = Power;
            int Temp2 = Power / 4;
            int Temp3 = Power % 4;

            List<double> MyList = new List<double>();
            double Res = 0;
            double Res1 = 1;
            if (Temp3 == 0)
            {
                for (int i = 0; i < Temp2; i++)
                {
                    Res = Math.Pow(Rkm, 4);
                    if (Res <= Mod)
                    {
                        Res = Mod - Res;
                        MyList.Add(Res);
                        Res = 0;
                    }
                    else
                    {
                        Res = Res % Mod;
                        MyList.Add(Res);
                        Res = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Temp2; i++)
                {
                    Res = Math.Pow(Rkm, 4);
                    if (Res <= Mod)
                    {
                        Res = Mod - Res;
                        MyList.Add(Res);
                        Res = 0;
                    }
                    else
                    {
                        Res = Res % Mod;
                        MyList.Add(Res);
                        Res = 0;
                    }
                }
                for (int f = 0; f < Temp3; f++)
                {
                    Res = Math.Pow(Rkm, 1);
                    if (Res <= Mod)
                    {
                        Res = Mod - Res;
                        MyList.Add(Res);
                        Res = 0;
                    }
                    else
                    {
                        Res = Res % Mod;
                        MyList.Add(Res);
                        Res = 0;
                    }
                }
                for (int j = 0; j < MyList.Count; j++)
                {
                    Res1 = (Res1 * MyList[j]);
                    if (Res1 <= Mod)
                    {
                        Res1 = Mod - Res1;
                        //MyList.Add(Res);
                        //Res = 0;
                    }
                    else
                    {
                        Res1 = Res1 % Mod;
                        //MyList.Add(Res);
                        //Res = 0;
                    }
                }
            }
            return (int)Res1;
        }

        private static BigInteger power(BigInteger alpha, BigInteger X)
        {
            BigInteger res = alpha;
            for (BigInteger i = 0; i < X - 1; i++)
            {
                res *= alpha;
            }
            return res;
        }
    }
}
