using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace prk_while
{
    class Program
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
*/

            private static List<String> keyArray = new List<String> { "procedure",  "var", "integer", "Begin",
        "if", "and", "then", "else", "while", "not", "do", "End", "in", "=", "+", "<", ">", "*", "-", "/"};
            private static List<String> dArray = new List<String> { ".", ":", ";", "'", "(", ")", ":=", ",", "[", "]" };
            private static string errorMessage = "";
            private static bool strongError = false;


            /*
            while count <> 0 do
            begin
            p:=nexline(p);
            dec(count);
            end;
            */
            static void Main(string[] args) {
                string[] lines = File.ReadAllLines(@"file.txt");
                //string str = lines[0];
                Console.WriteLine("CODE:");
                foreach (string line in lines) {
                    Console.WriteLine(line);
                }
                strongError = false;
                errorMessage = "";
                string str = "";

                if (fastAn(lines)) {
                    string statement = "";
                    int brackets = 0;
                    foreach (string line in lines) {
                        str += line + " ";
                    }
                    if (findIfThen(str)) {
                        //get statement between WHILE and DO
                        int posStart = str.IndexOf("while") + 5;
                        int posEnd = str.IndexOf("do");
                        for (int i = posStart; i < posEnd; i++) {
                            statement += str[i];
                        }
                    for (int i = 0; i < str.Length; i++) {
                       if (str[i] == '(') brackets++;
                       if (str[i] == ')') brackets--;
                    }
                    //check expression is correct
                    if (brackets == 0) {
                            if (checkExpr(statement)) {
                                if (str.ToLower().Contains(" begin ")) {
                                    posStart = posEnd;
                                    string word = "";
                                    if (str.ToLower().Contains(" end")) {
                                        posEnd = str.ToLower().IndexOf(" end");
                                        for (int i = str.ToLower().IndexOf(" begin ") + 7; i < posEnd; i++) {
                                            if (str[i] == ';') {
                                                if (checkState(word)) {
                                                    word = "";
                                                }
                                                else i = str.Length;
                                            }
                                            else word += str[i];
                                        }
                                        if (cleaning(word) != "") {
                                        if (checkState(word)) {
                                            int k = str.Length - 1;
                                            while (str[k] == ' ') k--;
                                            if (str[k] != ';' && k != posEnd + 3) Console.WriteLine("\nThere is something after END.");
                                            else {
                                                if (str[k] == ';' && posEnd + 4 != k) Console.WriteLine("\nThere is something after END.");
                                            }
                                            if (str[posEnd + 4] != ';') Console.WriteLine("\nMissing ; after END.");
                                            else
                                                Console.WriteLine("\nNo errors. Parse completed successfully.");
                                        }
                                    }
                                        else {
                                           int k = str.Length - 1;
                                           while (str[k] == ' ') k--;
                                           if (str[k] != ';' && k != posEnd+3) Console.WriteLine("\nThere is something after END.");
                                           else {
                                            if (str[k] == ';' && posEnd+4 != k) Console.WriteLine("\nThere is something after END.");
                                           }
                                           if (str[posEnd + 4] != ';') Console.WriteLine("\nMissing ; after END.");
                                           else
                                            Console.WriteLine("\nNo errors. Parse completed successfully.");    
                                        }
                                    }
                                    else {
                                        Console.WriteLine("Error. Missing END.");
                                    }
                                }
                                else {
                                    string word = "";
                                    for (int i = posEnd + 4; i < str.Length; i++) {
                                        word += str[i];
                                    }
                                    if (checkState(word) && str.Length-2 == ';') Console.WriteLine("No errors. Parse completed successfully.");
                                }
                            }
                        }
                        else {
                            if (brackets < 0) Console.WriteLine("Error. Missing (");
                            if (brackets > 0) Console.WriteLine("Error. Missin )");
                        }
                    }
                    else Console.WriteLine("\n" + errorMessage);
                }
                if (strongError) Console.WriteLine("\n" + errorMessage);
                Console.ReadKey();
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
                    //there should be one IF and one THEN in other case it's Mistakes are found.
                    if (word == "while" && !ifFounded) {
                        //symbol after IF should be ( or space
                        if (str[i] == '(' || str[i] == ' ') ifFounded = true;
                        else {
                            Console.WriteLine("Mistakes are found. WHILE not found.");
                            return false;
                        }
                    }
                    else {
                        if (word.ToLower() == "while") {
                            Console.WriteLine("Mistakes are found. find WHILE but it's already exist.");
                            return false;
                        }
                    }

                    if (word.ToLower() == "do" && !thenFounded) {
                        //symbol before THEN should be ) or space
                        //symbol after THEN should be space
                        if (str.IndexOf("do") + 2 <= str.Length) {
                            if ((str[str.IndexOf("do") - 1] == ' ' || str[str.IndexOf("do") - 1] == ')') && str[str.IndexOf("do") + 2] == ' ') thenFounded = true;
                            else {
                                Console.WriteLine("Mistakes are found. DO not found.");
                                return false;
                            }
                        }
                    }
                    else {
                        if (word.ToLower() == "do") {
                            Console.WriteLine("Mistakes are found. THEN was found, but it's already exist");
                            return false;
                        }
                    }

                }
                if (ifFounded && thenFounded) return true;
                if (!ifFounded) errorMessage += "\nMistakes are found. WHILE missing.";
                if (!thenFounded) errorMessage += "\nMistakes are found. DO missing.";
                // Console.WriteLine( "Mistakes are found. IF and THEN not found.");
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

            static bool fastAn(string[] str) {
                int i = 0;
                foreach (string line in str) {
                    while (i < str.Length) {
                        if (Char.IsDigit(line[i]) || Char.IsLetter(line[i]) || dArray.Contains(line[i].ToString()) || keyArray.Contains(line[i].ToString()) || line[i] == ' ') i++;
                        else {
                            Console.WriteLine("Can't recognize that symbol: " + line[i]);
                            return false;
                        }
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
                            errorMessage = "\nMistakes are found.compare symbol already founded";
                            return false;
                        }

                        switch (str[i]) {
                            case '<':
                                findCompare = true;
                                if (i + 1 >= str.Length) {
                                    strongError = true;
                                    errorMessage += "\nMistakes are found.missing expression in " + str;
                                    return false;
                                }
                                if (str[i + 1] == '=' || str[i + 1] == '>') i++;
                                if (!checkSmth(word)) {
                                    //    errorMessage += "\nMistakes are found.in " + str; //optional
                                    return false;
                                }

                                break;
                            case '>':
                                findCompare = true;
                                if (i + 1 >= str.Length) {
                                    strongError = true;
                                    errorMessage += "\nMistakes are found.missing expression in " + str;
                                    return false;
                                }
                                if (str[i + 1] == '=') i++;
                                if (!checkSmth(word)) {
                                    //  errorMessage += "\nMistakes are found.in " + str; //optional
                                    return false;
                                }
                                break;
                            case '=':
                                findCompare = true;
                                if (i + 1 >= str.Length) {
                                    strongError = true;
                                    errorMessage += "\nMistakes are found.missing expression in " + str;
                                    return false;
                                }
                                if (!checkSmth(word)) {
                                    //   errorMessage += "\nMistakes are found.in " + str; //optional
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
            /*  if (!findCompare) {
                  checkSmth(word);

                  strongError = true;
                  errorMessage += "\nMistakes are found.missing boolean expression in " + str;
                  return false;
              }*/
            if (!checkSmth(word)) {
                //   errorMessage += "\nMistakes are found.in " + str; //optional
                return false;
            }
            return true;
        }


            //<smth> ::= <part> | <strconst>
            static bool checkSmth(string str) {
                if (strongError) return false;
                str = cleaning(str);
                if (str == "") {
                    errorMessage += "\nError. missing expression.";
                    return false;
                }
                if (str[0].ToString() == "'" && str[str.Length - 1].ToString() == "'") return true;
                else {
                    if (!checkPart(str)) {
                        //      errorMessage += "\nError. in " + str; //optional
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
                    errorMessage += "\nError. missing statement";
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
                    // errorMessage += "\nError. in " + str; //optional
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
                    errorMessage += "\nError. missing statement";
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
                    // errorMessage += "\nError. in " + str;
                    return false;
                }
                return true;
            }

            //<F> ::= <ID> | <func> | <number> | (<part>) |
            static bool checkF(string str) {
                if (strongError) return false;
                if (str == "") {
                    strongError = true;
                    errorMessage += "\nError. missing statement";
                    return false;
                }
             //   if (checkStructure(str)) return true;
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
                    errorMessage += "\nError. missing statement";
                    return false;
                }
                bool numRow = false;
                bool getNumb = false;
                for (int i = 0; i < str.Length; i++) {
                    if (Char.IsDigit(str[i])) {
                        getNumb = true;
                        if (numRow) {
                            strongError = true;
                            errorMessage = "Error. expected OPERATOR but " + str[i] + " was found";
                            return false;
                        }
                    }
                    if (str[i] == ' ') {
                        if (getNumb) numRow = true;
                    }
                    if (Char.IsLetter(str[i]) || dArray.Contains(str[i].ToString())) {
                        if (numRow) {
                            strongError = true;
                            errorMessage = "Error. expected OPERATOR but " + str[i] + " was found";
                            return false;
                        }
                        else return false;
                    }
                }
                if (getNumb) return true;
                strongError = true;
                errorMessage = "Error. missing statement";
                return false;
            }

            //<id>::=<id>.idd{[part]} | idd{[part]} |
            static bool checkId(string str) {
                if (strongError) return false;
                if (str == "") {
                    strongError = true;
                    errorMessage += "\nError: missing statement";
                    return false;
                }
                //string word = "";
                bool isWord = false;
                bool isPart = false;
                string part = "";
                for (int i = 0; i < str.Length; i++) {
                    if (!isPart) {
                        if (str[i] == '@' && !isWord) {
                            isWord = true;
                        }
                        else {
                            if (str[i] == '@' && isWord) {
                                strongError = true;
                                errorMessage += "\nError: identifier can't contain @ in its middle.";
                                return false;
                            }
                        }
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
                        if (str[i] == '[') {
                            isWord = false;
                            isPart = true;
                        }
                        if (dArray.Contains(str[i].ToString()) && str[i] != '(' && str[i] != '.' && !isPart && str[i] != '@') {
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
                    else {
                        if (str[i] != ']') part += str[i];
                        else {
                            isPart = false;
                            if (!checkPart(part)) {
                                return false;
                            }
                            part = "";
                        }
                    }
                }
                return true;
            }

            

            static bool checkFunc(string str) {
                if (strongError) return false;
                if (str == "") {
                    strongError = true;
                    errorMessage += "\nError. missing statement";
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
                    //       errorMessage += "\nError. in " + str; //optional
                    return false;
                }
                bool isArg = false;
                bool wasConst = false;
                j++;
                word = "";
                int brackets = 1;
                while (j < str.Length && brackets != 0) {
                    if (str[j] == '(' && !isConst) brackets++;
                    if (str[j] == ')' && !isConst) brackets--;
                    if (str[j].ToString() == "'") {
                        if (isConst) wasConst = true;
                        isConst = !isConst;
                    }

                    if (brackets != 0 && str[j] != ',' && !isConst && str[j].ToString() != "'") {
                        if (wasConst) {
                            strongError = true;
                            errorMessage += "\nError.   missing , in atributes";
                            return false;
                        }
                        word += str[j];
                    }

                    if (str[j] == ',' && !isConst) {
                        wasConst = false;
                        isArg = true;
                        word = cleaning(word);
                        if (word[0].ToString() == "'" && word[word.Length - 1].ToString() == "'") word = "";
                        else {
                            if (!checkPart(word)) {
                                //   errorMessage += "\nError. in " + str; //optional
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
                    errorMessage = "\nError. missing ) in " + str;
                    return false;
                }
                if (isArg) {
                    word = cleaning(word);
                    if (word == "") {
                        strongError = true;
                        errorMessage += "\nError. missing argument in " + str;
                        return false;
                    }
                    if (word[0].ToString() == "'" && word[word.Length - 1].ToString() == "'") return true;
                    if (!checkPart(word)) {
                        //   errorMessage += "\nError. in " + str; //optional
                        return false;
                    }
                }
                if (j < str.Length) {
                    for (int i = j; i < str.Length; i++) {
                        if (str[i] != ' ' || str[i] != ')') {
                            strongError = true;
                            errorMessage += "\nError. expected ; in " + str;
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
                    errorMessage += "\nError. missing statement";
                    return false;
                }
                str = cleaning(str);
                if (checkAssign(str)) { return true; }
                if (checkId(str)) { return true; }
                if (checkFunc(str)) { return true; }

                strongError = true;
                errorMessage += "\nError. incorrect statement: " + str;
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
                        errorMessage = "\nError. expected := but found " + str[j + 1] + " in " + str;
                        return false;
                    }
                    word = cleaning(word);
                    if (!checkId(word)) {
                        strongError = true;
                        errorMessage = "\nError. you can assign only to identifier but there is " + word + " in " + str;
                        return false;
                    }
                    word = "";
                    for (int i = j + 2; i < str.Length; i++) {
                        word += str[i];
                    }
                    word = cleaning(word);
                    if (word[0].ToString() == "'" && word.Length == 1) {
                        strongError = true;
                        errorMessage += "\nError. missing ' in " + word;
                        return false;
                    }
                    if (word[0].ToString() == "'" && word[word.Length - 1].ToString() == "'") return true;

                    if (!checkPart(word)) {
                        errorMessage += "\nError. in " + str;
                        return false;
                    }
                }
            return true;
        }
    }
}

