using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;



namespace prk2_var7
{
    /* 
     * Program created by: https://github.com/miiroo
     * 
    *  NOTE: Additional libs are used: System.IO 
    *  NOTE: You have to create a new project (Visual C# -> Console app) 
    *          and repaste all in namespace brackets
    *  NOTE: You have to build project first (ctrl+shift+b) and add code2.txt to builded .exe file's folder
    *          than you can run it and test. You have to change line in code2.txt to "crash" it.
    *  
    * How it works: it detects IF and THEN then it looks for boolean expression between them and chek it.
    *                   there's recursive for boolean expression because there could be more than one AND/OR
    * 
    * Step1: We should find IF and THEN to start work with boolean expression 
    *           and they should be in a single example in other case show error.
    * 
    * Step2: We find boolean expression and we have to detect is there any AND/OR(1) or no(2)
    * 
    * (1)AND/OR - we have to divide to two parts: expression before AND/OR and after and recursivly 
    *                   check both part in Step2.
    *                   
    * (2)no AND/OR - we've found single statement
    *                   with some of that patterns:
    *                       -smth </>/=/<=/>=/<> smth
    *                       -smth
    *                       
    *                   in first way we have to be sure that there is correct delimiter and it's alone
    *                   then we check both parts to it's grammar (not done)   
    *                   
    *                   in second way it could be func which returns boolean or boolean id
    *                   so we just check that there is something. (still don't check grammar coz it wasn't realized)
    *                   
    * Step3: We have to check that there is something after THEN (just check it exists and don't check its grammar)                   
    * Step4: Done :)     
    * 
    * 
    * 
    * Some useful grammar used for that IF:
    * 
    * <state> ::= if <exp> then <statement>
    * <statement> ::= <assign> | <func> 
    * <exp> ::= <bool exp> |<exp> AND <exp> | <exp> OR <exp>
    * <bool exp> ::= <part><compare symbol><part>
    * <compare symbol> ::= < | > | = | <= | >= | <>
    * <part> ::=  <id>|<const>|<assign>|<func>
    * 
    * <id> ::= idd | <id>.idd | <func>.idd     (note: idd - it's a grammar from previous work (lab 1))
    * 
    * <assign> ::= <id> := <part>
    * 
    * <func> ::= <id>(<arg>) | <id>()
    * <arg> ::= <argum> | <arg>,<argum>
    * <argum> ::= <part>
    * 
    * <strconst> ::= '<str>'
    * <str> ::= <str><letter> | <letter>
    * <letter> ::= a..z | A..Z | 0..9 | space
    * 
    * <numconst> ::= <number><operator><numconst> | <number>
    * <operator> ::= +|-|*|/
    * <number> ::= <number><digit> | <digit>
    * <digit> ::= 0..9
    * 
    * 
    */


    class Program
    {
        private static List<String> keyArray = new List<String> { "procedure", "TObject", "var", "integer", "Begin",
            "if", "and", "then", "else", "while", "not", "do", "End", "=", "+", "<", ">", "*", "-", "/"};
        private static List<String> dArray = new List<String> { ".", ":", ";", "'", "(", ")", ":=", "," };

        //if (CEdit1.Text='') and (Kedit2.Text='' and d=1) then ShowMessage('Please enter data');
        static void Main(string[] args) {
            string[] lines = File.ReadAllLines(@"code2.txt");
            string str = lines[0];
            Console.WriteLine("CODE:");
            Console.WriteLine(str);
            string statement = "";
            int brackets = 0;
            if (fastAnalize(str)) {

                //there is IF and THEN
                if (findIfThen(str)) {
                    //get statement between IF and THEN
                    int posStart = str.IndexOf("if") + 2;
                    int posEnd = str.IndexOf("then");
                    for (int i = posStart; i < posEnd; i++) {
                        if (str[i] == '(') brackets++;
                        if (str[i] == ')') brackets--;
                        statement += str[i];
                    }

                    //check expression is correct
                    if (checkExpr(statement) && brackets == 0) {
                        string part = "";
                        for (int i = posEnd + 4; i < str.Length; i++) {
                            if (str[i] != ';') part += str[i];
                            else i = str.Length;
                        }
                        if (checkGr(part)) Console.WriteLine("Success");
                        else Console.WriteLine("Error: wrong or missing statement after THEN.");
                    }
                    else Console.WriteLine("Error: wrong bool expression.");
                }
                else {
                    Console.WriteLine("Error: some troubles with IF or THEN.");
                }
            }
            Console.ReadKey();
        }


