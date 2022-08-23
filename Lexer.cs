using System;

namespace Derivative;

// ============TOKENS============= 
public class Token
{
    // OPERATOR TYPES
    public const string ADD = "ADD";
    public const string SUB = "SUB";
    public const string MUL = "MUL";
    public const string DIV = "DIV";
    public const string EXP = "EXP";
    public const string LOG = "LOG";
    public const string LN  = "LN";
    public const string SIN = "SIN";
    public const string COS = "COS";
    public const string TAN = "TAN";
    public const string CSC = "CSC";
    public const string SEC = "SEC";
    public const string COT = "COT";    
    public const string LPAREN = "LPAREN";
    public const string RPAREN = "RPAREN";

    public string type {get; set;}
    public string value {get; set;}
    public Token(string type = "", string value = "")
    {
        this.type = type;
        this.value = value;
    }

    public override string ToString()
    {
        if (value != "") 
        { return $"{type}:{value}"; }
        else 
        { return type; }
    }
}


// ============LEXER============= 
class Lexer
{
    string? text;
    char currentchar, prevchar;
    int pos = -1;
    public Lexer(string text)
    {
        this.text = text;
        next_char();
    }

    public void next_char(int num = 1)
    {
        pos += num;
        currentchar = pos < text!.Length ? text[pos] : '\0';
        prevchar = pos == 0 ? '\0' : text[pos-1];
    }

    public int setlen(int len) //helper function
    {
        while (len > text!.Length - pos)
        { len -= 1; }
        return len;
    }
    public Token make_number()
    {
        int period = 0;
        string numstr = "";
        while (number.Contains(currentchar) || currentchar == '.' )
        {
            if (currentchar == '.')
            {
                if (period > 1) {break;}
                period += 1;
                numstr += '.';
                next_char();
            }
            else
            {
                numstr += currentchar;
                next_char();
            }
        }

        if (period == 0)
        {   return new Token("INT", numstr); }
        else
        {   return new Token("FLOAT", numstr); }
    }

    public Token make_constant()
    {
        if (currentchar == 'p') //pi
        {
            next_char();
            if (currentchar == 'i')
            { return new Token("CONST" , "pi"); }
            else
            { throw new ArgumentOutOfRangeException(nameof(currentchar), $"Invalid Character: {currentchar}"); }
        }
        else                    //e
        {   return new Token("CONST" , "e"); }
    }

    public Token make_trig()  
    {
        string trig = text!.Substring(pos,setlen(3));
        return trig switch
        {
            "sin" => new Token(Token.SIN),
            "cos" => new Token(Token.COS),
            "tan" => new Token(Token.TAN),
            "csc" => new Token(Token.CSC),
            "sec" => new Token(Token.SEC),
            "cot" => new Token(Token.COT),
            _ => throw new ArgumentOutOfRangeException(nameof(currentchar), $"Invalid Character: {currentchar}")
        };
    }

    // LEXER | TOKEN GENERATOR
    public List<Token> make_tokens()
    {
        List<Token> tokens = new List<Token>();
        while (currentchar != '\0')
        {
            if (currentchar is ' ')
            { next_char(); }
            else if (number.Contains(currentchar))
            { 
                tokens.Add(make_number());
                if (constants.Contains(currentchar)
                    || variable.Contains(currentchar)
                    || trigs.Contains(text!.Substring(pos, setlen(3)))
                    || text!.Substring(pos, setlen(2)) == "ln"
                    || text!.Substring(pos, setlen(3)) == "log" 
                    || currentchar == '(')
                { tokens.Add(new Token(Token.MUL)); } //number (const,var,trig,ln,log,rparen)
            }
            else if (constants.Contains(currentchar))
            { 
                tokens.Add(make_constant());
                next_char();
            }
            else if (trigs.Contains(text!.Substring(pos,setlen(3))))
            {
                tokens.Add(make_trig());
                next_char(3);
            }
            else if (text!.Substring(pos,setlen(2)) == "ln")
            { 
                tokens.Add(new Token(Token.LN));
                next_char(2);
            }
            else if (text!.Substring(pos,setlen(3)) == "log")
            { 
                tokens.Add(new Token(Token.LOG));
                next_char(3);
            }
            else if (variable.Contains(currentchar))
            { 
                tokens.Add(new Token("VAR" , Convert.ToString(currentchar)));
                next_char();
            }
            else if (currentchar == '+')
            { 
                tokens.Add(new Token(Token.ADD));
                next_char();
            } 
            else if (currentchar == '-')
            { 
                tokens.Add(new Token(Token.SUB));
                next_char();
            }
            else if (currentchar == '/')
            { 
                tokens.Add(new Token(Token.DIV));
                next_char();
            }
            else if (currentchar == '^')
            { 
                tokens.Add(new Token(Token.EXP));
                next_char();
            }
            else if (currentchar == '(')
            { 
                if (prevchar == ')')
                { tokens.Add(new Token(Token.MUL)); } //(expr)(expr) | ex: (1+1)(1+1)
                tokens.Add(new Token(Token.LPAREN));
                next_char();
            }
            else if (currentchar == ')')
            { 
                tokens.Add(new Token(Token.RPAREN));
                next_char();
            }
            else
            {
                char charexcpt = currentchar;
                next_char();
                throw new ArgumentOutOfRangeException(nameof(charexcpt), $"Invalid Character: {charexcpt}");
            }
        }
        return tokens;
    }
}