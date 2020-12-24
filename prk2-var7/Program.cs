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
    * NOTE: if you want to use that grammar for yourself, please, change some names.
    * 
    * if <bool st> then <state>
    * <state> ::= <assign>|<func>|<id>  (it could be ID coz pascal got functions without () )
    * <assign> ::= <id>:=<smth>
    * 
    * <bool state> ::= <expr><bool operand><bool state> | <expr>
    * <bool operand> ::= AND | OR
    * 
    * <expr> ::= <smth><compare symbol><smth>
    * <compare symbol> ::= < | > | = | <= | >= | <>
    * 
    * 
    * <part> ::= <T> | <T>+<part>| <T>-<part>
    * <T> ::= <F> | <F>*<F> | <F>/<F>
    * <F> ::= <id> | <func> | <number> | (<part>)
    * 
    * <number> ::= digit<number>|digit
    * <strconst> ::= '<symbols>'
    * <symbols> ::= all possible symbols
    * 
    * <func> ::= <id>(<param>) | <id>()
    * <param> ::= <smth>,<param> | <smth>
    * 
    * <id> ::= <id>.idd | idd    (idd - it's identifier from previous lab)
    * 
    * 
    * <smth> ::= <part> | <strconst>
    * 
    * 
    */


    class Program
    {
        private static List<String> keyArray = new List<String> { "procedure", "TObject", "var", "integer", "Begin",
            "if", "and", "then", "else", "while", "not", "do", "End", "=", "+", "<", ">", "*", "-", "/"};
        private static List<String> dArray = new List<String> { ".", ":", ";", "'", "(", ")", ":=", "," };
        private static string errorMessage = "";
        private static bool strongError = false;
        //if (CEdit1.Text='') and (Kedit2.Text='' and d=1) then ShowMessage('Please enter data');
        static void Main(string[] args) {
            string[] lines = File.ReadAllLines(@"code2.txt");
            string str = lines[0];
            Console.WriteLine("CODE:");
            Console.WriteLine(str);
            strongError = false;
            errorMessage = "";
            if (fastAn(str)) {
                string statement = "";
                int brackets = 0;
                if (fastAn(str)) {

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
                        bool just = false;
                        //check expression is correct
                        if (brackets == 0) {
                            if (checkBoolSt(statement)) {
                                string part = "";
                                for (int i = posEnd + 4; i < str.Length; i++) {
                                    if (str[i] != ';') part += str[i];
                                    else {
                                        i++;
                                        while (i < str.Length && str[i] == ' ') i++;
                                        if (i < str.Length) just = true;
                                        i = str.Length;
                                    }
                                }
                                if (checkState(part) && !just) {
                                    brackets = 0;
                                    for (int i = str.IndexOf(part); i < str.Length; i++) {
                                        if (str[i] == '(') brackets++;
                                        if (str[i] == ')') brackets--;
                                    }
                                    if (brackets == 0)Console.WriteLine( "Success");
                                    else {
                                       Console.WriteLine("Error massage - missing ( or )");
                                    }
                                }

                                else {
                                   Console.WriteLine( "Error message -  wrong or missing statement after THEN.");
                                   Console.WriteLine(errorMessage);
                                }
                            }
                            else {
                                //  Console.WriteLine("Error message -  error in expression");
                               Console.WriteLine(errorMessage);
                               Console.WriteLine("\nErro message - error in bool expression");
                            }
                        }
                        else {
                            if (brackets > 0)Console.WriteLine("Error message -  ) not found between IF and THEN (in bool expression)");
                            if (brackets < 0)Console.WriteLine("Error message -  ( not found between IF and THEN (in bool expression)");
                        }
                    }
                    else {
                       Console.WriteLine("\nError message -  some troubles with IF or THEN.");
                    }
                }
            }
            Console.ReadKey();
        }

        //cleaning function. Returns string without additional spaces and brackets in it's start and end
        static string cleaning(string str) {
            if (str == "") {
                return "";
            }
            //postion of last (
            int posOpen = 0;
            while (posOpen < str.Length && str[posOpen] == ' ' || str[posOpen] == '(') {
                if (str[posOpen] == ' ')
                    str = str.Remove(posOpen, 1);
                else posOpen++;

                if (str == "") return "";
            }
            if (str == "") {
                return "";
            }
            posOpen--;
            if (posOpen == str.Length) return "";

            int posEnd = str.Length - 1;
            while (str != "" && posEnd > 0 && str[posEnd] == ' ' || str[posEnd] == ')') {
                if (str[posEnd] == ' ')
                    str = str.Remove(posEnd, 1);
                posEnd--;
                if (str == "") return "";
            }
            if (str == "") {
                return "";
            }
            posEnd++;
            if (posEnd == 0) return "";

            int brackets = 0;
            int lastPos = posOpen + 1;
            if (posOpen >= 0 && posOpen < str.Length && str[posOpen] == '(') {

                while (posOpen >= 0) {
                    while (lastPos < str.Length && (str[lastPos] != ')' || brackets != 0)) {
                        if (str[lastPos] == '(') brackets++;
                        if (str[lastPos] == ')') brackets--;
                        lastPos++;
                    }

                    if (lastPos == str.Length) {
                        while (posOpen >= 0) {
                            str = str.Remove(posOpen, 1);
                            posOpen--;
                        }
                        return str;
                    }
                    posOpen--;
                    lastPos++;
                }

                posOpen = 0;
                posEnd = str.Length - 1;
                while (str[posOpen] == '(' && str[posEnd] == ')') {
                    str = str.Remove(posEnd, 1);
                    posEnd--;
                    str = str.Remove(posOpen, 1);
                }

            }
            else {
                brackets = 0;
                lastPos = posEnd - 1;
                if (posEnd < str.Length && posEnd >= 0 && str[posEnd] == ')') {
                    while (posEnd < str.Length) {
                        while (lastPos > 0 && (str[lastPos] != '(' || brackets != 0)) {
                            if (str[lastPos] == '(') brackets++;
                            if (str[lastPos] == ')') brackets--;
                            lastPos--;
                        }

                        if (lastPos == 0) {
                            while (posEnd < str.Length) {
                                str = str.Remove(posEnd, 1);
                            }
                            return str;
                        }
                        posEnd++;
                        lastPos--;
                    }

                    posOpen = 0;
                    posEnd = str.Length - 1;
                    while (str[posOpen] == '(' && str[posEnd] == ')') {
                        str = str.Remove(posEnd, 1);
                        posEnd--;
                        str = str.Remove(posOpen, 1);
                    }
                }
            }
            return str;
        }

        static bool fastAn(string str) {
            int i = 0;
            while (i < str.Length) {
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
                    else {
                       Console.WriteLine("Error message -  IF not found.");
                        return false;
                    }
                }
                else {
                    if (word == "if") {
                       Console.WriteLine("Error message -  find IF but it's already exist.");
                        return false;
                    }
                }

                if (word == "then" && !thenFounded) {
                    //symbol before THEN should be ) or space
                    //symbol after THEN should be space
                    if (str.IndexOf("then") + 4 <= str.Length) {
                        if ((str[str.IndexOf("then") - 1] == ' ' || str[str.IndexOf("then") - 1] == ')') && str[str.IndexOf("then") + 4] == ' ') thenFounded = true;
                        else {
                           Console.WriteLine("Error message -  THEN not found.");
                            return false;
                        }
                    }
                }
                else {
                    if (word == "then") {
                       Console.WriteLine("Error message -  THEN was found, but it's already exist");
                        return false;
                    }
                }

            }
            if (ifFounded && thenFounded) return true;
           Console.WriteLine( "Error message -  IF and THEN not found.");
            return false;
        }


        //<bool st> ::= <expr> and/or <bool st> | <expr>
        static bool checkBoolSt(string str) {
            // str = cleaning(str);
            if (strongError) return false;
            string word1 = "";
            string word2 = "";
            bool foundAndOr = false;
            for (int i = 0; i < str.Length; i++) {
                if (!foundAndOr) {
                    if (str[i] != ' ') word1 += str[i];
                    else {
                        if (word1 == "and" || word1 == "or") {
                            word1 = "";
                            foundAndOr = true;
                            if (!checkExpr(word2)) {
                            //    errorMessage += "\nError message - in " + str; //optional
                                return false;
                            }
                            word2 = "";
                        }
                        else {
                            word2 += str[i];
                            word2 += word1;
                            word1 = "";
                        }
                    }
                }
                else {
                    word1 += str[i];
                }
            }
            word2 += " ";
            word2 += word1;
            if (!foundAndOr) {
                if (!checkExpr(word2)) {
                 //   errorMessage += "\nError message - in " + str; //optional
                    return false;
                }
            }
            else {
                if (!checkBoolSt(word1)) {
                //    errorMessage += "\nError message - in " + str; //optional
                    return false;
                }
            }
            return true;
        }


        //<expr> ::= <smth> </>/=/<=/>= <smth>
        static bool checkExpr(string str) {
            //  str = cleaning(str);
            if (strongError) return false;
            string word = "";
            bool findCompare = false;
            bool isConst = false;
            for (int i = 0; i < str.Length; i++) {
                if (!isConst) {
                    if (str[i].ToString() == "'") isConst = true;
                    if (str[i] != '<' && str[i] != '>' && str[i] != '=') word += str[i];
                    else {
                        if (findCompare) {
                            strongError = true;
                            errorMessage = "\nError message - compare symbol already founded";
                            return false;
                        }

                        switch (str[i]) {
                            case '<':
                                findCompare = true;
                                if (i + 1 >= str.Length) {
                                    strongError = true;
                                    errorMessage += "\nError message - missing expression in " + str;
                                    return false;
                                }
                                if (str[i + 1] == '=' || str[i + 1] == '>') i++;
                                if (!checkSmth(word)) {
                                //    errorMessage += "\nError message - in " + str; //optional
                                    return false;
                                }

                                break;
                            case '>':
                                findCompare = true;
                                if (i + 1 >= str.Length) {
                                    strongError = true;
                                    errorMessage += "\nError message - missing expression in " + str;
                                    return false;
                                }
                                if (str[i + 1] == '=') i++;
                                if (!checkSmth(word)) {
                                  //  errorMessage += "\nError message - in " + str; //optional
                                    return false;
                                }
                                break;
                            case '=':
                                findCompare = true;
                                if (i + 1 >= str.Length) {
                                    strongError = true;
                                    errorMessage += "\nError message - missing expression in " + str;
                                    return false;
                                }
                                if (!checkSmth(word)) {
                                 //   errorMessage += "\nError message - in " + str; //optional
                                    return false;
                                }
                                break;
                        }
                        word = "";
                    }
                }
                else {
                    if (str[i].ToString() == "'") isConst = false;
                    word += str[i];
                }
            }
            if (!findCompare) {
                strongError = true;
                errorMessage += "\nError message - missing boolean expression in " + str;
                return false;
            }
            if (!checkSmth(word)) {
                //   errorMessage += "\nError message - in " + str; //optional
                return false;
            }
            return true;
        }

        //<smth> ::= <part> | <strconst>
        static bool checkSmth(string str) {
            if (strongError) return false;
            str = cleaning(str);
            if (str == "") {
                errorMessage += "\nError message - missing expression.";
                return false;
            }
            if (str[0].ToString() == "'" && str[str.Length - 1].ToString() == "'") return true;
            else {
                if (!checkPart(str)) {
              //      errorMessage += "\nError message - in " + str; //optional
                    return false;
                }
            }
            return true;
        }


        //<part>::=<T> | <T>+<part> | <T>-<part>
        static bool checkPart(string str) {
            if (strongError) return false;
            str = cleaning(str);
            if (str == "") {
                strongError = true;
                errorMessage += "\nError message - missing statement";
                return false;
            }

            string word = "";
            int countBr = 0;
            bool exVar = true;
            int j = 0;
            while (exVar && j < str.Length) {
                if (str[j] == '(') { countBr++; }
                if (str[j] == ')') { countBr--; }

                if (str[j] == '+' || str[j] == '-') {
                    if (countBr == 0) {
                        if (!checkT(word)) {
                            return false;
                        }
                        word = "";
                    }
                    else word += str[j];
                }
                else word += str[j];
                j++;
            }
            if (!checkT(word)) {
                // errorMessage += "\nError message - in " + str; //optional
                return false;
            }
            return true;
        }

        //<T> ::= <F> | <F>*<F> | <F> / <F>
        static bool checkT(string str) {
            if (strongError) return false;
            str = cleaning(str);
            if (str == "") {
                strongError = true;
                errorMessage += "\nError message - missing statement";
                return false;
            }
            string word = "";
            int countBr = 0;
            bool exVar = true;
            int j = 0;
            while (exVar && j < str.Length) {
                if (str[j] == '(') { countBr++; }
                if (str[j] == ')') { countBr--; }

                if (str[j] == '*' || str[j] == '/') {
                    if (countBr == 0) {
                        if (!checkF(word)) {
                            return false;
                        }
                        word = "";
                    }
                    else word += str[j];
                }
                else word += str[j];
                j++;
            }
            if (!checkF(word)) {
                // errorMessage += "\nError message - in " + str;
                return false;
            }
            return true;
        }

        //<F> ::= <ID> | <func> | <number> | (<part>) 
        static bool checkF(string str) {
            if (strongError) return false;
            if (str == "") {
                strongError = true;
                errorMessage += "\nError message - missing statement";
                return false;
            }
            if (str[0] == '(' && str[str.Length-1] == ')') return checkPart(str);
            if (checkNumb(str)) return true;
            if (checkId(str)) return true;
            if (checkFunc(str)) return true;
            return false;
        }

        //<number> ::= digit<number>|digit
        static bool checkNumb(string str) {
            if (strongError) return false;
            if (str == "") {
                strongError = true;
                errorMessage += "\nError message - missing statement";
                return false;
            }
            bool numRow = false;
            bool getNumb = false;
            for (int i = 0; i < str.Length; i++) {
                if (Char.IsDigit(str[i])) {
                    getNumb = true;
                    if (numRow) {
                        strongError = true;
                        errorMessage = "Error message - expected OPERATOR but " + str[i] + " was found";
                        return false;
                    }
                }
                if (str[i] == ' ') {
                    if (getNumb) numRow = true;
                }
                if (Char.IsLetter(str[i]) || dArray.Contains(str[i].ToString())) {
                    if (numRow) {
                        strongError = true;
                        errorMessage = "Error message - expected OPERATOR but " + str[i] + " was found";
                        return false;
                    }
                    else return false;
                }
            }
            if (getNumb) return true;
            strongError = true;
            errorMessage = "Error message - missing statement";
            return false;
        }

        //<id>::=<id>.idd | idd
        static bool checkId(string str) {
            if (strongError) return false;
            if (str == "") {
                strongError = true;
                errorMessage += "\nError message - missing statement";
                return false;
            }
            //string word = "";
            bool isWord = false;
            for (int i = 0; i < str.Length; i++) {
                if (Char.IsDigit(str[i]) && !isWord) {
                    strongError = true;
                    errorMessage += "\nError message - identifier/function can't start with digit in " + str;
                    return false;
                }
                if (Char.IsLetter(str[i])) {
                    isWord = true;
                }
                if (str[i] == '.') isWord = false;
                if (str[i] == '(') return false;
                if (dArray.Contains(str[i].ToString()) && str[i] != '(' && str[i] != '.') {
                    strongError = true;
                    errorMessage += "\nError message - identifier/function can't consider current symbol " + str[i] + " in " + str;
                    return false;
                }
                if (str[i] == ' ') {
                    strongError = true;
                    errorMessage += "\nError message - expected ; in " + str;
                    return false;
                }
            }
            return true;
        }

        static bool checkFunc(string str) {
            if (strongError) return false;
            if (str == "") {
                strongError = true;
                errorMessage += "\nError: missing statement";
                return false;
            }

            bool isConst = false;
            string word = "";
            int j = 0;
            while (j < str.Length && str[j] != '(') {
                word += str[j];
                j++;
            }
            if (!checkId(word)) {
                //       errorMessage += "\nError: in " + str; //optional
                return false;
            }
            bool isArg = false;
            j++;
            word = "";
            int brackets = 1;
            while (j < str.Length && brackets != 0) {
                if (str[j] == '(' && !isConst) brackets++;
                if (str[j] == ')' && !isConst) brackets--;
                if (str[j].ToString() == "'") isConst = !isConst;
                if (brackets != 0 && str[j] != ',' && !isConst) word += str[j];
                if (str[j] == ',' && !isConst) {
                    isArg = true;
                    word = cleaning(word);
                    if (word[0].ToString() == "'" && word[word.Length - 1].ToString() == "'") word = "";
                    else {
                        if (!checkPart(word)) {
                            //   errorMessage += "\nError: in " + str; //optional
                            return false;
                        }
                        word = "";
                    }
                }
                if (isConst) word += str[j];
                j++;
            }
            if (brackets != 0) {
                strongError = true;
                errorMessage = "\nError: missing ) in " + str;
                return false;
            }
            if (isArg) {
                word = cleaning(word);
                if (word == "") {
                    strongError = true;
                    errorMessage += "\nError: missing argument in " + str;
                    return false;
                }
                if (word[0].ToString() == "'" && word[word.Length - 1].ToString() == "'") return true;
                if (!checkPart(word)) {
                    //   errorMessage += "\nError: in " + str; //optional
                    return false;
                }
            }
            if (j < str.Length) {
                for (int i = j; i < str.Length; i++) {
                    if (str[i] != ' ' || str[i] != ')') {
                        strongError = true;
                        errorMessage += "\nError: expected ; in " + str;
                        return false;
                    }
                }
            }
            return true;
        }


        //<state>::=<assign> | <func> | <id>
        static bool checkState(string str) {
            if (strongError) return false;
            if (str == "") {
                strongError = true;
                errorMessage += "\nError message - missing statement";
                return false;
            }
            str = cleaning(str);
            if (checkAssign(str)) { return true; }
            if (checkId(str)) { return true; }
            if (checkFunc(str)) { return true; }

            strongError = true;
            errorMessage += "\nError message - incorrect statement after then: " + str;
            return false;
        }

        static bool checkAssign(string str) {
            string word = "";
            int j = 0;
            while (j < str.Length && str[j] != ':') {
                word += str[j];
                j++;
            }
            if (j >= str.Length) {
                return false;
            }
            else {
                if (str[j + 1] != '=') {
                    strongError = true;
                    errorMessage = "\nError message - expected := but found " + str[j + 1] + " in " + str;
                    return false;
                }
                word = cleaning(word);
                if (!checkId(word)) {
                    strongError = true;
                    errorMessage = "\nError message - you can assign only to identifier but there is " + word + " in " + str;
                    return false;
                }
                word = "";
                for (int i = j + 2; i < str.Length; i++) {
                    word += str[i];
                }
                word = cleaning(word);
                if (word[0].ToString() == "'" && word.Length == 1) {
                    strongError = true;
                    errorMessage += "\nError message - missing ' in " + word;
                    return false;
                }
                if (word[0].ToString() == "'" && word[word.Length - 1].ToString() == "'") return true;

                if (!checkPart(word)) {
                    errorMessage += "\nError message - in " + str;
                    return false;
                }
            }
            return true;
        }

    }
}
