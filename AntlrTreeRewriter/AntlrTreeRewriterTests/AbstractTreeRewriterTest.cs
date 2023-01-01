namespace AntlrTreeRewriterTests;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

public abstract class AbstractTreeRewriterTest
{
  protected static IParseTree Parse(string input)
  {
    var lexer = new ExprLexer(CharStreams.fromString(input));
    var parser = new ExprParser(new CommonTokenStream(lexer));

    return parser.parse();
  }
}