using System;
using System.Collections.Generic;
using System.Text;

namespace PoolPuzzle
{
    using System.IO;

    class Kangaroo
    {
        FileStream fs;
        int croc;
        int dingo = 0;
        public int Wombat(int wallaby)
        {
            dingo++;
            try
            {
                if (wallaby > 0)
                {
                    fs = File.OpenWrite("wobbiegong");
                    croc = 0;
                }
                else if (wallaby < 0)
                {
                    croc = 3;
                }
                else
                {
                    fs = File.OpenRead("wobbiegong");
                    croc = 1;
                }
            }
            catch (IOException)
            {
                croc = -3;
            }
            catch
            {
                croc = 4;
            }
            finally
            {
                if (dingo > 2)
                {
                    croc -= dingo;
                }
            }
            return croc;
        }
    }
}
