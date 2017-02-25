using System;
using System.IO;


public class uluroGroup
{
    /*
    Open Group
    C020	"headerName"	N	N	N
    Close Group
    C020	"headerName"	N	N	N
     */
    private string groupName;
    //{
    //    get
    //    {
    //        return groupName;
    //    }
    //    set
    //    {
    //        string test = value;
    //        if (test.Substring(0, 1) != "\"")
    //        {
    //            test = '\"' + test;
    //        }
    //        if (test.Substring(test.Length - 1) != "\"")
    //        {
    //            test = test + '\"';
    //        }

    //        this.groupName = test;

    //    }
    //}
    public bool openState { get; set; }
    public StreamWriter outFile { get; set; }

    public uluroGroup(string name, StreamWriter outPut)
    {
        groupName = "\"" + name + "\"";
        outFile = outPut;
        openState = true;
    }

    public void writeOpen()
    {

        //flip the state and header to be written.
        if (openState)
        {
            openState = false;
        }
        else
        {
            Console.WriteLine("Another instance of uluroGroup " + this.groupName + " was opened before the previous was closed.");
        }

        outFile.WriteLine("C019" + '\t' + this.groupName + "	N	N	N");

	}

    public void writeClose()
    {

        //flip the state and header to be written.
        if (!openState)
        {
            openState = true;
        }
        else
        {
            Console.WriteLine("uluroGroup " + this.groupName + " was closed when it was already closed.");
        }

        outFile.WriteLine("C020" + '\t' + this.groupName + "	N	N	N");

	}
}

public class uluroCond
{
    public uluroCond(StreamWriter outFile)
    {
        this.outFile = outFile;
    }
    public StreamWriter outFile { get; set; }

    private string getCondValue(string test)
    {
        string testCode = "";
        switch (test.ToUpper())
        {
            case "REGEXP":
            case "REGEX":
                testCode = "\"E\"";
                break;
			case "NOT BLANK":
				testCode = "\"X\"";
				break;
			case "BLANK":
            case "EMPTY":
                testCode = "\"0\"";
                break;
            default:
                testCode = "\"" + test + "\"";
                break;
        }
        return testCode;
    }

    private void writeCondition<T, V>(T param1, string test, V param2, bool NOT=false)
    {
        //C012	""	"E"	C2	F	ACCT_ID	X	\d{4,6}-\d{4,6}	N	Y/N	N
        string outString = "C012	\"\"	" + getCondValue(test);

        outString = outString + "\tC2\t" + (param1.GetType() == System.Type.GetType("String") ? "X\t" + param1:"V\t" + param1.ToString()) 
            + "\t" 
            + (param2.GetType() == System.Type.GetType("String") ? "X\t" + param2 : "V\t" + param2.ToString()) 
            + "\tN\t" + (NOT ? "Y" : "N") + "\tN";
        outFile.WriteLine(outString);
    }
    
    //Maybe an easier way to to do this.
    public void createCondition(uluroVariable param1, string test, uluroVariable param2, bool NOT=false)
    {
        writeCondition<uluroVariable, uluroVariable>(param1, test, param2, NOT);
    }
    public void createCondition(string param1, string test, uluroVariable param2, bool NOT = false)
    {
        writeCondition<string, uluroVariable>(param1, test, param2, NOT);
    }
    public void createCondition(uluroVariable param1, string test, string param2, bool NOT = false)
    {
        writeCondition<uluroVariable, string>(param1, test, param2, NOT);
    }
    public void createCondition(string param1, string test, string param2, bool NOT = false)
    {
        writeCondition<string, string>(param1, test, param2, NOT);
    }
    public void createCondition(string param1, string test, bool NOT = false)
    {
        writeCondition<string, string>(param1, test, "", NOT);
    }
    public void createCondition(uluroVariable param1, string test, bool NOT = false)
    {
        writeCondition<uluroVariable, string>(param1, test, "", NOT);
    }

}

public abstract class uluroVariable
{
    public string varType;
    //{
    //    get
    //    {
    //        return varType;
    //    }
    //    set
    //    { 
    //        string test = value;
    //        if (test.Substring(0, 1) != "\"")
    //        {
    //            test = '\"' + test;
    //        }
    //        if (test.Substring(test.Length - 1) != "\"")
    //        {
    //            test = test + '\"';
    //        }

    //        this.varType = test;
    //    }
    //}
    public string varName;
	//{
	//    get
	//    {
	//        return varName;
	//    }
	//    set
	//    {
	//        string test = value;
	//        if (test.Substring(0, 1) != "\"")
	//        {
	//            test = '\"' + test;
	//        }
	//        if (test.Substring(test.Length - 1) != "\"")
	//        {
	//            test = test + '\"';
	//        }

