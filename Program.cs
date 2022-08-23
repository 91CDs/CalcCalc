global using static Derivative.variables;
global using static Derivative.Token;
using System;

namespace Derivative;

// ============CONSTANTS============= 
public static class variables
{
    //For Lexer
    public const string number = "0123456789";
    public const string variable = "xyzabcd";
    public const string constants = "ep";
    public readonly static List<string> trigs = new List<string>(){"sin","cos","tan","csc","sec","cot"};
    //For Eval
    public readonly static List<string> Num =new List<string>(){"INT","VAR","CONST"};
}

// ============CALCULATE============= 
class Calculate
{
    public static void run(string input)
    {
        //lexer
        Lexer lexer = new Lexer(input);
        var tokens = lexer.make_tokens();
        //parse
        Parser parser = new Parser(tokens);
        var ast = parser.parse();
        //eval
        var eval = Evaluator.visit(ast);

        Console.WriteLine(tokens.Repr());
        Console.WriteLine(ast);
        Console.WriteLine(eval);
    }
    public static void Main(string[] args)
    {
        while (true)
        {
            Console.Write("calc >");
            string? input = Console.ReadLine();
            if (input == String.Empty)
            { break; }
            run(input!);
        }
    }
}
