using UnityEngine;
using System.Collections.Generic;
using EnumLibrary;
using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[Serializable]
public class Function //Wraps up values to form a special list
{
	[XmlAttribute("Function_Name")]
	public string
		fName;
	[XmlArray("Function_Arguments"), XmlArrayItem("Function_Argument")]
	public List<Argument>
		fArguments = new List<Argument> ();
	[XmlArray("Function_Calls"), XmlArrayItem("Function_Call")]
	public List<FunctionCalls>
		fCalls = new List<FunctionCalls> (); //Final calls based off argument going negative (-1 = 1, -2 = 2 so on).
	[XmlArray("Function_Parameters"), XmlArrayItem("Function_Parameter")]
	public List<int>
		fParameters = new List<int> (); //Parameters for ^^
	[XmlAttribute("Function_Before_Message")]
	public string
		fBeforeMessage;
	[XmlAttribute("Function_After_Message")]
	public string
		fAfterMessage;

	public Function ()
	{
	}

	public Function (string name, FunctionCalls[] calls, List<int> parameters, Argument[] arguments, string beforeMessage, string afterMessage)
	{
		fName = name;
		fArguments = arguments.ToList ();
		fParameters = parameters;
		fCalls = calls.ToList ();
		fBeforeMessage = beforeMessage;
		fAfterMessage = afterMessage;
	}
}

[Serializable]
public class Argument //Special Statement that includes continue, function to call and priority
{
	[XmlAttribute("Argument_Name")]
	public string
		aName;
	[XmlArray("Argument_Values"), XmlArrayItem("Argument_Value")]
	public List<Value>
		aValues = new List<Value> ();
	[XmlAttribute("Argument_TrueCall")]
	public int
		aTrueID;
	[XmlAttribute("Argument_FalseCall")]
	public int
		aFalseID;
	[XmlIgnore()]
	public int
		aCallerID;

	public Argument ()
	{
	}

	public Argument (string name, Value[] values, int trueID, int falseID)
	{
		aName = name;
		aValues = values.ToList ();
		aTrueID = trueID;
		aFalseID = falseID;
	}
}

[Serializable]
public class Value //Returns a value for use in a function or argument
{
	[XmlAttribute("Value_Name")]
	public string
		vName;
	[XmlAttribute("Value_1")]
	public Values
		vValue1;
	[XmlAttribute("Value_Sign")]
	public signs
		vSign;
	[XmlAttribute("Value_2")]
	public Values
		vValue2;
	[XmlAttribute("Value_Number")]
	public double
		vValueNumber;
	[XmlAttribute("Value_String")]
	public string
		vValueString;
	[XmlIgnore()]
	public int
		vCallerID; //Look up as if value was an argument

	public Value (signs sign, string name, Values value1, Values value2, double number = 0, string text = "")
	{
		vName = name;
		vSign = sign;
		vValueNumber = number;
		vValueString = text;

		vValue1 = value1;
		vValue2 = value2;
	}

	public Value ()
	{
	}
}

[XmlRoot("Master_Node_Function"), XmlInclude(typeof(Function)), XmlInclude(typeof(Value)), XmlInclude(typeof(Argument))]
public class XMLSerializerFunction
{
	[XmlElement("Function")]
	public Function
		function;

	public void Save (string path)
	{
		var serializer = new XmlSerializer (typeof(XMLSerializerFunction));
		using (var stream = new FileStream(path, FileMode.Create))
			serializer.Serialize (stream, this);
	}

	public static XMLSerializerFunction Load (string path)
	{
		var serializer = new XmlSerializer (typeof(XMLSerializerFunction));
		using (var stream = new FileStream(path, FileMode.Open))
			return (XMLSerializerFunction)serializer.Deserialize (stream) as XMLSerializerFunction;
	}
}