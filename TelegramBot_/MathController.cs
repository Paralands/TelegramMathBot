using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot_
{
    public static class MathController
    {
        public static List<string> numbers = new List<string>()
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "0"
        };

        public static List<string> mathOperations = new List<string>()
        {
            "+", "-", "*", "/", "^", "!", "(", ")"
        };

        public static Tuple<double, string> DoMath(string msg)
        {
            bool b_answer = CheckExpression(ConvertBrackets(GetMathSymbols(msg)));

            if (!b_answer)
            {
                return new Tuple<double, string>(default, Config.MATH_NO_EXPRESSION_TEXT);
            }
            else
            {
                return new Tuple<double, string>(Count(GetExpression(msg)), "");
            }
        }

        private static double Count(List<string> list)
        {
            int firstBracket = list.FindMathOperation("(");
            int secondBracket = list.FindMathOperation(")");

            if (firstBracket != -1 && secondBracket != -1)
            {
                list = CountInBrackets(list, firstBracket, secondBracket);
                return Count(list);
            }

            int indexDivide = list.FindMathOperation("/");
            int indexMultiply = list.FindMathOperation("*");
            int indexFactorial = list.FindMathOperation("!");
            int indexPower = list.FindMathOperation("^");
            
            if (indexFactorial >= 1)
            {
                if (Int32.TryParse(list[indexFactorial-1], out int numBeforeFactorial) && numBeforeFactorial >= 1)
                    list[indexFactorial - 1] = Factorial(numBeforeFactorial).ToString();
                list.RemoveAt(indexFactorial);
                return Count(list);
            }
            if (indexPower != -1)
                return Count(list.DoMathOperation(list.FindMathOperation("^")));
            if ((indexMultiply < indexDivide || indexDivide == -1) && indexMultiply != -1)
                return Count(list.DoMathOperation(list.FindMathOperation("*")));
            if ((indexMultiply > indexDivide || indexMultiply == -1) && indexDivide != -1)
                return Count(list.DoMathOperation(list.FindMathOperation("/")));

            double answer;
            double firstElement = 1;
            double elementNext = 0;
            int k1 = 0;
            int k2 = 1;

            if (!Double.TryParse(list[0], out answer))
            {
                firstElement = list[0] == "-" ? -1 : 1;
                k1 = 1;
                k2 = 0;
            }

            for (int i = 0; i < list.Count - 1; i++)
            {
                if (i % 2 == k1 || i == k1)
                {
                    elementNext = Convert.ToDouble(list[i + 2]);
                    continue;
                }
                if (i % 2 == k2 || i == k2)
                {
                    if (i == 0)
                    {
                        answer += Convert.ToDouble(list[1]) * firstElement;
                        continue;
                    }
                    switch (list[i])
                    {
                        case "+":
                            answer += elementNext;
                            break;
                        case "-":
                            answer -= elementNext;
                            break;
                        default: break;
                    }
                }
            }
            
            return answer;
        }

        public static List<string> GetExpression(string msg)
        {
            return GetMathList(ConvertBrackets(GetMathSymbols(msg)));
        }
        
        //Gets first string, that contains some of the operations and numbers
        private static string GetMathSymbols(string msg)
        {
            string answer = "";

            int i = 0;
            for (i = 0; i < msg.Length; i++)
            {
                var element = msg.ElementAt(i).ToString();

                if (element == " ")
                    continue;

                if (ElementIsMath(element, true))
                    answer += element;
                else if (answer != "" && CheckMathSymbols(answer))
                    break;
                else answer = "";
            }
            
            if (CheckMathSymbols(answer))
                return answer;
            else return "";
        }

        //Checks if brackets give sense
        private static bool CheckBrackets(string msg)
        {
            List<string> list = GetMathList(msg).Where(x => x == "(" || x == ")").ToList();

            if (list.Count == 0)
                return true;
            if (list.Count % 2 != 0)
                return false;

            int limit = list.Count / 2;
            for (int i = 0; i < limit; i++)
            {
                var bracketLeft1 = list.FindMathOperation("(");
                var bracketRight1 = list.FindMathOperation(")");
                if (bracketLeft1 == -1 || bracketRight1 == -1)
                    return false;
                list.RemoveRange(bracketLeft1, 2);
            }
            return true;
        }

        //Checks if expression gives sense (but not if it is solvable or not)
        private static bool CheckExpression(string expression)
        {
            if (expression == "")
                return false;
            if (!CheckBrackets(expression))
                return false;
            for (int i = 0; i < expression.Length; i++)
            {
                var element = expression.ElementAt(i).ToString();
                if (element == "." || element == ",")
                {
                    if (i == expression.Length - 1)
                        return false;
                    if (!numbers.Contains(expression.ElementAt(i + 1).ToString()))
                        return false;
                }
                if (element == "^" || element == "*" || element == "/")
                {
                    if (i == 0 || i == expression.Length - 1)
                        return false;
                    var elementBefore = expression.ElementAt(i-1).ToString();
                    var elementAfter = expression.ElementAt(i+1).ToString();
                    if (!(numbers.Contains(elementBefore) || elementBefore == "!" || elementBefore == ")"))
                        return false;
                    if (!(numbers.Contains(elementAfter) || elementAfter == "!" || elementAfter == "(" || elementAfter == "," || elementAfter == "."))
                        return false;
                }
                if (element == "!")
                {
                    if (i == 0)
                        return false;
                    var elementBefore = expression.ElementAt(i - 1).ToString();
                    if (!(elementBefore == ")" || numbers.Contains(elementBefore)))
                        return false;
                }
                if (element == "+" || element == "-")
                {
                    if (i == expression.Length - 1)
                        return false;
                    if (i != 0)
                    {
                        var elementBefore = expression.ElementAt(i - 1).ToString();
                        if (!(numbers.Contains(elementBefore) || elementBefore != ")" || elementBefore != "("))
                            return false;
                    }
                    var elementAfter = expression.ElementAt(i + 1).ToString();
                    if (!(numbers.Contains(elementAfter) || elementAfter != "." || elementAfter != "," || elementAfter != "("))
                        return false;
                }
                if (element == "(")
                {
                    if (i == expression.Length - 1)
                        return false;
                    var elementAfter = expression.ElementAt(i + 1).ToString();
                    if (!(numbers.Contains(elementAfter) || elementAfter == "+" || elementAfter == "-" || elementAfter == "." || elementAfter == "," || elementAfter == "("))
                        return false;
                }
                if (element == ")")
                {
                    if (i == 0)
                        return false;
                    if (i != expression.Length - 1)
                    {
                        var elementAfter = expression.ElementAt(i + 1).ToString();
                        if (elementAfter == "." || elementAfter == "," || numbers.Contains(elementAfter))
                            return false;
                    }
                }
            }
            return true;
        }

        //Return List of elements of expression 
        private static List<string> GetMathList(string expression)
        {
            List<string> list = new List<string>();
            string addToList = "";
            for (int i = 0; i < expression.Length; i++)
            {
                var element = expression.ElementAt(i).ToString();

                if (numbers.Contains(element))
                {
                    addToList += element;
                    if (i == expression.Length - 1)
                        list.Add(addToList);
                    continue;
                }
                if (element == "," || element == ".")
                {
                    addToList += ",";
                    continue;
                }                 
                if (addToList != "")
                {
                    list.Add(addToList);
                    addToList = "";
                }                
                if (mathOperations.Contains(element))
                {
                    list.Add(element);
                    continue;
                }          
            }
            return list;
        }

        //Checks if there is at least one mathOperation and one number
        private static bool CheckMathSymbols(string msg)
        {
            if (msg == "")
                return false;

            for (int i = 0; i < msg.Length; i++)
            {
                var element = msg.ElementAt(i).ToString();
                if (numbers.Contains(element))
                    break;
                if (i == msg.Length - 1)
                    return false;
            }
            for (int i = 0; i<msg.Length; i++)
            {
                var element = msg.ElementAt(i).ToString();
                if (mathOperations.Contains(element))
                    break;
                if (i == msg.Length - 1)
                    return false;
            }
            
            return true;
        }

        //converts 2(2+2) into 2*(2+2)
        private static string ConvertBrackets(string msg)
        {
            for (int i = 1; i < msg.Length; i++)
            {
                var element = msg.ElementAt(i).ToString();
                var elementBefore = msg.ElementAt(i - 1).ToString();
                if (element == "(")
                {
                    if (elementBefore == ")" || elementBefore == "!" || numbers.Contains(elementBefore))
                    {
                        var char1 = msg.Take(i);
                        string msg1 = "";
                        for (int j = 0; j < char1.Count(); j++)
                        {
                            msg1 += char1.ElementAt(j).ToString();
                        }

                        var char2 = msg.TakeLast(msg.Length - i);
                        string msg2 = "";
                        for (int j = 0; j < char2.Count(); j++)
                        {
                            msg2 += char2.ElementAt(j).ToString();
                        }

                        msg = msg1 + "*" + msg2;
                    }
                }
            }
            return msg;
        }

        private static List<string> CountInBrackets(List<string> list, int firstBracket, int secondBracket)
        {
            if (list[firstBracket] != "(" && list[secondBracket] != ")")
                throw new Exception("Argument list is not is brackets");

            list[firstBracket] = Count(list.GetRange(firstBracket + 1, secondBracket - firstBracket - 1)).ToString();
            list.RemoveRange(firstBracket + 1, secondBracket - firstBracket);
            return list;
        }

        private static bool ElementIsMath(string element, bool dotAndComa)
        {
            if (dotAndComa)
                return numbers.Contains(element) || mathOperations.Contains(element) || element.Equals(".") || element.Equals(",");
            else return numbers.Contains(element) || mathOperations.Contains(element);
        }    

        public static int Factorial(int number)
        {
            if (number < 0)
                throw new ArgumentException("number have to be natural or 0");

            if (number <= 1)
                return 1;
            else return number * Factorial(number - 1);
        }
    }
}