        static bool fastAnalize(string str) {
            int i = 0;
            while( i<str.Length) {
                if (Char.IsDigit(str[i]) || Char.IsLetter(str[i]) || dArray.Contains(str[i].ToString()) || keyArray.Contains(str[i].ToString()) || str[i] == ' ') i++;
                else {
                    Console.WriteLine("Can't recognize that symbol: " + str[i]);
                    return false;
                }
            }
            return true;
        }

        //find if and then. Return TRUE if all is ok in other case return FALSE
        static bool findIfThen(string str) {
            string word = "";
            bool ifFounded = false;
            bool thenFounded = false;
            for (int i = 0; i < str.Length; i++) {
                word = "";
                while (i < str.Length && !dArray.Contains(str[i].ToString()) && str[i] != ' ') {
                    word += str[i];
                    i++;
                }
                //there should be one IF and one THEN in other case it's error.
                if (word == "if" && !ifFounded) {
                    //symbol after IF should be ( or space
                    if (str[i] == '(' || str[i] == ' ') ifFounded = true;
                    else return false;
                }
                else {
                    if (word == "if") return false;
                }

                if (word == "then" && !thenFounded) {
                    //symbol before THEN should be ) or space
                    //symbol after THEN should be space
                    if (str.IndexOf("then") + 4 >= str.Length) return true;
                    if ((str[str.IndexOf("then") - 1] == ' ' || str[str.IndexOf("then") - 1] == ')') && str[str.IndexOf("then") + 4] == ' ') thenFounded = true;
                    else return false;
                }
                else {
                    if (word == "then") return false;
                }

            }
            if (ifFounded && thenFounded) return true;
            return false;
        }

        //chech boolean expression. Return TRUE if all is ok in other case return FALSE
        static bool checkExpr(string str) {
            string part = "";
            string part2 = "";
            //check that there is OR/AND ///////////
            if (str.Contains("or")) {
                //check it's not in some word
                if( str.IndexOf("or")+2<str.Length && (str[str.IndexOf("or")-1] != ')' && str[str.IndexOf("or") - 1] != ' ') && (str[str.IndexOf("or") + 2] != '(' && str[str.IndexOf("or") + 2] != ' ')) {
                    //there is no OR. We should check boolean grammar
                    if (boolGramar(str)) return true;
                    else return false;

                }
                else {
                    if (!(str.IndexOf("or") + 2 < str.Length)) return false;
                    //there is OR. We should divide to two parts and check both of them
                    int posS = str.IndexOf("or");
                    for (int i = 0; i < posS; i++) part += str[i];
                    posS += 2;
                    for (int i = posS; i < str.Length; i++) part2 += str[i];

                    if (checkExpr(part))
                        if (checkExpr(part2)) return true;
                        else return false;
                    else return false;
                }
            }

            if (str.Contains("and")) {
                //check it's not in some word
                if (str.IndexOf("and") + 3 < str.Length && (str[str.IndexOf("and") - 1] != ')' && str[str.IndexOf("and") - 1] != ' ') && (str[str.IndexOf("and") + 3] != '(' && str[str.IndexOf("and") + 3] != ' ')) {
                    //there is no AND. We should check boolean grammar
                    if (boolGramar(str)) return true;
                    else return false;
                }
                else {
                    if (!(str.IndexOf("and") + 3 < str.Length)) return false;
                    //there is AND. We should divide to two parts and check both of them
                    int posS = str.IndexOf("and");
                    for (int i = 0; i < posS; i++) part += str[i];
                    posS += 3;
                    for (int i = posS; i < str.Length; i++) part2 += str[i];

                    if (checkExpr(part))
                        if (checkExpr(part2)) return true;
                        else return false;
                    else return false;
                }
            }
            ///////////OR/AND CHEKING END//////////////

            //there were no OR/AND
            if (boolGramar(str)) return true;
            return false;
        }

