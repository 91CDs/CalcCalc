using System;
using System.Reflection;
using static Derivative.Evaluator;
using static Derivative.Function;

namespace Derivative;
class Function
{
    public string type { get; set; } 
    public string token { get; set; }
    public Function? left { get; set; }
    public Function? right { get; set; }    
    public Function(string token_, string type_)
    {
        type  = type_;
        token = token_;
        left  = default;
        right = default;
    }
    public Function(string token_, Function right_)
    {
        type  = "OP";
        token = token_;
        left  = default;
        right = right_;
    }
    public Function(Function? left_, string token_, Function? right_)
    {
        type  = "OP";
        token = token_;
        left  = left_;
        right = right_;
    }
    public override string ToString()
    {
        if (left == default && right == default)
        { return String.Format("{0}", token); }
        else if (left == default)
        { return String.Format("{0}({1})", token, right); }
        else if (token == "+" || token == "-")
        { return String.Format("{0} {1} {2}", left, token, right); }
        else
        { return String.Format("({0}{1}{2})", left, token, right); }
    }
    // Helper Functions
    // Turns ints, floats, constants, and vars into functions
    public static Function func(string T_value, string T_type)
    {
        return new Function(T_value, T_type);
    }
    // Turns node into functions
    public static Function func(Node node)
    {
        var functoken = node.token.type switch 
        {
            "INT" or "FLOAT" or "CONST" => "NUM",
            "VAR" => "VAR",
            Token.ADD => "+",
            Token.SUB => "-",
            Token.MUL => "*",
            Token.DIV => "/",
            Token.EXP => "^",
            _ => node.token.type.ToLower()
        };
        if (node.left == default && node.right == default)
        { return new Function(node.token.value, functoken); }
        else if (node.left == default)
        { return new Function(functoken, func(node.right!)); }
        else
        { return new Function(func(node.left!), functoken, func(node.right!)); }
    }
    // Functions for calculating derivatives (with simplify)
    public static Function Num_dx()
    {
        return func("0", "NUM");
    }
    public static Function Var_dx()
    {
        return func("1", "NUM");
    }

