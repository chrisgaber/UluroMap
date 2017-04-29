using System;
using System.IO;

public class uMirror
{
	private StreamWriter _outFile;
	private StreamReader _mirrorFrom;
	private string _insertTag;
	public uMirror(string getFrom, string GroupName, StreamWriter outFile)
	{
		_mirrorFrom = new StreamReader(getFrom);
		_insertTag = GroupName;
		_outFile = outFile;
	}

	public void setGroup(string GroupName)
	{
		_insertTag = GroupName;
	}

	public void mirrorToGroup()
	{
		string line = "";
		while(!_mirrorFrom.EndOfStream && !(line = _mirrorFrom.ReadLine()).StartsWith($"C019\t\"+{_insertTag}"))
		{
			_outFile.WriteLine(line);
		}

		if (!_mirrorFrom.EndOfStream)
			_outFile.WriteLine(line); //Write the tag itself.
	}

	public void mirrorAfterGroup()
	{
		string line = "";
		while (!_mirrorFrom.EndOfStream && !(line = _mirrorFrom.ReadLine()).StartsWith($"C020\t\"+{_insertTag}"))
		{
			continue;
		}

		_outFile.WriteLine(line);

		while (!_mirrorFrom.EndOfStream)
		{
			line = _mirrorFrom.ReadLine();
			_outFile.WriteLine(line);
		}
	}
}

public class uluroGroup
{

    public StreamWriter outFile { get; set; }

    public uluroGroup(StreamWriter outPut)
    {
        outFile = outPut;
    }

    public void writeOpen( string name)
    {

        outFile.WriteLine($"C019\t\"{name}\"\tN\tN\tN");

	}

	public void writeClose(string name)
	{

		outFile.WriteLine($"C020\t\"{name}\"\tN\tN\tN");

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
			case "NOT EMPTY":
				testCode = "\"0\"";
				break;
			case "BLANK":
            case "EMPTY":
                testCode = "\"X\"";
                break;
            default:
                testCode = $"\"{test}\"";
                break;
        }
        return testCode;
    }

    private void writeCondition<T, V>(T param1, string test, V param2, bool isTrue)
    {
        //C012	""	"E"	C2	F	ACCT_ID	X	\d{4,6}-\d{4,6}	N	Y/N	N

        //Need the magic string for the test
        string testType = $"C012\t\"\"\t{getCondValue(test)}";

        //Format parameters based on if they are static or variable.
        string firstParam = $"\tC2\t{(param1.GetType() == System.Type.GetType("String") ? "X\t" + param1 : "V\t" + param1.ToString())}";
        string secondParam = $"\t{(param2.GetType() == System.Type.GetType("String") ? "X\t" + param2 : "V\t" + param2.ToString())}"; 

        //There is a check box for NOT(thetest).  I forced this 
        string notBox = $"\tN\t{(isTrue ? "N" : "Y")}\tN";  //Counter intuitive, but this is for the NOT check box.

        outFile.WriteLine($"{testType}{firstParam}{secondParam}{notBox}");
    }
    
    //Maybe an easier way to to do this.
    public void createCondition(uluroVariable param1, string test, uluroVariable param2, bool isTrue)
    {
        writeCondition<uluroVariable, uluroVariable>(param1, test, param2, isTrue);
    }
    public void createCondition(string param1, string test, uluroVariable param2, bool isTrue)
    {
        writeCondition<string, uluroVariable>(param1, test, param2, isTrue);
    }
    public void createCondition(uluroVariable param1, string test, string param2, bool isTrue)
    {
        writeCondition<uluroVariable, string>(param1, test, param2, isTrue);
    }
    public void createCondition(string param1, string test, string param2, bool isTrue)
    {
        writeCondition<string, string>(param1, test, param2, isTrue);
    }
    public void createCondition(string param1, string test, bool isTrue)
    {
        writeCondition<string, string>(param1, test, "", isTrue);
    }
    public void createCondition(uluroVariable param1, string test, bool isTrue)
    {
        writeCondition<uluroVariable, string>(param1, test, "", isTrue);
    }

}

public class uluroFuncWriter
{
	StreamWriter outFile;

	public uluroFuncWriter(StreamWriter outFile)
	{
		this.outFile = outFile;
	}

    public void createFunc(uluroVariable assignTo, string func, params object[] input)
    {
        var paramTot = input.Length;
        
        //"C011	\"YPOS\"	\"R\"	\"+\"	C2	X	.3	V	YPOS	N	N"
        //C011	"resultvar"	"resultType"	"+" C(paramcount)	X/V VAL/PARAMNAME	N	N

        string outStr = $"C011\t\"{assignTo.varName}\"\t\"{assignTo.varType}\"\t\"{func}\"\tC{paramTot.ToString()}";

        for (int i = 0; i < paramTot; i++)
        {
            if (input[i].GetType() == typeof(String))
            {
                outStr += $"\tX\t{input[i].ToString()}";
            }
            else if (input[i].GetType() == typeof(uluroVariable))
            {
                outStr +=   $"\tV\t{input[i].ToString()}";
            }
        }

        //Add the magic trailing vars.
        outFile.WriteLine($"{outStr}\tN\tN");
    }
}

public abstract class uluroVariable
{
    public string varType;

    public string varName;

