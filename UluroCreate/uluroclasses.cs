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

    public void createReadPosition(int xpos, int length, int ypos = 0, string delimiter = "", string funcName = "", string param = "")
    {
        //C008	2	"_TEST VAR"	"M"	"P"
        string outStr = "C008" + "	1	" + varName + "	" + varType + "	'P'	";
        //
        switch (funcName.ToUpper())
        {

            case "TRIM":
                outStr = outStr + "'T'\tC0";
                break;
            case "MULT":
            case "MULTIPLY":
                outStr = outStr + "'*'\tC1\tX\t" + param;
                break;
            case "DIV":
            case "DIVISION":
                outStr = outStr + "'/'\tC1\tX\t" + param;
                break;
            case "ADD":
                outStr = outStr + "'+'\tC1\tX\t" + param;
                break;
            case "MOD":
                outStr = outStr + "'!'\tC1\tX\t" + param;
                break;
            default:
                outStr = outStr + "'|'\tC0";
                break;
        }
        //Not sure what this is yet.
        outStr = outStr + "	N	'F'		0		N	0	0	0	0		1N	0	Y	Y";
        outFile.WriteLine(outStr.Replace('\'', '\"'));
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