    // d/dx(num) = 0
    // d/dx(var) = 1
    //============ DERIVATIVES of UnaryOp ===============
    public static Function Neg_dx(Node function)
    {
        var dx = new Function("-", visit(function));
        // = = = Simplify = = =
        var fx_dx = dx.right!;
        if (fx_dx.token == "0") // d/dx(num) = 0
        {
            dx = Num_dx();
        }
        return dx;
        
    }
    public static Function Sin_dx(Node function)
    {
        var dx = new Function
        (
            new Function("cos", func(function)),
            "*",
            visit(function)
        );
        // = = = Simplify = = =
        var fx_dx = dx.right!;
        if (fx_dx.token == "0") // d/dx(num) = 0
        {
            dx = Num_dx();
        }
        else if (fx_dx.token == "1")
        {
            dx = dx.left!;
        }
        return dx;
    }
    public static Function Cos_dx(Node function)
    {
        var dx = new Function
        (
            new Function("-", new Function("sin", func(function))),
            "*",
            visit(function)
        );        
        // = = = Simplify = = =
        var fx_dx = dx.right!;
        if (fx_dx.token == "0") // d/dx(num) = 0
        {
            dx = Num_dx();
        }
        else if (fx_dx.token == "1")
        {
            dx = dx.left!;
        }
        return dx;
    }
    public static Function Tan_dx(Node function)
    {
        var dx = new Function
        (
            new Function("sec", func(function)),
            "*",
            visit(function)
        );
        // = = = Simplify = = =
        var fx_dx = dx.right!;
        if (fx_dx.token == "0") // d/dx(num) = 0
        {
            dx = Num_dx();
        }
        else if (fx_dx.token == "1")
        {
            dx = dx.left!;
        }
        return dx;
    }
    public static Function Csc_dx(Node function)
    {
        var dx = new Function
        (
            new Function
            (
                new Function("-", new Function("csc", func(function))),
                "*",
                new Function("cot", func(function))
            ),
            "*",
            visit(function)
        );
        // = = = Simplify = = =
        var fx_dx = dx.right!;
        if (fx_dx.token == "0") // d/dx(num) = 0
        {
            dx = Num_dx();
        }
        else if (fx_dx.token == "1")
        {
            dx = dx.left!;
        }
        return dx;
    }
    public static Function Sec_dx(Node function)
    {
        var dx = new Function
        (
            new Function
            (
                new Function("sec", func(function)),
                "*",
                new Function("tan", func(function))
            ),
            "*",
            visit(function)
        );
        // = = = Simplify = = =
        var fx_dx = dx.right!;
        if (fx_dx.token == "0") // d/dx(num) = 0
        {
            dx = Num_dx();
        }
        else if (fx_dx.token == "1")
        {
            dx = dx.left!;
        }
        return dx;
    }
    public static Function Cot_dx(Node function)
    {
        var dx = new Function
        (
            new Function("csc", new Function(func(function), "^", func("2", "NUM"))),
            "*",
            visit(function)
        );
        // = = = Simplify = = =
        var fx_dx = dx.right!;
        if (fx_dx.token == "0") // d/dx(num) = 0
        {
            dx = Num_dx();
        }
        else if (fx_dx.token == "1")
        {
            dx = dx.left!;
        }
        return dx;
    }
    public static Function Ln_dx(Node function)
    {
        var dx = new Function
        (
            new Function(func("1", "NUM"), "/", func(function)),
            "*",
            visit(function)
        );
        // = = = Simplify = = =
        var fx_dx = dx.right!;
        if (fx_dx.token == "0") // d/dx(num) = 0
        {
            dx = Num_dx();
        }
        else if (fx_dx.token == "1")
        {
            dx = dx.left!;
        }
        return dx;
    }
    //============ DERIVATIVES of BinaryOp ===============
    public static Function AddSub_dx(Node leftfunc, Node rightfunc)
    {
        var dx = new Function(visit(leftfunc) , "+" , visit(rightfunc)); 
        // = = = Simplify = = =
        var fx_dx = dx.left!;
        var gx_dx = dx.right!;
        if (fx_dx.token == "0") 
        { 
            dx = gx_dx; // Identity Property
        }
        else if (gx_dx.token == "0")
        { 
            dx = fx_dx; // Identity Property
        }
        return dx;
    }
    public static Function ConstantMul_dx(Node leftfunc, Node rightfunc)
    {
        var dx = new Function(func(leftfunc) , "*" , visit(rightfunc)); 
        // = = = Simplify = = =
        var coeff = dx.left!; //Always NUM 
        var gx_dx = dx.right!;
        if (gx_dx.token == " * ") // Derivatives of (EXP) terms
        {
            if (gx_dx.left!.type == "NUM") 
            { 
                coeff.token = (Convert.ToInt32(coeff.token) * Convert.ToInt32(gx_dx.left!.token)).ToString(); // Multiplying Nums
                dx.left  = coeff;
                dx.right = gx_dx = gx_dx.right;
            }
            else if (gx_dx.right!.type == "NUM")
            { 
                coeff.token = (Convert.ToInt32(coeff.token) * Convert.ToInt32(gx_dx.right!.token)).ToString(); // Multiplying Nums
                dx.left  = coeff;
                dx.right = gx_dx = gx_dx.left;
            }
        }
        else if (gx_dx.type == "NUM")
        {
            coeff.token = (Convert.ToInt32(coeff.token) * Convert.ToInt32(gx_dx.token)).ToString();
            dx = new Function(coeff.token, "NUM");
        }
        return dx;
    }
    public static Function Product_dx(Node leftfunc, Node rightfunc)
    {
        var dx = new Function
        (
            new Function(func(leftfunc), "*", visit(rightfunc)),
            "+",
            new Function(func(rightfunc), "*", visit(leftfunc))
        ); 
        // = = = Simplify = = =
        
        return dx;
    }
    public static Function Quotient_dx(Node leftfunc, Node rightfunc)
    {
        var dx = new Function
        (
            new Function
            (
                new Function(func(rightfunc), " * ", visit(leftfunc)),
                "-",
                new Function(func(leftfunc), " * ", visit(rightfunc))
            ),
            "/",
            new Function(func(rightfunc), "^", func("2", "NUM"))
        );
        // = = = Simplify = = =
        
        return dx;
    }
    public static Function Power_dx(Node leftfunc, Node rightfunc)
    {
        int exp = Convert.ToInt32(rightfunc.token.value);
        return new Function
        (
            func(rightfunc),
            " * ",
            exp == 2 ? func(leftfunc) : new Function(func(leftfunc),  "^",  func((exp-1).ToString(), "NUM"))
        );
    }
    public static Function GeneralPower_dx(Node leftfunc, Node rightfunc)
    {
        int exp = Convert.ToInt32(rightfunc);
        return new Function
        (
            new Function
            (
                func(rightfunc),
                " * ",
                exp == 2 ? func(leftfunc) : new Function(func(leftfunc), "^", func((exp-1).ToString(), "NUM"))
            ),
            "*",
            visit(leftfunc)
        );
    }
    public static Function Exponential_dx(Node leftfunc, Node rightfunc)
    {
        return new Function
        (
            new Function(func(leftfunc), "^", func(rightfunc)),
            "*",
            visit(rightfunc)
        );
    }
    public static Function GeneralExp_dx(Node leftfunc, Node rightfunc)
    {
        return new Function
        (
            new Function
            (
                new Function(func(leftfunc), "^", func(rightfunc)),
                "*", 
                new Function("LN", func(leftfunc))
            ),
            "*",
            visit(rightfunc)
        );
    }
    public static Function PowerLog_dx(Node leftfunc, Node rightfunc)
    {
        var exp = new Node(leftfunc, new Token("MUL"), new Node(new Token("LN"), rightfunc, "UnaryNode"), "BinaryNode");
        return new Function
        (
            new Function
            (
                func("e", "NUM"),
                "^",
                func(exp)
            ),
            "*",
            visit(exp)
        );
    }
}

