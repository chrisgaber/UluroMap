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

            StreamWriter outTemp = new StreamWriter(@"c:\temp\testout.nmp");

			uMirror da3 = new uMirror(@"U:\SUBMIT\MAPS\DACDA5\da5.nmp", "MID TRANS", outTemp);

			da3.mirrorToGroup();


			uluroCond test = new uluroCond(outTemp);
			uluroFuncWriter ufunc = new uluroFuncWriter(outTemp);
			uluroGroup group = new uluroGroup(outTemp);

			uMapper col1 = new uMapper(alignment: "L", fontSize: "8");
			uMapper col2 = new uMapper(alignment: "C", fontSize: "8");
			uMapper col3 = new uMapper(alignment: "C", fontSize: "8");

			uReal xMid1 = new uReal("XMID1", outTemp);
			uReal xMid15 = new uReal("XMID1.5", outTemp);
			uReal xMid2 = new uReal("XMID2", outTemp);
			uReal xMid3 = new uReal("XMID3", outTemp);

			uReal ypos = new uReal("YPOS", outTemp);
			uReal ydelta = new uReal("YDELTA", outTemp);



			Dictionary<string, uText> tranLines = new Dictionary<string, uText>();
            for (int i = 0; i < 12; i++)
            {
                string descr = "PROP CLASS " + (i + 1).ToString();
                tranLines.Add(descr, new uText(descr, outTemp));

                string actual = "CURR ACTUAL " + (i + 1).ToString();
                tranLines.Add(actual, new uText(actual, outTemp));
                //tranLines[actual].createReadPosition(xpos: 7422 + i * 65, length: 15, funcName: "TRIM");

                string taxable = "CURR TAXABLE " + (i + 1).ToString();
                tranLines.Add(taxable, new uText(taxable, outTemp));
                //tranLines[taxable].createReadPosition(xpos: 7437 + i * 65, length: 15, funcName: "TRIM");

				string units = "UNITS " + (i + 1).ToString();
				tranLines.Add(units, new uText(units, outTemp));
				//tranLines[units].createReadPosition(xpos: 7452 + i * 65, length: 5, funcName: "TRIM");
			}

			for (int i = 0; i < 12; i++)
			{
				string descr = "PROP CLASS " + (i + 1).ToString();
				test.createCondition(tranLines[descr], "not blank", true);
				group.writeOpen("MAP " + descr);
				col1.createMap(tranLines[descr], xMid1, ypos);

				string units = "UNITS " + (i + 1).ToString();
				col1.createMap(tranLines[units], xMid15, ypos);

				string actual = "CURR ACTUAL " + (i + 1).ToString();
				col2.createMap(tranLines[actual], xMid2, ypos);

				string taxable = "CURR TAXABLE " + (i + 1).ToString();
				col3.createMap(tranLines[taxable], xMid3, ypos);

				ufunc.createFunc(ypos, "+", ypos, ydelta);

				group.writeClose("MAP " + descr);
			}
			da3.mirrorAfterGroup();

            outTemp.Close();
        }

    }

}
