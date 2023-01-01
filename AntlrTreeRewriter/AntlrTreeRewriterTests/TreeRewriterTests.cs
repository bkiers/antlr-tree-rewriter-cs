namespace AntlrTreeRewriterTests;

using AntlrTreeRewriter;
using Xunit;

public class TreeRewriterTests : AbstractTreeRewriterTest
{
  [Fact]
  public void SimpleTest()
  {
    var tree = Parse("a + b");

    var node = new TreeRewriter(tree)
        .Ignore(ExprLexer.Eof)
        .Rewrite();

    Assert.Equal("add_expr", node.Label);
    Assert.Equal("a", node.Children[0].Label);
    Assert.Equal("+", node.Children[1].Label);
    Assert.Equal("b", node.Children[2].Label);
  }

  [Fact]
  public void PromoteTest()
  {
    var tree = Parse("a + b");

    var node = new TreeRewriter(tree)
        .Ignore(ExprLexer.Eof)
        .Promote(ExprLexer.ADD)
        .Rewrite();

    Assert.Equal("+", node.Label);
    Assert.Equal("a", node.Children[0].Label);
    Assert.Equal("b", node.Children[1].Label);
  }

  [Fact]
  public void IgnoreTest()
  {
    var tree = Parse("(a + (b))");

    var node = new TreeRewriter(tree)
        .Ignore(ExprLexer.Eof, ExprLexer.OPAR, ExprLexer.CPAR)
        .Rewrite();

    Assert.Equal("a", node.Children[0].Label);
    Assert.Equal("+", node.Children[1].Label);
    Assert.Equal("b", node.Children[2].Label);
  }

  [Fact]
  public void PromoteAndIgnoreTest()
  {
    var tree = Parse("(a) + b");

    var node = new TreeRewriter(tree)
        .Promote(ExprLexer.ADD)
        .Ignore(ExprLexer.Eof, ExprLexer.OPAR, ExprLexer.CPAR)
        .Rewrite();

    Assert.Equal("+", node.Label);
    Assert.Equal("a", node.Children[0].Label);
    Assert.Equal("b", node.Children[1].Label);
  }

  [Fact]
  public void PromoteMultipleTest()
  {
    var tree = Parse("a + b - c");

    var node = new TreeRewriter(tree)
        .Ignore(ExprLexer.Eof)
        .Promote(ExprLexer.ADD, ExprLexer.MIN)
        .Rewrite();

    Assert.Equal("+", node.Label);
    Assert.Equal("a", node.Children[0].Label);
    Assert.Equal("-", node.Children[1].Label);
    Assert.Equal("b", node.Children[1].Children[0].Label);
    Assert.Equal("c", node.Children[1].Children[1].Label);
  }
}