// ============EVALUATOR============= 
static class Evaluator
{
    public static Function visit(Node node)
    {
        string methodName = $"visit_{node.nodetype}";
        MethodInfo visitmethod = typeof(Evaluator).GetMethod(methodName) ?? throw new Exception("no method with dat name");
        return (Function)visitmethod.Invoke(typeof(Evaluator) , new object[]{node})!;
    }

    public static Function visit_NumberNode(Node node)
    {
        if (node.token.type == "VAR")
        {  
            return Var_dx(); 
        }
        else
        { 
            return Num_dx(); 
        }
    }
    public static Function visit_UnaryNode(Node node)
    {
        var op = node.token.type;
        var function = node.right!;
        if (op == Token.SUB)
        {
            return Neg_dx(function);
        }
        else if (op == Token.SIN)
        {
            return Sin_dx(function);
        }
        else if (op == Token.COS)
        {
            return Cos_dx(function);
        }
        else if (op == Token.TAN)
        {
            return Tan_dx(function);
        }
        else if (op == Token.CSC)
        {
            return Csc_dx(function);
        }
        else if (op == Token.SEC)
        {
            return Sec_dx(function);
        }
        else if (op == Token.COT)
        {
            return Cot_dx(function);
        }
        else  //(op == Token.LN)
        {
            return Ln_dx(function);
        }
        // else (op == Token.LOG)
        // {
            
        // }
    }
    public static Function visit_BinaryNode(Node node) //DONT CHANGE TOKEN TYPE STRING PLSSS
    {
        var op = node.token.type;
        var leftfunc = node.left!;
        var rightfunc = node.right!;
        if (op == Token.ADD || op == Token.SUB)
        {  
            return AddSub_dx(leftfunc, rightfunc);
        }
        else if (op == Token.MUL)
        {
            if (new List<string>(){"INT","VAR","CONST"}.Contains(leftfunc.token.type))
            { 
                return ConstantMul_dx(leftfunc, rightfunc);
            }
            else if (new List<string>(){"INT","VAR","CONST"}.Contains(leftfunc.token.type))
            {
                return ConstantMul_dx(rightfunc, leftfunc);
            }
            else
            { 
                return Product_dx(leftfunc, rightfunc);
            }
        }
        else if (op == Token.DIV)
        {
            return Quotient_dx(leftfunc, rightfunc);
        }
        else //(op == Token.EXP)
        {
            if (leftfunc.token.type == "VAR" && new List<string>(){"INT","VAR","CONST"}.Contains(rightfunc.token.type))
            {
                return Power_dx(leftfunc, rightfunc);
            }
            else if (new List<string>(){"INT","VAR","CONST"}.Contains(rightfunc.token.type))
            {
                return GeneralPower_dx(leftfunc, rightfunc);
            }
            else if (leftfunc.token.value == "e")
            {
                return Exponential_dx(leftfunc, rightfunc);
            }
            else if (new List<string>(){"INT","VAR","CONST"}.Contains(leftfunc.token.type))
            {
                return GeneralExp_dx(leftfunc, rightfunc);
            }
            else
            { 
                return PowerLog_dx(leftfunc, rightfunc);
            }
        }
    }
}