        //cheking how properly bool statement is
        //return TRUE if all is ok in other case return FALSE
        //bool statement is smth </>/<=/>=/= smth
        //smth = some function or math operations
        static bool boolGramar(string str) {
            bool delimF = false;
            string part = "";
            bool part1 = false;
            bool part2 = false;
            //get parts till and after delim
            for (int j = 0; j < str.Length; j++) {
                part = "";
                while (j < str.Length && str[j] != '<' && str[j] != '>' && str[j] != '=') {
                    part += str[j];
                    j++;
                }
                // we found delimetr and it's alone
                if (!delimF) {
                    if (!(j < str.Length))
                        return false;
                    switch (str[j]) {
                        case '<':
                            if (str[j + 1] == '=' || str[j + 1] == '>') j++;
                            part1 = checkGr(part);
                            delimF = true;
                            break;
                        case '>':
                            if (str[j + 1] == '=') j++;
                            part1 = checkGr(part);
                            delimF = true;
                            break;
                        case '=':
                            part1 = checkGr(part);
                            delimF = true;
                            break;
                    }
                }
                //we found delimetr but we have already found it
                //or it's end of string
                else {
                    //we found delimetr
                    if (j<str.Length && (str[j] == '<' || str[j] == '>' || str[j] == '=')) return false;
                    part2 = checkGr(part);
                }
            }
            if (part1 && part2) return true;
            return false;
        }


        //we have to check statement grammar
        //it could be fuction or math operations
        //but in our case we just check that there is smth
        //grammar for bool statement should be
        //<statement> := <part> </>/=/<>... <part>
        //here we get only <part> and check it's grammar 



        //universal way
        static bool checkGr(string str) {
            if (str == "") return false; 
            int j = 0;
            //some cleaning from trash (, ) and spaces
            while (j < str.Length) {
                if (str[j] == ' ' || str[j] == '(')
                    str = str.Remove(j, 1);
                else j = str.Length;
            }

            j = str.Length - 1;
            while (j > 0 && str[j] == ' ') {
                str = str.Remove(j, 1);
                j = str.Length - 1;
            }

            int countOpen = 0;
            bool strConst = false;
            for(int i = 0; i<str.Length; i++) {
                if (!strConst) {
                    if (str[i] == '(') countOpen++;
                    if (str[i] == ')' && countOpen <= 0) {
                        str = str.Remove(i, 1);
                        i--;
                    }
                    if (str[i] == ')' && countOpen > 0) countOpen--;
                }
                if (str[i].ToString() == "'") strConst = !strConst;
            }
            //////////////end of cleaning////////////////////

            //part is <numconst>
            if (checkNum(str)) return true;
            //part is <strconst>
            if (checkStr(str)) return true;
            //part is <func>
            if (checkFunc(str)) return true;
            //part is <assign>
            if (checkAssign(str)) return true;
            //part is <id>
            if (checkId(str)) return true;
            return false;
        }


        //<numconst> := <number><operator><numconst> | <number>
        //<operator> := +|-|*|/
        //<number> := <number><digit> | <digit>
        //<digit> := 0..9
        static bool checkNum(string str) {
            if (str == "") return false;

            bool rowNum = false;
            bool rowOp = false;
            for (int i=0; i<str.Length; i++) {
                if (Char.IsLetter(str[i])) return false;

                //it's digit
                if (char.IsDigit(str[i])) {
                    if (rowOp) rowOp = false;

                    if (!rowNum) {
                        rowNum = true;
                        while (i < str.Length && Char.IsDigit(str[i])) i++;
                        i--;
                    }
                    else return false;
                }

                //it's operator
                if(keyArray.Contains(str[i].ToString())) {
                    if (!rowNum) return false;
                    rowNum = false;
                    if (!rowOp) rowOp = true;
                    else return false;
                }
            }
            if (rowOp) return false;
            return true;
        }

