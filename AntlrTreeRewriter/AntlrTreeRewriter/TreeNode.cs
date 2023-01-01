namespace AntlrTreeRewriter;

using System.Text;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

public class TreeNode
{
  private readonly IList<TreeNode> children = new List<TreeNode>();

  public TreeNode()
  {
  }

  public TreeNode(IParseTree tree)
  {
    this.SetParseTree(tree);
  }

  public void AddChild(TreeNode child)
  {
    this.children.Add(child);
  }

  public void SetParseTree(IParseTree tree)
  {
    switch (tree)
    {
      case ITerminalNode terminalNode:
        this.SetToken(terminalNode.Symbol);
        break;
      case ParserRuleContext context:
        this.SetContext(context);
        break;
      default:
        // Should never happen
        throw new Exception("Unknown ParseTree: " + tree.GetType().Name);
    }
  }

  public string? Label { get; private set; }
  
  public int? TokenType { get; private set; }
  
  public int? Line { get; private set; }
  
  public int? StartIndex { get; private set; }
  
  public int? StopIndex { get; private set; }

  public IList<TreeNode> Children => new List<TreeNode>(this.children);

  public override string ToString()
  {
    var includeParens = this.children.Count > 0;

    var builder = new StringBuilder()
        .Append(includeParens ? "(" : "")
        .Append(this.Label);

    foreach (var child in this.children)
    {
      builder.Append(' ').Append(child);
    }

    return builder
        .Append(includeParens ? ")" : "")
        .ToString();
  }

  public string Mermaid()
  {
    var builder = new StringBuilder("graph TD\n\n");
    var stack = new Stack<TreeNode>();
    
    stack.Push(this);

    while (stack.Count > 0)
    {
      var current = stack.Pop();

      foreach (var child in current.Children)
      {
        builder
          .Append($"  N_{current.GetHashCode()}[\"{current.Label}\"]")
          .Append(" --> ")
          .Append($"N_{child.GetHashCode()}[\"{child.Label}\"]\n");
        
        stack.Push(child);
      }
    }
    
    return builder.ToString();
  }

  private static string RuleName(string className)
    => $"{className[..1].ToLower()}{Regex.Replace(className[1..], @"Context$", string.Empty)}";

  private void SetToken(IToken token)
  {
    this.Label = token.Text;
    this.TokenType = token.Type;
    this.Line = token.Line;
    this.StartIndex = token.StartIndex;
    this.StopIndex = token.StopIndex;
  }

  private void SetContext(ParserRuleContext context)
  {
    this.Label = RuleName(context.GetType().Name);
    this.TokenType = null;
    this.Line = context.Start.Line;
    this.StartIndex = context.Start.StartIndex;
    this.StopIndex = context.Stop.StopIndex;
  }
}