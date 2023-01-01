namespace AntlrTreeRewriterTests;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using AntlrTreeRewriter;
using Xunit;

public class TreeNodeTests : AbstractTreeRewriterTest
{
  [Fact]
  public void ToString_SingleNode_HasNoParenthesis() 
  {
    var tree = Parse("42");

    var node = new TreeRewriter(tree)
      .Ignore(ExprLexer.Eof)
      .Rewrite();

    Assert.Equal("42", node.ToString());
  }

  [Fact]
  public void ToString_MultipleNodes_HasParenthesis() 
  {
    var tree = Parse("Q + 42");

    var node = new TreeRewriter(tree)
      .Ignore(ExprLexer.Eof)
      .Promote(ExprLexer.ADD)
      .Rewrite();

    Assert.Equal("(+ Q 42)", node.ToString());
  }

  [Fact]
  public void ToString_MultipleExpressions_HasNestedParenthesis()
  {
    var tree = Parse("Q + 42 + x");

    var node = new TreeRewriter(tree)
      .Ignore(ExprLexer.Eof)
      .Promote(ExprLexer.ADD)
      .Rewrite();

    Assert.Equal("(+ Q (+ 42 x))", node.ToString());
  }

  [Fact]
  public void TreeNode_Context_LabelIsNormalized()
  {
    var node = new TreeNode(new Add_exprContext());

    Assert.Equal("add_expr", node.Label);
  }

  [Fact]
  public void TreeNode_TerminalNodeImpl_LabelIsNotNormalized()
  {
    var node = new TreeNode(new TerminalNodeImpl(new CommonToken(3, "FOO")));

    Assert.Equal("FOO", node.Label);
  }

  // This is how the context-class is named for ANTLR's parser rules: `rule_name` becomes `Rule_nameContext`
  public class Add_exprContext : ParserRuleContext 
  {
    public Add_exprContext()
    {
      base.Start = new CommonToken(1, "start");
      base.Stop = new CommonToken(2, "stop");
    }
  }
}

