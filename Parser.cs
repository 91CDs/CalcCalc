using System;

namespace Derivative;

// ============NODES============= 
class Node
{
    public string nodetype = "";
    public Token token { get; set; }
    public Node? left { get; set; }
    public Node? right { get; set; }    
    public Node(Token token, string nodetype)
    {
        this.nodetype = nodetype;
        this.token = token;
        this.left = default;
        this.right = default;
    }
    public Node(Token token, Node right, string nodetype )
    {
        this.nodetype = nodetype;
        this.token = token;
        this.left = default;
        this.right = right;
    }
    public Node(Node? left, Token token, Node? right, string nodetype)
    {
        this.nodetype = nodetype;
        this.token = token;
        this.left = left;
        this.right = right;
    }
    public override string ToString()
    {
        if (left == default && right == default)
        { return String.Format("{0}", token); }
        else if (left == default)
        { return String.Format("({0} {1})", token, right); }
        else
        { return String.Format("({0} {1} {2})", left, token, right); }
    }
}

// ============PARSER============= 
class Parser
{
    List<Token> tokenlist;
    int idx = -1;
    Token currenttoken = new Token();
    public Parser(List<Token> tokenlist)
    {
        this.tokenlist = tokenlist;
        next_token();
    }
    public void next_token()
    {
        idx += 1;
        currenttoken = idx < tokenlist.Count ? tokenlist[idx] : new Token();
    }
    public Node factor() //always node
    {
        Token token = currenttoken;
        if (new List<string>(){"INT","FLOAT","CONST","VAR"}.Contains(currenttoken.type))
        {
            next_token();
            return new Node(token, "NumberNode");
        }
        // else if (currenttoken.type == "VAR")
        // {
        //     next_token();
        //     return new Node(token, "VarNode");
        // }
        else if (currenttoken.type == Token.LPAREN)
        {
            next_token();
            var paren_node = expr(); 
            if (currenttoken.type == Token.RPAREN) 
            { next_token(); }
            return paren_node;
        }
        else //OP TOKEN TYPE != L.PAREN or RPAREN
        { 
            throw new ArgumentOutOfRangeException(nameof(currenttoken),"Expected: INT, FLOAT, CONST, or VAR");
        }
    }
    public Node unary() //right associative | unarynode if OP TOKEN first , else factor()
    {
        Stack<Token> unary_tokens = new Stack<Token>();
        while (new List<string>(){SUB,LOG,LN,SIN,COS,TAN,CSC,SEC,COT}.Contains(currenttoken.type))
        {
            unary_tokens.Push(currenttoken);
            next_token();
            if (new List<string>(){"INT","FLOAT","VAR","CONST"}.Contains(currenttoken.type))
            { break; }
        }
        var rightnode = factor();
        if (unary_tokens.Count != 0)
        {
            int len = unary_tokens.Count;
            for (int i = 0; i < len; i++)
            {
                // var node_type = rightnode.nodetype switch
                // {
                //     "NumberNode" => "ArithmeticNode",
                //     "ArithmeticNode" => "ArithmeticNode",
                //     _ => "UnaryNode"
                // };
                rightnode = new Node(unary_tokens.Pop(), rightnode , "UnaryNode");
            }
        }
        return rightnode;
    }
    
    public Node exp() //right associative
    {
        Stack<Node> exp_nodes = new Stack<Node>();
        var leftnode = unary();
        while (currenttoken.type == Token.EXP)
        {   
            exp_nodes.Push(leftnode);
            next_token();
            leftnode = unary();
            if (currenttoken.type != Token.EXP)
            { break; }
        }
        var rightnode = leftnode;
        if (exp_nodes.Count != 0)
        {
            int len = exp_nodes.Count;
            for (int i = 0; i < len; i++)
            {
                leftnode = exp_nodes.Pop();
                var op = new Token(Token.EXP);
                // var node_type = (leftnode.nodetype, rightnode.nodetype) switch
                // {
                //     ("NumberNode" , "NumberNode") 
                //     or ("ArithmeticNode" , "NumberNode") 
                //     or ("NumberNode" , "ArithmeticNode") 
                //     or ("ArithmeticNode" , "ArithmeticNode") => "ArithmeticNode",
                //     _ => "BinaryNode"
                // }; 
                rightnode = new Node(leftnode, op, rightnode, "BinaryNode");
            }
        }
        return rightnode;
    }
    
    public Node term() //left associative
    {
        var leftnode = exp();
        while (new List<string>(){Token.MUL,Token.DIV}.Contains(currenttoken.type))
        {
            var op = currenttoken;
            next_token();
            var rightnode = exp();
            leftnode = new Node(leftnode, op, rightnode, "BinaryNode");
        }
        return leftnode;
    }

    public Node expr() //left associative
    {
        var leftnode = term();
        while (new List<string>(){Token.ADD,Token.SUB}.Contains(currenttoken.type))
        {
            var op = currenttoken;
            next_token();
            var rightnode = term();
            leftnode = new Node(leftnode, op ,rightnode, "BinaryNode");
        }
        return leftnode;
    }
    // PARSER | AST GENERATOR
    public Node parse()
    {
        var eq = expr();
        return eq;
    }
}