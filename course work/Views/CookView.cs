namespace course_work.Views
{
    public class CookView
    {
        public void ShowMenu()
        {
            Console.WriteLine("\n1. Просмотр всех заказов");
            Console.WriteLine("2. Управление активными заказами (вывод + предложение отметить как приготовленный)");
            Console.WriteLine("3. Просмотр рецепта блюда");
            Console.WriteLine("0. Выход");
            Console.WriteLine("Сделайте выбор:");
        }
    }
}
