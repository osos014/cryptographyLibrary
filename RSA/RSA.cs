using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLibrary.RSA
{
    public class RSA
    {
        public int Encrypt(int p, int q, int M, int e)
        {
            //throw new NotImplementedException();
            int N = p * q;
            double dummy = Math.Pow(M, e);
            double C = dummy % N;
            return (int)C;
        }

        public int Decrypt(int p, int q, int C, int e)
        {
            //throw new NotImplementedException();
            int QN = (p - 1) * (q - 1);
            int D = Get_Inverse(e, QN);

            int M = Get_Power_Mod(C, D, p * q);

            //BigInteger  m = BigInteger.ModPow(C,D,p*q);
            return (int)M;
        }
        public int Get_Inverse(int M, int N)
        {
            int[,] arr = new int[2, 7];
            arr[0, 0] = 0;
            arr[0, 1] = 1;
            arr[0, 2] = 0;
            arr[0, 3] = N;
            arr[0, 4] = 0;
            arr[0, 5] = 1;
            arr[0, 6] = M;
            arr[1, 0] = N / M;
            arr[1, 1] = arr[0, 4];
            arr[1, 2] = arr[0, 5];
            arr[1, 3] = arr[0, 6];
            arr[1, 4] = arr[0, 1] - (arr[1, 0] * arr[0, 4]);
            arr[1, 5] = arr[0, 2] - (arr[1, 0] * arr[0, 5]);
            arr[1, 6] = N % M;
            if (arr[1, 6] == 1 || arr[1, 6] == 0)
            {
                while (arr[1, 5] < 0)
                    arr[1, 5] += N;
                return arr[1, 5];
            }
            else
            {
                while (arr[1, 6] != 1 && arr[1, 6] != 0)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        arr[0, i] = arr[1, i];
                    }
                    arr[1, 0] = arr[0, 3] / arr[0, 6];
                    arr[1, 1] = arr[0, 4];
                    arr[1, 2] = arr[0, 5];
                    arr[1, 3] = arr[0, 6];
                    arr[1, 4] = arr[0, 1] - (arr[1, 0] * arr[0, 4]);
                    arr[1, 5] = arr[0, 2] - (arr[1, 0] * arr[0, 5]);
                    arr[1, 6] = arr[0, 3] % arr[0, 6];

                }
                while (arr[1, 5] < 0)
                    arr[1, 5] += N;
                return arr[1, 5];

            }
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
                    Res *= Math.Pow(Rkm, 2);
                    if (Res <= Mod)
                    {
                        Res = Mod - Res;
                    }
                    else
                    {
                        Res = Res % Mod;
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
    }
}
