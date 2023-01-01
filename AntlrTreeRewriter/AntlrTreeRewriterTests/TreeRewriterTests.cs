namespace AntlrTreeRewriterTests;

using Antlr4.Runtime;
using AntlrTreeRewriter;
using Newtonsoft.Json;
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

  [Fact]
  public void JsonDemo()
  {
    var source = "(3.14159265 + Mu) * 42";

    var lexer = new ExprLexer(CharStreams.fromString(source));
    var parser = new ExprParser(new CommonTokenStream(lexer));
    var root = parser.parse();

    var node = new TreeRewriter(root)
      .Ignore(ExprLexer.Eof, ExprLexer.OPAR, ExprLexer.CPAR)
      .Promote(ExprLexer.ADD, ExprLexer.MIN, ExprLexer.MUL, ExprLexer.DIV, ExprLexer.MOD, ExprLexer.AND, ExprLexer.OR)
      .Rewrite();
    
    var json = JsonConvert.SerializeObject(node);

    Assert.Equal("{\"Label\":\"*\",\"TokenType\":3,\"Line\":1,\"StartIndex\":18,\"StopIndex\":18,\"Children\":[{\"Label\":\"+\",\"TokenType\":1,\"Line\":1,\"StartIndex\":12,\"StopIndex\":12,\"Children\":[{\"Label\":\"3.14159265\",\"TokenType\":11,\"Line\":1,\"StartIndex\":1,\"StopIndex\":10,\"Children\":[]},{\"Label\":\"Mu\",\"TokenType\":10,\"Line\":1,\"StartIndex\":14,\"StopIndex\":15,\"Children\":[]}]},{\"Label\":\"42\",\"TokenType\":11,\"Line\":1,\"StartIndex\":20,\"StopIndex\":21,\"Children\":[]}]}", json);

    var deserializedNode = JsonConvert.DeserializeObject<TreeNode>(json);

    Assert.Equal("*", deserializedNode.Label);
    Assert.Equal("+", deserializedNode.Children[0].Label);
    Assert.Equal("3.14159265", deserializedNode.Children[0].Children[0].Label);
    Assert.Equal("Mu", deserializedNode.Children[0].Children[1].Label);
    Assert.Equal("42", deserializedNode.Children[1].Label);
  }
}