        //<strconst> := '<str>'
        //<str> := <str><letter> | <letter>
        //<letter> := a..z | A..Z | 0..9 | space
        static bool checkStr(string str) {
            if (str == "") return false;

            bool findOne = false;
            bool findSecond = false;
             
            for (int i=0; i<str.Length; i++) {
                if (str[i] != ' ' || str[i].ToString() !="'") {
                    if (!findOne) return false;
                    if (findOne && findSecond) return false;
                }
               if (str[i].ToString() == "'") {
                    if (!findOne) findOne = true;
                    else {
                        if (!findSecond) findSecond = true;
                        else return false;
                    }
               }
            }
            return true;
        }

        //<func> := <id>(<arg>) | <id>()
        //<arg> := <argum> | <arg>,<argum>
        //<argum> := <part>
        static bool checkFunc(string str) {
            if (str == "") return false;

            //check func name is correct function name
            if (Char.IsDigit(str[0])) return false;
            int j = 0;
            string argum = "";
            //find func (
            while (j < str.Length && str[j] != '(') j++;
            j++;
            int countOpen = 1;
            bool getArg = false;
            bool space = false;
            bool haveWord = false;
            bool notALone = false;
            bool getConst = false;
            //get argument in func(<arg>)
            while (j < str.Length && !getArg) {
                if (str[j].ToString() == "'") getConst = !getConst;
                if (!getConst) {
                    while (j< str.Length && str[j] != ',' && countOpen != 0) {
                        if (str[j] == '(') countOpen++;
                        if (str[j] == ')') countOpen--;
                        if (str[j] == ' ') {
                            if (haveWord) space = true;
                        }
                        else {
                            haveWord = true;
                            if (countOpen != 0) {
                                if (space) return false;
                            }
                        }
                        if (countOpen != 0) {
                            argum += str[j];
                            j++;
                        }
                    }
                    if (j<str.Length && str[j] == ',') notALone = true;
                    if (countOpen == 0) getArg = true;

                    if (notALone) {
                        if (!checkGr(argum)) return false;
                    }
                    else {
                        bool onlySpaces = true;
                        for (int i = 0; i < argum.Length; i++)
                            if (argum[i] != ' ') onlySpaces = false;
                        if (!onlySpaces)
                            if (!checkGr(argum)) return false;
                    }

                    haveWord = false;
                    argum = "";
                }
                j++;
            }
            if (j < str.Length) {
                for (int i = j; i< str.Length; i++)
                    if (str[i] != ' ') return false;
            }
            if (getConst) return false;
            if (!getArg) return false;
            return true;
        }

        //<assign> := <id> := <part>
        static bool checkAssign(string str) {
            if (str == "") return false;
            string word = "";
            int j = 0;
            while (j<str.Length && str[j] != ':') {
                word += str[j];
                j++;
            }
            if (j >= str.Length) return false;
            if (!checkId(word)) return false;
            if (str[j + 1] != '=') return false;
            j += 2;
            word = "";
            while (j<str.Length) {
                word += str[j];
            }
            if (!checkGr(word)) return false;

            return true;
        }

        //<id> := idd | <id>.idd | <func>.idd
        static bool checkId(string str) {
            if (str == "") return false;
            bool isFirst = true;

            bool isFunc = false;
            string word = "";
            for (int j=0; j<str.Length; j++) {
                if(str[j] != '.') {
                    if(isFirst) {
                        if (Char.IsDigit(str[j]))  return false;
                        isFirst = false;
                    }
                    if (str[j] == '(') isFunc = true;
                    word += str[j];
                } else {
                    isFunc = false;
                    if (!checkGr(word)) return false;
                    word = "";
                    isFirst = true;
                }
            }
            if (isFunc) return false;

            return true;
        }

    }
}
