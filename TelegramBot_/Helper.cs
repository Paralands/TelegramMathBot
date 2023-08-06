using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot_
{
    public static class Helper
    {
        public static void AddUser<ChatUser>(this List<TelegramBot_.ChatUser> users, TelegramBot_.ChatUser user)
        {
            if (user.IsAdded)
                return;
            if (!users.Select(user => user.ID).Contains(user.ID))
            {
                users.Add(user);
                user.IsAdded = true;
            }
        }

        public static int FindMathOperation(this List<string> list, string mathOperation)
        {
            if (!MathController.mathOperations.Contains(mathOperation))
                throw new Exception("mathOperation argument is not a mathOperation");

            if (mathOperation == "(" || mathOperation == ")")
            {
                List<string> listCopy = new List<string>(list);
                    
                int j = 0;
                while (true)
                {
                    int index = listCopy.FindIndex(x => x == "(");
                    if (index == -1)
                    {
                        break;
                    }            
                    //finding "(" bracket for which the closest breacket is ")"
                    for (int i = index + 1; i < listCopy.Count; i++)
                    {
                        if (listCopy[i] == ")")
                        {
                            if (mathOperation == "(")
                                return index+j;
                            else return i+j;
                        }
                        if (listCopy[i] == "(")
                        {
                            listCopy.RemoveAt(index);
                            j++;
                            break;
                        }
                    }            
                }
                return -1; //If there is no bracket
            }

            return list.FindIndex(x => x == mathOperation);
        }

        public static List<string> DoMathOperation(this List<string> list, int i) //i = mathOperationIndex
        {
            if (!(0 < i && i < list.Count - 1))
            {
                throw new ArgumentOutOfRangeException("Math symbol is not in the middle");
            }
                
            double answer;

            switch (list[i])
            {
                case "*":
                    answer = Convert.ToDouble(list[i - 1]) * Convert.ToDouble(list[i + 1]);
                    break;
                case "/":
                    answer = Convert.ToDouble(list[i - 1]) / Convert.ToDouble(list[i + 1]);
                    break;
                case "^":
                    answer = Math.Pow(Convert.ToDouble(list[i - 1]), Convert.ToDouble(list[i + 1]));
                    break;
                default: throw new Exception("Symbol is not '*', '/' or '^'");
            }

            list[i - 1] = answer.ToString();
            list.RemoveRange(i,2);
            
            return list;
        }
    }
}