	public string mapFont;
    public StreamWriter outFile { get; set; }
    public override string ToString()
    {
        return varName;
    }

    public  void createFixed(string param)
    {
        string outStr = $"C008\t1\t\"{varName}\"\t\"{varType}\"\t\"F\"\t{param}\t\"|\"   C0 N   \"F\"     0       N   0   0   0   0   N   0   Y Y";
        outFile.WriteLine(outStr);
    }

    public void createReadPosition(int xpos=1, int length=0, string funcName = "", string param = "", int ypos = 1, string delimiter = "")
    {
        //Var info: header,some int,"name","type","P" (for position)
        //C008	2	"_TEST VAR"	"M"	"P"
        string outStr = $"C008\t{1}\t\"{varName}\"\t\"{varType}\"\t\"P\"\t";

        //Input Info:  X, Y, len, delimiter
        //7392	1	30	""	
        if (delimiter.Length > 0)
        {
            delimiter = "\"" + delimiter + "\"";
        }
        outStr += $"{xpos.ToString()}\t{ypos.ToString()}\t{length.ToString()}\t{delimiter}\t";

        //Function info: X, Y, len, delimiter
        switch (funcName.ToUpper())
        {

            case "TRIM":
                if (param.Length == 0)
                {
                    outStr += $"\"T\"\tC0\t{param}";
                }
                else
                {
                    outStr += $"\"T\"\tC1\tX\t{param}";
                }
                break;
            case "MULT":
            case "MULTIPLY":
                outStr += $"\"*\"\tC1\tX\t{param}";
                break;
            case "DIV":
            case "DIVISION":
                outStr += $"\"/\"\tC1\tX\t{param}";
                break;
            case "ADD":
                outStr += $"\"+\"\tC1\tX\t{param}";
                break;
            case "MOD":
                outStr += $"\"!\"\tC1\tX\t{param}";
                break;
            default:
                outStr += $"\"{param}\"\tC0\t";
                break;
        }
        //Not sure what this is yet.
        outStr += "N\t\"F\"\t\t0\t\tN\t0\t0\t0\t0\t\t1N\t0\tY\tY";
        outFile.WriteLine(outStr);
    }

	private string setFont(string alignment = "L", string fontName = "Arial", string fontSize = "10", string fontStyle = "")
	{
		//N	"Arial"	8	TY	0	"BOLD ITALIC UNDERLINE STRIKEOUT "	L
		string fontString = $"N\t\"{fontName}\"\t{fontSize}\tTY\t0\t\"{ fontStyle}\"\t{alignment}";

		return fontString;
	}

	private void genericMapWriter(float xpos, float ypos, string varName, string format, string fontstring, string varString = "			")
	{
		//C009	0	3.0520	4.3960	"_TEST CURRENCY"	1		Y		L	Y	Y	N	N	N	0	V	DESCR X	Y POSITION						Y
		string genericFormula = $"C009\t0\t{xpos.ToString()}\t{ypos.ToString()}\t\"{varName}\"";
		//This part is static.  1 is tied to the var num of the variable, but doesn't seem to matter.
		//special formatting goes at the first Y spot.
		genericFormula += $"\t1\t{format}\t{fontstring}\tY\tY\tN\tN\tN\t0{varString}\t\t\t\t\t\tY";
		outFile.WriteLine(genericFormula);
	}

	public void createMap(uReal xpos, uReal ypos, string format = "", string alignment = "L", string fontName = "Arial", string fontSize = "10", string fontStyle = "")
	{
		string fontStr = setFont(alignment, fontName, fontSize, fontStyle);
		string varStr = $"\tV\t{xpos.varName}\t{ypos.varName}";
		genericMapWriter(0.0000f, 0.0000f, this.varName, format, fontStr, varStr);
	}

	public void createMap(float xpos, float ypos, string format = "", string alignment = "L", string fontName = "Arial", string fontSize = "10", string fontStyle = "")
	{
		string fontStr = setFont(alignment, fontName, fontSize, fontStyle);
		genericMapWriter(xpos, ypos, this.varName, format, fontStr);
	}

}

//This holds font info and maps variables to it.  Its possible to call the mapper directly on the uVariables if desired.  Just repetative.
public class uMapper
{
	private string _fontName;
	private string _fontSize;
	private string _fontStyle;
	private string _alignment;

	public uMapper(string alignment = "L", string fontName = "Arial", string fontSize = "10", string fontStyle = "")
	{
		_alignment = alignment;
		_fontName = fontName;
		_fontSize = fontSize;
		_fontStyle = fontStyle.ToUpper();
	}           

	public void resetFonts(string alignment = "L", string fontName = "Arial", string fontSize = "10", string fontStyle = "")
	{
		_alignment = alignment;
		_fontName = fontName;
		_fontSize = fontSize;
		_fontStyle = fontStyle.ToUpper();
	}



	public void createMap(uluroVariable mapThis, uReal xpos, uReal ypos, string format = "")
	{
		mapThis.createMap(xpos, ypos, format, _alignment, _fontName, _fontSize, _fontStyle);
	}


	public void createMap(uluroVariable mapThis, float xpos, float ypos, string format = "")
	{
		mapThis.createMap(xpos, ypos, format, _alignment, _fontName, _fontSize, _fontStyle);
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