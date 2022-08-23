using System;
using System.Text;

namespace Derivative;

// ============ToString (for debugging)============= 
public static class ToStringMethods
{
    //String Representation of Lexer
    public static string Repr(this List<Token> list)
    {
        StringBuilder str = new StringBuilder("[]");
        foreach (var item in list)
        {
            str.Insert(str.Length - 1, $" {item} ");
        }
        return str.ToString();
    }
}