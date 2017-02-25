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
            for (int i = 0; i < 5; i++)
            {
                string descr = "PROP CLASS " + (i + 1).ToString();
                tranLines.Add(descr, new uText(descr, outTemp));
                tranLines[descr].createReadPosition(xpos: 7392 + i * 65, length: 30, funcName: "TRIM");

                string actual = "CURR ACTUAL " + (i + 1).ToString();
                tranLines.Add(actual, new uText(actual, outTemp));
                tranLines[actual].createReadPosition(xpos: 7422 + i * 65, length: 15, funcName: "TRIM");

                string taxable = "CURR TAXABLE " + (i + 1).ToString();
                tranLines.Add(taxable, new uText(taxable, outTemp));
                tranLines[taxable].createReadPosition(xpos: 7437 + i * 65, length: 15, funcName: "TRIM");
            }


            uluroCond test = new uluroCond(outTemp);
            uluroGroup wraper = new uluroGroup("COND MAP",outTemp);
			uReal xPos1 = new uReal("XPOS1", outTemp);
			uReal xPos2 = new uReal("XPOS2", outTemp);
			uReal xPos3 = new uReal("XPOS3", outTemp);
			xPos1.createFixed("1");
			xPos2.createFixed("2");
			xPos3.createFixed("3");
			uReal ypos = new uReal("YPOS", outTemp);
			ypos.createFixed("4");
			
            for (int i=0;i<5;i++)
            {
                test.createCondition(tranLines["PROP CLASS " + (i + 1).ToString()], "blank",false);
                wraper.writeOpen();

				string descr = "PROP CLASS " + (i + 1).ToString();
				tranLines[descr].createMap(xPos1, ypos);

				string actual = "CURR ACTUAL " + (i + 1).ToString();
				tranLines[actual].createMap(xPos2, ypos);

				string taxable = "CURR TAXABLE " + (i + 1).ToString();
				tranLines[taxable].createMap(xPos3,ypos);
				outTemp.WriteLine("C011	\"YPOS\"	\"R\"	\" + \"	C2	X	.3	V	YPOS	N	N");
				wraper.writeClose();

            }

            outTemp.Close();
        }
    }

}
