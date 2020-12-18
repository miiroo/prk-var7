using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace prk_var7
{

 /*
 *  NOTE: Additional libs are used: System.IO 
 *  NOTE: You have to create a new project (Visual C# -> Console app) 
 *          and repaste all in namespace brackets
 *  NOTE: You have to build project first (ctrl+shift+b) and add code.txt to builed .exe file's folder
 *          than you can run it and test. You have to change lines in code.txt to "crash" it.
 * 
 * How it works:
 * All steps go with lines are taken one by one from file.
 * 
 * Step1: we find symbol
 * Step2: we decide what it is: letter(1)/number(2)/delim(3)/1 symbol key word(4)/space(5)/or we don't know that symbol(6)
 * 
 * (1) Letter - we add all letters and numbers till we find any other symbol or space.
 * (1).1 - check is it KEY WORD or ID. If it is ID we should check does it exist or no. If no we should add it to the ID list.
 * (1).2 - add log line and go next
 * 
 * (2) Number - we add all numbers till we find any other symbol (letter or smth else).
 * (2).1 - Numbers could be only CONST, so we should check does it exist or no. If no we should add it to the CONST list.
 * (2).2 - add log line and go next
 * 
 * (3) Delim - We have to check is it delim ' or double delim (f.e. :=)
 * (3).1.1 - if delim is ' - it means that there will be string const. We should add all symbols till next '
 * (3).1.2 - We have to check does const exist or no. If no we should add it to the CONST list.
 * (3).1.3 - add log line and go next
 * (3).2 - if it's not a string const we just add log line and go next
 * 
 * (4) 1 symbol key word - (f.e. </=/>). Just add log line and go next
 * 
 * (5) space - just go next
 * 
 * (6) Unrecognized symbol - show error + symbol and stop program.
 * 
 * Step3: if we haven't found any errors just print generated tables and log list
 * Step4: done! :)
 * 
 * This method get any word by symbols.
 * 
 */

    class Program
    {

        private static List<String> keyArray = new List<String> { "procedure", "TObject", "var", "integer", "Begin",
            "if", "and", "then", "else", "while", "not", "do", "End", "=", "+", "<", ">"};
        private static List<String> dArray = new List<String> {".", ":", ";", "'", "(", ")", ":=", "," };
        private static List<String> idArray = new List<String>();
        private static List<String> cnstArray = new List<String>();
        private static List<String> logLine = new List<String>();

        static void Main(string[] args) {
            string[] lines = File.ReadAllLines(@"code.txt");
            string word;
            Console.WriteLine("CODE:");
            foreach (string line in lines) {
                Console.WriteLine(line);
            }
            Console.WriteLine();
            Console.WriteLine();

            int exitC = 0; // 0 - don't recognize word 1 - recognized it 2 - error.
            printTable("key");
            printTable("d");


            foreach (string line in lines) {
                if (exitC != 2) {
                    for (int i = 0; i < line.Length && exitC != 2; i++) {
                        exitC = 0;
                        word = "";
                        //found letter -> id, key word////////////////////////////////
                        if (Char.IsLetter(line[i])) {
                            //find word
                            while (i < line.Length && (Char.IsLetter(line[i]) || Char.IsDigit(line[i])) && line[i] != ' ' && !dArray.Contains(line[i].ToString()) && !keyArray.Contains(line[i].ToString()) ) {
                                word += line[i];
                                i++;
                            }
                            i--;
                            //check keyWord
                            if (checkKeyWord(word)) {
                                //add to log
                                addLog(word);
                                exitC = 1;
                            }
                            else {
                                //not key word - check id and it's already created
                                if (checkId(word)) {
                                    addLog(word);
                                    //add to log 
                                    exitC = 1;
                                }
                                //current id doesn't exist
                                else {
                                    //add to ids
                                    //add to log 
                                    idArray.Add(word);
                                    addLog(word);
                                    exitC = 1;
                                }
                            }

                        }
                        //////////////////////////////////////////////

                        //found number -> numeric const///////////////////////////////
                        if (Char.IsDigit(line[i]) && exitC == 0) {
                            while (i < line.Length && Char.IsDigit(line[i])) {
                                word += line[i];
                                i++;
                            }
                            i--;
                            //const already exist
                            if (checkConst(word)) {
                                //add to log
                                addLog(word);
                                exitC = 1;
                            }
                            //const doesn't exist
                            else {
                                //add to const
                                //add to log
                                cnstArray.Add(word);
                                addLog(word);
                                exitC = 1;
                            }
                        }
                        //////////////////////////////////////////

                        //found delim///////////////////////////////////
                        if (dArray.Contains(line[i].ToString()) && exitC == 0) {
                            if (line[i] == ':' && line[i + 1] == '=') {
                                i++;
                                // add to log :=
                                addLog(":=");
                                exitC = 1;
                            }
                            else {
                                //found ' -> check string const////
                                if (line[i].ToString() == "'") {
                                    //add ' to log
                                    addLog("'");
                                    word += line[i];
                                    i++;
                                    while (line[i].ToString() != "'") {
                                        word += line[i];
                                        i++;
                                    }
                                    word += line[i];
                                    //add ' to log
                                    addLog("'");
                                    

                                    //check const
                                    if (checkConst(word)) {
                                        //add to log
                                        addLog(word);
                                        exitC = 1;
                                    }
                                    else {
                                        exitC = 1;
                                        //add to const
                                        //add to log
                                        cnstArray.Add(word);
                                        addLog(word);
                                    }
                                }
                                ///////
                                //it's some delim not string const
                                else {
                                    //add to log
                                    addLog(word);
                                    exitC = 1;
                                }
                            }
                        }
                        /////////////////////////////////////////

                        //found one symbol key word////////////////////
                        if (checkKeyWord(line[i].ToString()) && exitC == 0) {
                            addLog(line[i].ToString());
                            exitC = 1;
                        }
                        ////////////////////////////////////////

                        //just a space//////////////////////////
                        if (line[i] == ' ' && exitC == 0) {
                            exitC = 1;
                        }
                        ////////////////////////////////////////

                        if (exitC != 1) {
                            exitC = 2;
                            System.Console.WriteLine("Can't recognize symbol: " + line[i]);
                        }
                    }
                }
            }
            if (exitC != 2) {
                //print all tables
                printTable("id");
                printTable("cnst");
                printTable("log");
            }
            System.Console.ReadKey();
        }

        //return TRUE if word was founded is a key word 
        //in other case return FALSE 
        private static bool checkKeyWord(string word) {
            if (keyArray.Contains(word)) return true;
            return false;
        }


        //return TRUE if word was founded is an existed id 
        //in other case return FALSE
        private static bool checkId(string word) {
            if (idArray.Contains(word)) return true;
            return false;
        }


        //return TRUE if word was founded is an existed const
        //in other case return FALSE
        private static bool checkConst(string word) {
            if (cnstArray.Contains(word)) return true;
            return false;
        }

        //adding log line based on where this word is saved
        private static void addLog(string word) {
            if (checkConst(word)) { logLine.Add(logLine.Count + ": " + word + " in CONST table:" + cnstArray.IndexOf(word)); }
            if (checkId(word)) { logLine.Add(logLine.Count + ": " + word + " in ID table:" + idArray.IndexOf(word)); }
            if (checkKeyWord(word)) { logLine.Add(logLine.Count + ": " + word + " in KEYS table:" + keyArray.IndexOf(word)); }
            if (dArray.Contains(word)) { logLine.Add(logLine.Count + ": " + word + " in DELIM table:" + dArray.IndexOf(word)); }
        }


        //just printing cute tables, nothing special
        private static void printTable(string word) {
            int i = 0;
            switch (word) {
                case "key":
                    System.Console.WriteLine("KEY WORDS");
                    for (int j = 0; j < 50; j++) System.Console.Write("#");
                    System.Console.WriteLine();
                    foreach (string line in keyArray) {
                        System.Console.Write("#  ");
                        System.Console.Write(i + ":");
                        System.Console.Write(line);
                        for (int j = line.Length; j < 46; j++) {
                            System.Console.Write(" ");
                        }
                        System.Console.Write("#");
                        System.Console.WriteLine();
                        i++;
                    }
                    for (int j = 0; j < 50; j++) System.Console.Write("#");
                    System.Console.WriteLine();
                    System.Console.WriteLine();
                    break;

                case "d":
                    System.Console.WriteLine("DELIMETERS");
                    for (int j = 0; j < 50; j++) System.Console.Write("#");
                    System.Console.WriteLine();
                    foreach (string line in dArray) {
                        System.Console.Write("#  ");
                        System.Console.Write(i+":");
                        System.Console.Write(line);
                        for (int j = line.Length; j < 46; j++) {
                            System.Console.Write(" ");
                        }
                        System.Console.Write("#");
                        System.Console.WriteLine();
                        i++;
                    }
                    for (int j = 0; j < 50; j++) System.Console.Write("#");
                    System.Console.WriteLine();
                    System.Console.WriteLine();
                    break;

                case "id":
                    System.Console.WriteLine("ID");
                    for (int j = 0; j < 50; j++) System.Console.Write("#");
                    System.Console.WriteLine();
                    foreach (string line in idArray) {
                        System.Console.Write("#  ");
                        System.Console.Write(i + ":");
                        System.Console.Write(line);
                        for (int j = line.Length; j < 46; j++) {
                            System.Console.Write(" ");
                        }
                        System.Console.Write("#");
                        System.Console.WriteLine();
                        i++;
                    }
                    for (int j = 0; j < 50; j++) System.Console.Write("#");
                    System.Console.WriteLine();
                    System.Console.WriteLine();
                    break;
                case "cnst":
                    System.Console.WriteLine("CONST");
                    for (int j = 0; j < 50; j++) System.Console.Write("#");
                    System.Console.WriteLine();
                    foreach (string line in cnstArray) {
                        System.Console.Write("#  ");
                        System.Console.Write(i + ":");
                        System.Console.Write(line);
                        for (int j = line.Length; j < 46; j++) {
                            System.Console.Write(" ");
                        }
                        System.Console.Write("#");
                        System.Console.WriteLine();
                        i++;
                    }
                    for (int j = 0; j < 50; j++) System.Console.Write("#");
                    System.Console.WriteLine();
                    System.Console.WriteLine();
                    break;
                case "log":
                    System.Console.WriteLine("LOGS");
                    for (int j = 0; j < 50; j++) System.Console.Write("#");
                    System.Console.WriteLine();
                    foreach (string line in logLine) {
                        System.Console.Write("#  ");
                        System.Console.Write(line);
                        for (int j = line.Length; j < 46; j++) {
                            System.Console.Write(" ");
                        }
                        System.Console.Write("#");
                        System.Console.WriteLine();
                    }
                    for (int j = 0; j < 50; j++) System.Console.Write("#");
                    System.Console.WriteLine();
                    System.Console.WriteLine(); break;
            }
        }
    }
}