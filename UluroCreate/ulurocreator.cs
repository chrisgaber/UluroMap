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
            Dictionary<string, uText> tranLines = new Dictionary<string, uText>();
            for(int i=0; i < 5; i++)
            {
                string descr = "PROP CLASS " + (i+1).ToString();
                tranLines.Add(descr, new uText(descr, outTemp));
                tranLines[descr].createReadPosition(xpos:7392 + i * 65, length: 30,funcName:"TRIM");

                string actual = "CURR ACTUAL " + (i+1).ToString();
                tranLines.Add(actual, new uText(actual, outTemp));
                tranLines[actual].createReadPosition(xpos: 7422 + i * 65, length:15,funcName:"TRIM");

                string taxable = "CURR TAXABLE " + (i+1).ToString();
                tranLines.Add(taxable, new uText(taxable, outTemp));
                tranLines[taxable].createReadPosition(xpos:7437 + i * 65, length:15, funcName:"TRIM");
            }

            //for(int i=0; i<5;i++)
            //{

            //}

            outTemp.Close();
        }
    }

}
