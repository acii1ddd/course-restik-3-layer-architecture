namespace course_work
{
    public static class Validator
    {
        public static int GetValidInteger(string errorMessage)
        {
            int result = 0;
            do
            {
                var input = Console.ReadLine();
                if (int.TryParse(input, out result))
                {
                    break;
                }
                else
                {
                    Console.Write(errorMessage + " ");
                }
            } while (true);
            return result;
        }

        public static int CheckValidInteger(string retryMessage, string errorMessage)
        {
            int result = 0;
            do
            {
                Console.Write(retryMessage);
                string input = Console.ReadLine();

                if (int.TryParse(input, out result))
                {
                    break; // Если ввод корректен, выходим из цикла
                }
                else
                {
                    Console.WriteLine(errorMessage);
                }
            } while (true);

            return result;
        }

        // ввод int числа, подходящего по условию предиката
        public static int GetValidInteger(string retryMessage, string errorRangeMessage, Func<int, bool> predicate)
        {
            while (true)
            {
                Console.Write(retryMessage);
                string input = Console.ReadLine();
                if (int.TryParse(input, out int result))
                {
                    if (predicate(result))
                    {
                        return result;
                    }
                    else
                    {
                        Console.WriteLine(errorRangeMessage);
                    }
                }
                else
                {
                    Console.WriteLine("Введите число корректно.\n");
                }
            }
        }
    }
}
