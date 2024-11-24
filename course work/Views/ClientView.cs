using BLL.DTO;

namespace course_work.Views
{
    public class ClientView
    {
        public void ShowMenu()
        {
            Console.WriteLine("\n1. Просмотр меню блюд");
            Console.WriteLine("2. Совершение заказа");
            Console.WriteLine("3. Просмотр статуса заказа");
            Console.WriteLine("Сделайте выбор:");
        }
        
        public void PrintDishes(List<DishDTO> dishes)
        {
            if (dishes == null)
            {
                Console.WriteLine("Список блюд пуст");
                return;
            }
            
            Console.WriteLine("\nМеню блюд:");
            //Console.WriteLine(new string('-', 25)); // line
            int nameWidth = dishes.Max(dish => dish.Name.Length) + 5; // макс длина названия блюда + 5
            for (int i = 0; i < dishes.Count(); i++)
            {
                Console.WriteLine($"{i + 1}. {dishes[i].Name.PadRight(nameWidth)}{dishes[i].Price}");
            }
        }
    }
}
