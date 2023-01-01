grammar Expr;

parse
 : expr EOF
 ;

expr
 : or_expr
 ;

or_expr
 : and_expr (OR expr)?
 ;

and_expr
 : add_expr (AND expr)?
 ;

add_expr
 : mult_expr ((ADD | MIN) expr)?
 ;

mult_expr
 : unary_expr ((MUL | DIV | MOD) expr)?
 ;

unary_expr
 : MIN atom
 | atom
 ;

atom
 : OPAR expr CPAR
 | ID
 | NUM
 ;

ADD  : '+'; // 1
MIN  : '-'; // 2
MUL  : '*'; // 3
DIV  : '/'; // 4
MOD  : '%'; // 5
AND  : '&&'; // 6
OR   : '||'; // 7
OPAR : '('; // 8
CPAR : ')'; // 9
ID   : [a-zA-Z_] [a-zA-Z_0-9]*; // 10
NUM  : [0-9]+ ('.' [0-9]+)?; // 11
WS   : [ \t\r\n]+ -> skip; // 12