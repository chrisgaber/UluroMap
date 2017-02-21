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
    private string groupName
    {
        get;
        set
        {
            if (value.Substring(1, 1) != '\"')
            {
                value = '\"' + value;
            }
            if (value.Substring(value.Length - 1) != '\"')
            {
                value = value + '\"';
            }
        }
    }
    public bool openState { get; }
    public StreamWriter outFile { get; set; }

    public uluroGroup (string name, StreamWriter outPut)
    {
        groupName = name;
        outFile = outPut;
        openState = true;
    }

    private string writeOpen()
    {

        //flip the state and header to be written.
        if (openState)
        {
            openState = false;
            headerName = "C019";
        }
        else
        {
            Console.WriteLine("Another instance of uluroGroup " + this.groupName + " was opened before the previous was closed.");
        }

        outFile.WriteLine("C020" + '\t' + this.groupName);
    }

    private string writeClose()
    {

        //flip the state and header to be written.
        if (!openState)
        {
            openState = true;
            headerName = "C020";
        }
        else
        {
            Console.WriteLine("uluroGroup " + this.groupName + " was closed when it was already closed.");
        }

        outFile.WriteLine("C020" + '\t' + this.groupName);
    }
}


abstract class uluroVariable
{
    private string varType
    {
        get;
        set
        {
            {
                if (value.Substring(1, 1) != '\"')
                {
                    value = '\"' + value;
                }
                if (value.Substring(value.Length - 1) != '\"')
                {
                    value = value + '\"';
                }
            }
        }
    }
    private string varName
    {
        get;
        set
        {
            if (value.Substring(1, 1) != '\"')
            {
                value = '\"' + value;
            }
            if (value.Substring(value.Length - 1) != '\"')
            {
                value = value + '\"';
            }
        }
    }

    public StreamWriter outFile { get; set; }

    public void createReadPosition(int xpos, int length, int ypos = 0, string delimiter = "", string funcName ="", string param)
    {
        //C008	2	"_TEST VAR"	"M"	"P"
        string outStr = "C008" + "	1	" + varName + "	" + varType + "	'P'	";
        //
        switch (funcName.ToUpper)
        {

            case "TRIM":
                outStr = outStr + "'T'\tC0";
            case "MULT":
            case "MULTIPLY":
                outStr = outStr + "'*'\tC1\tX\t" + param;
            case "DIV":
            case "DIVISION":
                outStr = outStr + "'/'\tC1\tX\t" + param;
            case "ADD":
                outStr = outStr + "'+'\tC1\tX\t" + param;
            case "MOD":
                outStr = outStr + "'!'\tC1\tX\t" + param;
            default:
                outStr = outStr + "'|'\tC0";
        }
        //Not sure what this is yet.
        outStr = outStr + "	N	'F'		0		N	0	0	0	0		1N	0	Y	Y";
        outFile.WriteLine(outStr.Replace("\'", '\"'));
    }
      
}

public class uDate:uluroVariable
{
    public uDate(string varName, StreamWriter sendTo)
    {
        this.varType = "\"D\"";
        this.varName = varName;
        this.outFile = sendTo;
    }
}
public class uCurrency : uluroVariable
{
    public uCurrency()
    {
        this.varType = "\"M\"";
        this.varName = varName;
        this.outFile = sendTo;
    }
}
public class uReal : uluroVariable
{
    public uReal(string varName, StreamWriter sendTo)
    {
        this.varType = "\"R\"";
        this.varName = varName;
        this.outFile = sendTo;
    }
}
public class uInt : uluroVariable
{
    public uInt(string varName, StreamWriter sendTo)
    {
        this.varType = "\"I\"";
        this.varName = varName;
        this.outFile = sendTo;
    }
}
public class uText : uluroVariable
{
    public uText(string varName, StreamWriter sendTo)
    {
        this.varType = "\"T";
        this.varName = varName;
        this.outFile = sendTo;
    }
}
