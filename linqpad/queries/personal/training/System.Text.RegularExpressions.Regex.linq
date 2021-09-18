<Query Kind="Program" />

void Main()
{
	
	var test = "Test {{2}}";
	
	var d = string.Format(test, 45, 45);
	
	
	
	d.Dump();
}

/*
Pattern	Description
	^					Start at the beginning of the string.
	\s*					Match zero or more white-space characters.
	[\+-]?				Match zero or one occurrence of either the positive sign or the negative sign.
	\s?					Match zero or one white-space character.
	\$?					Match zero or one occurrence of the dollar sign.
	\s?					Match zero or one white-space character.
	\d*					Match zero or more decimal digits.
	\.?					Match zero or one decimal point symbol.
	\d{2}?				Match two decimal digits zero or one time.
	(\d*\.?\d{2}?){1}	Match the pattern of integral and fractional digits separated by a decimal point symbol at least one time.
	$					Match the end of the string.

*/
