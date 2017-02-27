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

			uMirror da3 = new uMirror(@"U:\SUBMIT\MAPS\DACDA3\da3.nmp", "DETAILS",outTemp);

			da3.mirrorToGroup();

            Dictionary<string, uText> tranLines = new Dictionary<string, uText>();
            for (int i = 0; i < 5; i++)
            {
                string descr = "PROP CLASS " + (i + 1).ToString();
                tranLines.Add(descr, new uText(descr, outTemp));
                //tranLines[descr].createReadPosition(xpos: 7392 + i * 65, length: 30, funcName: "TRIM");

                string actual = "CURR ACTUAL " + (i + 1).ToString();
                tranLines.Add(actual, new uText(actual, outTemp));
                //tranLines[actual].createReadPosition(xpos: 7422 + i * 65, length: 15, funcName: "TRIM");

                string taxable = "CURR TAXABLE " + (i + 1).ToString();
                tranLines.Add(taxable, new uText(taxable, outTemp));
                //tranLines[taxable].createReadPosition(xpos: 7437 + i * 65, length: 15, funcName: "TRIM");
            }

			//Mapping part 1
            uluroCond test = new uluroCond(outTemp);
			uluroFuncWriter ufunc = new uluroFuncWriter(outTemp);
			uluroGroup group = new uluroGroup(outTemp);

			uMapper col1 = new uMapper(alignment: "R", fontSize: "8", fontStyle: "BOLD ");
			uMapper col2 = new uMapper(alignment: "L", fontSize: "8");
			uMapper col3 = new uMapper(alignment: "C", fontSize: "8");

			uReal ypos = new uReal("YPOS", outTemp);
			uReal ydelta = new uReal("YDELTA", outTemp);

			ypos.createFixed("4.35");
			ydelta.createFixed(".175");
			
			//Mapping part 1
			group.writeOpen("TOP TRANS");
			uReal xTop1 = new uReal("XTOP1", outTemp);
			uReal xTop2 = new uReal("XTOP2", outTemp);
			xTop1.createFixed("1.4");
			xTop2.createFixed("1.5");
			string[] outDescr = { "Account Type:", "Neighborhood:", "Situs Address:", "Legal Descr:" };//, "Zoning:", "TotalAcreage:"};
			string[] outVal = { "_ACCT TYPE", "_NEIGHBORHOOD", "_SITUS", "_LEGAL LINE 1" };//, "_PROPERTY USE"};
			for (int i = 0; i < outDescr.Length; i++)
			{
				uText udescr = new uText(outDescr[i].Replace(":", ""), outTemp);
				uText uVal = new uText(outVal[i], outTemp);

				test.createCondition(uVal.varName, "not EMPTY",true);
				group.writeOpen("MAP " + outVal[i]);

				udescr.createFixed(outDescr[i]);
				col1.createMap(udescr,xTop1, ypos);
				col2.createMap(uVal,xTop2, ypos);
				ufunc.createFunc(ypos, "+", ypos, ydelta);
				group.writeClose("MAP " + outVal[i]);
				
			}
			group.writeClose("TOP TRANS");

			//Mapping Part 2
			ufunc.createFunc(ypos, "+", ypos, ydelta);//Extra space
			uReal xMid1 = new uReal("XMID1", outTemp);
			uReal xMid15 = new uReal("XMID1.5", outTemp);
			uReal xMid2 = new uReal("XMID2", outTemp);
			uReal xMid3 = new uReal("XMID3", outTemp);
			xMid1.createFixed("1.75");
			xMid15.createFixed("3.5");
			xMid2.createFixed("6.15");
			xMid3.createFixed("7.56");

			group.writeOpen("MID TRANS");
			
			col1.resetFonts(alignment: "L", fontSize: "8");
			col2.resetFonts(alignment: "C", fontSize: "8");
			col3.resetFonts(alignment: "C", fontSize: "8");
			for (int i=0;i<5;i++)
            {
                test.createCondition(tranLines["PROP CLASS " + (i + 1).ToString()], "not blank",true);
                group.writeOpen("PROP CLASS " + (i + 1).ToString());

				string descr = "PROP CLASS " + (i + 1).ToString();
				col1.createMap(tranLines[descr],xMid1, ypos);

				string actual = "CURR ACTUAL " + (i + 1).ToString();
				col2.createMap(tranLines[actual], xMid2, ypos);

				string taxable = "CURR TAXABLE " + (i + 1).ToString();
				col3.createMap(tranLines[taxable], xMid3, ypos);
				group.writeClose("PROP CLASS " + (i + 1).ToString());

            }

			ufunc.createFunc(ypos, "+", ypos, ydelta);//Extra space

			uText currenttotal = new uText("SUBTOTAL",outTemp);
			currenttotal.createFixed("Current Year’s Total Value");
			col1.createMap(currenttotal, xMid1, ypos);
			ufunc.createFunc(ypos, "+", ypos, ydelta);
			group.writeClose("MID TRANS");
		

			//Mapping the closing part
			ufunc.createFunc(ypos, "+", ypos, ydelta);//Extra space
			group.writeOpen("ADJUSTMENTS");

			uText ADJUSTHDR = new uText("ADJUSTHDR", outTemp);
			ADJUSTHDR.createFixed("Adjustments");
			col1.createMap(ADJUSTHDR, xMid1, ypos);
			ufunc.createFunc(ypos, "+", ypos, ydelta);

			string[] adjust = { "_HOH PROP CLASS", "_VETERAN PROP CLASS", "_DIS VETERAN PROP CLASS", "_TOTAL ADJ (CALC)" };
			string[] adjustval = { "_HOH TAXABLE", "_VETERAN TAXABLE", "_DIS VETERAN TAXABLE","_TOTAL ADJ TAXABLE" };//, "_PROPERTY USE"};
			for (int i = 0; i < outDescr.Length; i++)
			{
				uText udescr = new uText(adjust[i], outTemp);
				uText uVal = new uText(adjustval[i], outTemp);

				test.createCondition(adjust[i], "not EMPTY", true);
				group.writeOpen("MAP " + adjust[i]);
	
				col1.createMap(udescr, xMid1, ypos);
				col2.createMap(uVal, xMid3, ypos);
				ufunc.createFunc(ypos, "+", ypos, ydelta);
				group.writeClose("MAP " + outVal[i]);

			}

			uText ADJUSTTOT = new uText("ADJUST TOTAL", outTemp);
			ADJUSTHDR.createFixed("Total Adjustments");
			col1.createMap(ADJUSTTOT, xMid15, ypos);
			ufunc.createFunc(ypos, "+", ypos, ydelta);

			group.writeClose("ADJUSTMENTS");
			
			//tOTAL

			uText total = new uText("TOTAL TOTAL", outTemp);
			total.createFixed("Net Taxable Value");
			col1.createMap(total, xMid15, ypos);
			ufunc.createFunc(ypos, "+", ypos, ydelta);

			da3.mirrorAfterGroup();

            outTemp.Close();
        }

    }

}
