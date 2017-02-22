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
        groupName = name;
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

        outFile.WriteLine("C019" + '\t' + this.groupName);
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

        outFile.WriteLine("C020" + '\t' + this.groupName);
    }
}

public class uluroCond
{
    public uluroCond(StreamWriter outFile)
    {
        this.outFile = outFile;
    }
    public StreamWriter outFile { get; set; }

    public void createCondition(uluroVariable param1, string test, string param2, bool NOT)
    {
        //C012	""	"E"	C2	F	ACCT_ID	X	\d{4,6}-\d{4,6}	N	Y/N	N
        string outString = "C012	\"\"	";
        switch (test.ToUpper())
        {
            case "REGEXP":
            case "REGEX":
                outString = outString + "\"E\"";
                break;
            default:
                outString = outString + "\""+test+"\"";
                break;
        }
        outString = outString + "";
        outFile.WriteLine(outString);
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

    public StreamWriter outFile { get; set; }

    public  void createFixed(string param)
    {
        string outStr = "C008" + "	1	\"" + varName + "\"	\"" + varType + "\"	\"P\"	";
        outStr = outStr + "\"F\"" + '\t' + param + '\t';
        outStr = outStr + "N	\"F\"		0		N	0	0	0	0		1N	0	Y	Y";
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