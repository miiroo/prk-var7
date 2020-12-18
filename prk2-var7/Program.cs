using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;



namespace prk2_var7
{
    class Program
    {
        private static List<String> keyArray = new List<String> { "procedure", "TObject", "var", "integer", "Begin",
            "if", "and", "then", "else", "while", "not", "do", "End", "=", "+", "<", ">"};
        private static List<String> dArray = new List<String> { ".", ":", ";", "'", "(", ")", ":=", "," };

        //if (CEdit1.Text='') and (Kedit2.Text='' and d=1) then ShowMessage('Please enter data');
        static void Main(string[] args) {
            string[] lines = File.ReadAllLines(@"code2.txt");
            string str = lines[0];
            Console.WriteLine("CODE:");
            Console.WriteLine(str);
            string statement = "";
            int brackets = 0;

            //there is IF and THEN
            if (findIfThen(str)) {
                //get statement between IF and THEN
                int posStart = str.IndexOf("if") + 2;
                int posEnd = str.IndexOf("then");
                for(int i = posStart; i<posEnd; i++) {
                    if (str[i] == '(') brackets++;
                    if (str[i] == ')') brackets--;
                    statement += str[i];
                }
                
                //check expression is correct
                if (checkExpr(statement) && brackets == 0) {
                    string part = "";
                    for (int i = posEnd + 4; i < str.Length; i++) part += str[i];
                    if (checkGrammar(part)) Console.WriteLine("Success");
                    else Console.WriteLine("Error: missing statement after THEN.");
                }
                else Console.WriteLine("Error: wrong bool expression.");
            }
            else {
                Console.WriteLine("Error: some troubles with IF or THEN.");
            }

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
                    if (!(j < str.Length)) return false;
                    switch (str[j]) {
                        case '<':
                            if (str[j + 1] == '=' || str[j + 1] == '>') j++;
                            part1 = checkGrammar(part);
                            delimF = true;
                            break;
                        case '>':
                            if (str[j + 1] == '=') j++;
                            part1 = checkGrammar(part);
                            delimF = true;
                            break;
                        case '=':
                            part1 = checkGrammar(part);
                            delimF = true;
                            break;
                    }
                }
                //we found delimetr but we have already found it
                //or it's end of string
                else {
                    //we found delimetr
                    if (j<str.Length && (str[j] == '<' || str[j] == '>' || str[j] == '=')) return false;
                    part2 = checkGrammar(part);
                }
            }
            if (part1 && part2) return true;
            return false;
        }
        
        //we have to check statement grammar
        //it could be fuction or math operations
        //but in our case we just check that there is smth
        static bool checkGrammar(string str) {
            for (int i=0; i<str.Length; i++) {
                if (str[i] != ' ') return true;
            } 
            return false;
        }
    }
}
