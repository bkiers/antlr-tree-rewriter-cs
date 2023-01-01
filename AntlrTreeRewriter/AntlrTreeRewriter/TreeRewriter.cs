namespace AntlrTreeRewriter;

using Antlr4.Runtime.Tree;

public class TreeRewriter
{
  private readonly IParseTree root;
  private readonly ISet<int?> ignoredTokenTypes;
  private readonly ISet<int?> promoteTokenTypes;

  public TreeRewriter(IParseTree root)
  {
    this.root = root;
    this.ignoredTokenTypes = new HashSet<int?>();
    this.promoteTokenTypes = new HashSet<int?>();
  }

  public TreeRewriter Ignore(params int[] tokenTypes)
  {
    foreach (var tokenType in tokenTypes) 
    {
      this.ignoredTokenTypes.Add(tokenType);
    }

    return this;
  }

  public TreeRewriter Promote(params int[] tokenTypes)
  {
    foreach (var tokenType in tokenTypes)
    {
      this.promoteTokenTypes.Add(tokenType);
    }

    return this;
  }

  public TreeNode Rewrite()
  {
    var ast = new TreeNode();
    this.BuildAst(ast, this.root);
    return ast;
  }

  private void BuildAst(TreeNode ast, IParseTree tree)
  {
    // "Flatten" the tree
    ast.SetParseTree(tree);

    if (tree is ITerminalNode)
    {
      return;
    }

    // Collect all child indices of tokens that are not to be ignored and those that are other parser rules
    var childIndices = new List<int>();

    for (var i = 0; i < tree.ChildCount; i++)
    {
      var child = tree.GetChild(i);

      if (child is ITerminalNode terminalNode)
      {
        if (!this.ignoredTokenTypes.Contains(terminalNode.Symbol.Type))
        {
          // The child is a token that is not to be ignored
          childIndices.Add(i);
        }
      }
      else
      {
        // The child is another parser rule (ParserRuleContext)
        childIndices.Add(i);
      }
    }

    if (childIndices.Count == 1)
    {
      // There is only 1 child in this tree: let it be "flattened" by the next recursive call
      this.BuildAst(ast, tree.GetChild(childIndices[0]));
    }
    else
    {
      // There are multiple children
      foreach (var index in childIndices)
      {
        var childTree = tree.GetChild(index);
        var childAst = new TreeNode(childTree);

        if (this.promoteTokenTypes.Contains(childAst.TokenType))
        {
          ast.SetParseTree(childTree);
        }
        else
        {
          ast.AddChild(childAst);
        }

        this.BuildAst(childAst, childTree);
      }
    }
  }
}