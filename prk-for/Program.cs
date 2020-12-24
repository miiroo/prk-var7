using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace prk_for
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
    * How it works:
    * Step1: We find FOR, TO, DO in right order
    * Step2: the grammar for FOR statement is: 
    *           for <assign> to <f> do <state>
    *       so we should check <assign>, <f> and <state> for no grammar mistakes
    * 
    * Grammar for <assign>, <f>, and <state>
    * 
    * 
    * 
    *     * <part> ::= <T> | <T>+<part>| <T>-<part>
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
    class Program {

        private static List<String> keyArray = new List<String> { "procedure", "TObject", "var", "integer", "Begin",
            "if", "and", "then", "else", "while", "not", "do", "End", "to", "=", "+", "<", ">", "*", "-", "/"};
        private static List<String> dArray = new List<String> { ".", ":", ";", "'", "(", ")", ":=", "," };
        static string errorMessage = "";
        static bool strongError = false;
        static int toPos = 0;
        static int doPos = 0;
        static int forPos = 0;

        //for I:=0 to 19 do P:=new(PButton, cmCalcButton);
        static void Main(string[] args) {
            string[] lines = File.ReadAllLines(@"code2.txt");
            string str = lines[0];
            Console.WriteLine("CODE:");
            Console.WriteLine(str);
            strongError = false;
            errorMessage = "";
            if (fastAn(str)) {
                string assign = "";
                string f = "";
                string state = "";
                int brackets = 0;
                for(int i=0; i<str.Length; i++) {
                    if (str[i] == '(') brackets++;
                    if (str[i] == ')') brackets--;
                }
                if (brackets == 0) {
                    if (findFor(str)) {
                        if (findTo(str)) {
                            if (findDo(str)) {
                                for (int i=forPos+3; i<toPos; i++) {
                                    assign += str[i];
                                }
                                for (int i=toPos+2; i<doPos; i++) {
                                    f += str[i];
                                }
                                int j = doPos +2;
                                for (int i=doPos+2; i<str.Length && str[i] != ';'; i++) {
                                    state += str[i];
                                    j++;
                                }
                                if (j != str.Length) {
                                    bool foundSmth = false;
                                    j++;
                                    for (int i = j; i < str.Length; i++) {
                                        if (str[i] != ' ') {
                                            foundSmth = true;
                                            i = str.Length;
                                        }
                                    }
                                        if (!foundSmth) {
                                        if (checkAssign(assign) && checkF(f) && checkState(state)) Console.WriteLine("No error found.");
                                        else {
                                            strongError = false;
                                            if (!checkAssign(assign)) Console.WriteLine("There should be assignment between FOR and TO" +errorMessage);
                                            else if (!checkF(f)) Console.WriteLine("There should be NUMBER after TO." + errorMessage);
                                                 else if (!checkState(state)) Console.WriteLine("There should be action after DO." + errorMessage);

                                        }
                                        }
                                        else Console.WriteLine("There is something after DO statement.");
                                } 
                                else Console.WriteLine("Missing ; in DO section.");
                            //get ASSIGN between for forPos and toPos and check it
                            //get F betwwen toPos and doPos and check it
                            //check state after doPos
                            //find ; in the end

                            }
                            else Console.WriteLine("Missing DO.");
                        }
                        else Console.WriteLine("Missing TO.");
                    }
                    else Console.WriteLine("Missing FOR.");
                }
                else {
                    if (brackets < 0) Console.WriteLine("Missing (");
                    if (brackets > 0) Console.WriteLine("Missing )");
                }
            }
            Console.ReadKey();
        }

        static bool findFor(string str) {
            string word = "";
            bool noWord = true;
            int j = 0;
            for(int i=0; i<str.Length; i++) {
                if (str[i] != ' ') {
                    if (str[i] == '(') {
                        if (word == "for") {
                            forPos = i - 3;
                            return true;
                        }
                        else {
                            return false;
                        } 
                    }
                    word += str[i];
                }
                else {
                    if (word == "for") {
                        if (noWord) {
                            forPos = i - 3;
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                    noWord = false;
                    word = "";
                }
                j++;
            }
            if (j == str.Length) {
                if (word == "for") return true;
            }
            return false;
        }

        static bool findTo(string str) {
            string word = "";
            int j = 0;
            for (int i = forPos; i < str.Length; i++) {
                if (str[i] != ' ') {
                    if (str[i] == '(') {
                        if (word == "to") {
                            toPos = i - 2;
                            return true;
                        }
                    }
                    word += str[i];
                    if (str[i] == ')') word = "";
                }
                else {
                    if (word == "to") {
                       toPos = i - 2;
                       return true;
                    }
                    word = "";
                }
                j++;
            }
            if (j == str.Length) {
                if (word == "to") return true;
            }
            return false;
        }

        static bool findDo(string str) {
            string word = "";
            int j = 0;
            for (int i = toPos; i < str.Length; i++) {
                if (str[i] != ' ') {
                    if (str[i] == '(') {
                        if (word == "do") {
                            doPos = i - 2;
                            return true;
                        }
                    }
                    word += str[i];
                    if (str[i] == ')') word = "";
                }
                else {
                    if (word == "do") {
                        doPos = i - 2;
                        return true;
                    }
                    word = "";
                }
                j++;
            }
            if (j == str.Length) {
                if (word == "do") return true;
            }
            return false;
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


        //<part>::=<T> | <T>+<part> | <T>-<part>
        static bool checkPart(string str) {
            if (strongError) return false;
            str = cleaning(str);
            if (str == "") {
                strongError = true;
                errorMessage += "\nError: missing statement";
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
                // errorMessage += "\nError: in " + str; //optional
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
                errorMessage += "\nError: missing statement";
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
                // errorMessage += "\nError: in " + str;
                return false;
            }
            return true;
        }

        //<F> ::= <ID> | <func> | <number> | (<part>) 
        static bool checkF(string str) {
            if (strongError) return false;
            if (str == "") {
                strongError = true;
                errorMessage += "\nError: missing statement";
                return false;
            }
            if (str[0] == '(' && str[str.Length - 1] == ')') return checkPart(str);
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
                errorMessage += "\nError: missing statement";
                return false;
            }
            bool numRow = false;
            bool getNumb = false;
            for (int i = 0; i < str.Length; i++) {
                if (Char.IsDigit(str[i])) {
                    getNumb = true;
                    if (numRow) {
                        strongError = true;
                        errorMessage = "Error: expected OPERATOR but " + str[i] + " was found";
                        return false;
                    }
                }
                if (str[i] == ' ') {
                    if (getNumb) numRow = true;
                }
                if (Char.IsLetter(str[i]) || dArray.Contains(str[i].ToString())) {
                    if (numRow) {
                        strongError = true;
                        errorMessage = "Error: expected OPERATOR but " + str[i] + " was found";
                        return false;
                    }
                    else return false;
                }
            }
            if (getNumb) return true;
            strongError = true;
            errorMessage = "Error: missing statement";
            return false;
        }

        //<id>::=<id>.idd | idd
        static bool checkId(string str) {
            if (strongError) return false;
            if (str == "") {
                strongError = true;
                errorMessage += "\nError: missing statement";
                return false;
            }
            //string word = "";
            bool isWord = false;
            for (int i = 0; i < str.Length; i++) {
                if (Char.IsDigit(str[i]) && !isWord) {
                    strongError = true;
                    errorMessage += "\nError: identifier/function can't start with digit in " + str;
                    return false;
                }
                if (Char.IsLetter(str[i])) {
                    isWord = true;
                }
                if (str[i] == '.') isWord = false;
                if (str[i] == '(') return false;
                if (dArray.Contains(str[i].ToString()) && str[i] != '(' && str[i] != '.') {
                    strongError = true;
                    errorMessage += "\nError: identifier/function can't consider current symbol " + str[i] + " in " + str;
                    return false;
                }
                if (str[i] == ' ') {
                    strongError = true;
                    errorMessage += "\nError: expected ; in " + str;
                    return false;
                }
            }
            return true;
        }

        //<func>::=<id>(param) | <id>()
        //<param> ::=<arg>,<param> | <arg>
        //<arg> ::= <strconst> | <part>
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
                errorMessage += "\nError: missing statement";
                return false;
            }
            str = cleaning(str);
            if (checkAssign(str)) { return true; }
            if (checkId(str)) { return true; }
            if (checkFunc(str)) { return true; }

            strongError = true;
            errorMessage += "\nError: incorrect statement after then: " + str;
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
                    errorMessage = "\nError: expected := but found " + str[j + 1] + " in " + str;
                    return false;
                }
                word = cleaning(word);
                if (!checkId(word)) {
                    strongError = true;
                    errorMessage = "\nError: you can assign only to identifier but there is " + word + " in " + str;
                    return false;
                }
                word = "";
                for (int i = j + 2; i < str.Length; i++) {
                    word += str[i];
                }
                word = cleaning(word);
                if (word[0].ToString() == "'" && word.Length == 1) {
                    strongError = true;
                    errorMessage += "\nError: missing ' in " + word;
                    return false;
                }
                if (word[0].ToString() == "'" && word[word.Length - 1].ToString() == "'") return true;

                if (!checkPart(word)) {
                    errorMessage += "\nError: in " + str;
                    return false;
                }
            }
            return true;
        }
    }
}

