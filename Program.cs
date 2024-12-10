namespace DZ_3;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics.Metrics;

//{
internal class Program
{



    static void Main(string[] args)
    {
        do
        {
            PrintWelcomeMessage();

            IDictionary<string, int> dictValues = new Dictionary<string, int>();

            try

            {
                dictValues = FuncEnterValues();

                CountDiscriminant(dictValues);
            }

            catch (MyTrueException x)
            {
                FormatData(x.Message, Severity.Error, x.myData); //dictValues

            }
            Console.WriteLine("Нажмите любую клавишу, чтобы повторить");
            Console.WriteLine();
        }

        while (Console.ReadKey().Key != ConsoleKey.Escape);

    }

    static IDictionary<string, int> FuncEnterValues()

    {

        IDictionary<string, string> dictValues = new Dictionary<string, string>();//использую для диалога с пользователем
        IDictionary<string, int> dictValuesForReturn = new Dictionary<string, int>();//буду возвращать в функцию расчета дискриминанта, если ошибок ввода не найдено
        //здесь вводим значения, записываем в словарь dictValues


        List<string> argumList = new List<string>();
        argumList.Add("a:");
        argumList.Add("b:");
        argumList.Add("c:");

        int i = 0;
        while (i < 3)
        {
            string userCommand = "";
            while (true)//здесь цикл для проверки на переполнение
            {
                Console.WriteLine($"Введите значение {argumList[i]}");
                userCommand = Console.ReadLine();
                if (String.IsNullOrEmpty(userCommand))
                {
                    userCommand = "";
                }



                try
                {

                    unchecked //пытаюсь проверить на переполнение, но не хочу, чтобы останавливалось по ошибке ввода не целого, т.к. это я ниже проверяю сразу для 3х вводимых значений
                    {
                        int int_val = 0;
                        int_val = Int32.Parse(userCommand);

                    }

                    break;
                }
                catch (OverflowException)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Переполнение! Можно вводить значения в диапазоне от {int.MinValue} до {int.MaxValue}");
                    Console.ResetColor();
                    Console.WriteLine();
                }
                catch (Exception)
                {
                    break;
                }
            }

            dictValues.Add(argumList[i], userCommand);

            i++;
        }


        IDictionary<string, bool> errorValues = new Dictionary<string, bool>();

        

        bool noError = true;

        string err_message = "Неверные значения параметров:";

        foreach (var str in dictValues)

        {

            //str.Value - проверяем на целое и записываем в errVal

            int int_val = 0;
            bool bool_val = Int32.TryParse(str.Value, out int_val);




            errorValues.Add(str.Key, bool_val);

            noError = noError & bool_val;//если у кого-то из трех значений нераспарсилось, тогда noError будет ложь

            if (!bool_val)
            {
                char lastChar = err_message[err_message.Length - 1];
                if (lastChar == ':')
                {
                    err_message += " " + str.Key.Replace(":", "");
                }
                else
                {
                    err_message += ", " + str.Key.Replace(":", "");
                }
            }
            else
            {
                dictValuesForReturn.Add(str.Key, int_val);
            }

        }

        if (!noError)

        {
            throw new MyTrueException(err_message, dictValues);
        }

        
        return dictValuesForReturn;

    }

    static void PrintWelcomeMessage()
    {
        string welcomeMessage = "Добрый день! Давайте решим квадратное уравнение\n" +
                                "a * x^2 + b * x + c = 0\n";
        Console.WriteLine(welcomeMessage);
        Console.ResetColor();
    }

    static void FormatData(string message, Severity severity, IDictionary<string, string> data=null) //как задать необязательный параметр в виде пустого словаря?
    {
        string line1 = "";
        for (int i = 0; i < 50; i++)
        {
            line1 += "-";
        }

        if (severity == Severity.Warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        else if (severity == Severity.Error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        Console.WriteLine(line1);
        Console.WriteLine(message);
        Console.WriteLine(line1);
        Console.WriteLine();

        if (data != null)
        {
            foreach (var str in data)

            {

                Console.WriteLine(str.Key + " = " + str.Value);

            }
        }
        Console.ResetColor();
    }

    static void CountDiscriminant(IDictionary<string, int> data)
    {
        // D = b2 − 4ac
        int a = data["a:"];
        int b = data["b:"];
        int c = data["c:"];

        if (a != 0)
        {


            float discriminant = b * b - 4 * a * c;
            float sqrD = (float)Math.Sqrt(discriminant);
            float x1;
            float x2;

            try
            {
                if (discriminant < 0)
                {
                    throw new Exception("Корней нет. Дискриминант<0");
                }
                else if (discriminant == 0)
                {
                    x1 = (-b + sqrD) / (2 * a);
                    Console.WriteLine("Найден 1 корень уравнения:");
                    Console.WriteLine($"x = {x1}");
                }
                else
                {

                    x1 = (-b + sqrD) / (2 * a);
                    x2 = (-b - sqrD) / (2 * a);

                    Console.WriteLine("Найдено 2 корня уравнения:");
                    Console.WriteLine($"x1 = {x1}, x2 = {x2}");
                }

            }
            catch (Exception ex) 
            {
                FormatData(ex.Message, Severity.Warning);
            }


        }
        else
        {
            Console.WriteLine("Ошибка! 'a' не может быть равен нулю");
        }
    }

    enum Severity
    {
        Warning,
        Error
    }

    class MyTrueException : Exception

    {
        public IDictionary<string, string> myData; //здесь я не понял как мне в параметр Data от класса родителя Exception присвоить свой dictionary. Поэтому создал свое свойство класса myData
        public MyTrueException(string message, IDictionary<string, string> data)

            : base(message)
        {
            this.myData = data;
        }

    }
}
