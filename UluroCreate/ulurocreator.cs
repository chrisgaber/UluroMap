using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UluroCreate
{
    class program
    {
        static void Main(string[] args)
        {

            StreamWriter outTemp = new StreamWriter(@"c:\temp\testout.txt");
            uText[] tranLines = new uText[6];
            for(int i=0; i < 5; i++)
            {
                tranLines[i] = new uText("Test 1" + i.ToString(), outTemp);
                tranLines[i].createReadPosition(1 + i * 30, 15);
            }

            outTemp.Close();
        }
    }

}
