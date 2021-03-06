﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace prk_function
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
    class Program
    {
        private static List<String> keyArray = new List<String> { "procedure",  "var", "integer", "Begin",
        "if", "and", "then", "else", "while", "not", "do", "End", "in", "=", "+", "<", ">", "*", "-", "/"};
        private static List<String> dArray = new List<String> { ".", ":", ";", "'", "(", ")", ":=", ",", "[", "]" };
        private static string errorMessage = "";
        private static bool strongError = false;


        static void Main(string[] args) {
            string[] lines = File.ReadAllLines(@"code2.txt");
            string str = lines[0];
            Console.WriteLine("CODE:");
            Console.WriteLine(str);
            strongError = false;
            errorMessage = "";
            if (fastAn(str)) {
                int brackets = 0;
                for (int i = 0; i < str.Length; i++) {
                    if (str[i] == '(') brackets++;
                    if (str[i] == ')') brackets--;
                }
                if (brackets == 0) {
                    string func = "";
                    int j = 0;
                    while (j < str.Length && str[j] != ';') {
                        func += str[j];
                        j++;
                    }
                    if (checkFunc(func)) {
                        if (j == str.Length) {
                            Console.WriteLine("Missing ;");
                        }
                        else {
                            j++;
                            while (j < str.Length && j == ' ') j++;
                            if (j == str.Length) {
                                Console.WriteLine("No error found. Parse completed successfully");
                            }
                            else Console.WriteLine("There is something after.");
                        }
                    }
                    else {
                        Console.WriteLine(errorMessage);
                    }
                }
                else {
                    if (brackets < 0) Console.WriteLine("Missing (");
                    if (brackets > 0) Console.WriteLine("Missing )");
                }
            }
            Console.ReadKey();

        }


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

        static bool fastAn(string line) {
            int i = 0;
        //    foreach (string line in str) {
                while (i < line.Length) {
                    if (Char.IsDigit(line[i]) || Char.IsLetter(line[i]) || dArray.Contains(line[i].ToString()) || keyArray.Contains(line[i].ToString()) || line[i] == ' ') i++;
                    else {
                        Console.WriteLine("Can't recognize that symbol: " + line[i]);
                        return false;
                    }
                }
         //   }
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

        //<F> ::= <ID> | <func> | <number> | (<part>) | <structure>
        static bool checkF(string str) {
            if (strongError) return false;
            if (str == "") {
                strongError = true;
                errorMessage += "\nError. missing statement";
                return false;
            }
      //      if (checkStructure(str)) return true;
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
                    if (str[i] == '.') {
                        if (i+1<str.Length && str[i+1]!= '.')
                        isWord = false;
                        else {
                            strongError = true;
                            errorMessage += "\nError. No ID after . in " + str;
                            return false;
                        }
                    }
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
         //   if (isWord == true)
                return true;
      //      else {
       //         strongError = true;
      //          errorMessage += "\nError. No ID after . in " + str;
     //           return false;
     //       }
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
                    if (word == "") {
                        strongError = true;
                        errorMessage += "\nMissing argument in " + str;
                        return false;
                    }
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

    }
}
