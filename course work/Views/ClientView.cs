using BLL.DTO;
using DAL.Entities;

namespace course_work.Views
{
    public class ClientView
    {
        public void ShowMenu()
        {
            Console.WriteLine("\n1. Просмотр меню блюд");
            Console.WriteLine("2. Совершение заказа");
            Console.WriteLine("3. Просмотр статуса заказа");
            Console.WriteLine("0. Выход");
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

        private enum DishIdInputStatus
        {
            Finished = -2,
            Invalid = -1
        }

        private int GetDishId(int maxDishesCount)
        {
            Console.Write("\nВведите номер блюда (или нажмите Enter для завершения): ");
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                return (int)DishIdInputStatus.Finished; // закончил ввод
            }
            
            // text
            if (!int.TryParse(input, out int dishId))
            {
                Console.WriteLine("Некорректный номер блюда. Повторите ввод.");
                return (int)DishIdInputStatus.Invalid;
            }

            if (dishId > maxDishesCount || dishId <= 0)
            {
                Console.Write("Нет такого блюда в меню.\n");
                return (int)DishIdInputStatus.Invalid;
            }
            return dishId - 1; // индекс массива с 0
        }

        public int GetDishQuantity()
        {
            return Validator.GetValidInteger(
                "Введите количество блюда: ",
                "Количество должно быть больше 0. Повторите ввод.\n",
                dishId => dishId > 0
            );
        }

        public Dictionary<DishDTO, int> GetSelectedDishes(List<DishDTO> availableDishes)
        {
            // словарь для выбранных блюд
            var selectedDishes = new Dictionary<DishDTO, int>();
            do
            {
                int dishId = GetDishId(availableDishes.Count);
                if (dishId == (int)DishIdInputStatus.Finished)
                {
                    break; // клиент закончил ввод
                }
                if (dishId == (int)DishIdInputStatus.Invalid)
                {
                    continue;
                }

                // блюдо для добавления
                var dish = availableDishes.ToList()[dishId];
                // если количество
                int quantity = GetDishQuantity();

                if (selectedDishes.ContainsKey(dish))
                {
                    selectedDishes[dish] += quantity; // увелич кол-во, если блюдо уже выбрано
                }
                else
                {
                    selectedDishes[dish] = quantity;
                }
                Console.WriteLine($"Добавлено: {dish.Name}, кол-во - {selectedDishes[dish]}");
            } while (true);
            return selectedDishes;
        }

        public string GetYesOrNoAnswer()
        {
            string retryInput = "";
            Console.WriteLine("Вы не выбрали ни одного блюда. Заказ не был создан.");

            while (true)
            {
                Console.WriteLine("Хотите попробовать сделать заказ снова? (да/нет):");
                retryInput = Console.ReadLine()?.Trim().ToLower();

                if (retryInput == "да" || retryInput == "нет")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("\nВведите корректный ответ: \"да\" или \"нет\".");
                }
            }
            return retryInput;
        }
    }
}