	//        this.varName = test;
	//    }
	//}
	public string mapFont;
    public StreamWriter outFile { get; set; }
    public override string ToString()
    {
        return varName;
    }

    public  void createFixed(string param)
    {
        string outStr = "C008" + "	1	\"" + varName + "\"	\"" + varType + "\"	";
        outStr = outStr + "\"F\"" + '	' + param + '	';
        outStr = outStr + "\"|\"   C0 N   \"F\"     0       N   0   0   0   0   N   0   Y Y";
        outFile.WriteLine(outStr);
    }

    public void createReadPosition(int xpos=1, int length=0, string funcName = "", string param = "", int ypos = 1, string delimiter = "")
    {
        //Var info: header,some int,"name","type","P" (for position)
        //C008	2	"_TEST VAR"	"M"	"P"
        string outStr = "C008" + "	1	\"" + varName + "\"	\"" + varType + "\"	\"P\"	";

        //Input Info:  X, Y, len, delimiter
        //7392	1	30	""	
        if (delimiter.Length > 0)
        {
            delimiter = "\"" + delimiter + "\"";
        }
        outStr = outStr + xpos.ToString() + '\t' + ypos.ToString() + '\t' + length.ToString() + '\t'+ delimiter + '\t';

        //Function info: X, Y, len, delimiter

        switch (funcName.ToUpper())
        {

            case "TRIM":
                if (param.Length == 0)
                {
                    outStr = outStr + "\"T\"\tC0\t" + param;
                }
                else
                {
                    outStr = outStr + "\"T\"\tC1\tX\t" + param;
                }
                break;
            case "MULT":
            case "MULTIPLY":
                outStr = outStr + "\"*\"\tC1\tX\t" + param;
                break;
            case "DIV":
            case "DIVISION":
                outStr = outStr + "\"/\"\tC1\tX\t" + param;
                break;
            case "ADD":
                outStr = outStr + "\"+\"\tC1\tX\t" + param;
                break;
            case "MOD":
                outStr = outStr + "\"!\"\tC1\tX\t" + param;
                break;
            default:
                outStr = outStr + "\""+ param+"\"\tC0\t";
                break;
        }
        //Not sure what this is yet.
        outStr = outStr + "N	\"F\"		0		N	0	0	0	0		1N	0	Y	Y";
        outFile.WriteLine(outStr);
    }

    //Fixed
    public void createMap(float xpos, float ypos, string format = "")
    {
		//C009	0	3.0520	4.3960	"_TEST CURRENCY"	1		Y		L	Y	Y	N	N	N	0									Y
		string outStr = "C009\t0\t";
		outStr = outStr + xpos.ToString("F") + "\t" + ypos.ToString() + "\t\"" + varName + "\"";
		//This part is static.  1 is tied to the var num of the variable, but doesn't seem to matter.
		//special formatting goes at the first Y spot.
		outStr = outStr + "	1	" + format + "	Y		L	Y	Y	N	N	N	0									Y";
		
		outFile.WriteLine(outStr);
	}
	
	//dynamic
	public void createMap(uReal xpos, uReal ypos, string format = "")
	{
		//C009	0	3.0520	4.3960	"_TEST CURRENCY"	1		Y		L	Y	Y	N	N	N	0	V	DESCR X	Y POSITION						Y
		string outStr = "C009\t0\t0.0000\t0.0000\t\"" + varName + "\"";
		//This part is static.  1 is tied to the var num of the variable, but doesn't seem to matter.
		//special formatting goes at the first Y spot.
		outStr = outStr + "	1	"+format+"	Y		L	Y	Y	N	N	N	0";
		outStr = outStr + "	V	" + xpos.varName + "	" + ypos.varName;
		//Not sure what this is.
		outStr = outStr + "						Y";
		outFile.WriteLine(outStr);
	}

	public void setFont()
	{
		//To be worked on in the future.
	}
   
}

public class uDate : uluroVariable
{
    public uDate(string varName, StreamWriter sendTo)
    {
        this.varType = "D";
        this.varName = varName;
        this.outFile = sendTo;
    }
}
public class uCurrency : uluroVariable
{
    public uCurrency(string varName, StreamWriter sendTo)
    {
        this.varType = "M";
        this.varName = varName;
        this.outFile = sendTo;
    }
}
public class uReal : uluroVariable
{
    public uReal(string varName, StreamWriter sendTo)
    {
        this.varType = "R";
        this.varName = varName;
        this.outFile = sendTo;
    }
}
public class uInt : uluroVariable
{
    public uInt(string varName, StreamWriter sendTo)
    {
        this.varType = "I";
        this.varName = varName;
        this.outFile = sendTo;
    }
}
public class uText : uluroVariable
{
    public uText(string varName, StreamWriter sendTo)
    {
        this.varType = "T";
        this.varName = varName;
        this.outFile = sendTo;
    }
}