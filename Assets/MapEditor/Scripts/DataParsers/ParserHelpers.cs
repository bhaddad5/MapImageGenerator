using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ParserHelpers {

	public static string ClearOutComments(string file)
	{
		return RemoveBetween(file, "//", "\n");
	}

	private static string RemoveBetween(string s, string begin, string end)
	{
		Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
		return regex.Replace(s, string.Empty);
	}
}
