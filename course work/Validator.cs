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
    }
}
