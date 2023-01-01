#!/usr/bin/env sh

java -jar antlr-4.11.1-complete.jar -package AntlrTreeRewriterTests -no-listener -Dlanguage=CSharp ./grammars/*.g4

mv ./grammars/*.cs ./AntlrTreeRewriter/AntlrTreeRewriterTests

rm ./grammars/*.interp
rm ./grammars/*.tokens