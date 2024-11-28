using BLL.DTO;
using BLL.ServiceInterfaces.ValidatorsInterfaces;
using ConsoleTables;

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
            //var table = new ConsoleTable("№", typeof(dishes[i].Name));
            int nameWidth = dishes.Max(dish => dish.Name.Length) + 5; // макс длина названия блюда + 5
            for (int i = 0; i < dishes.Count(); i++)
            {
                Console.WriteLine($"{i + 1}. {dishes[i].Name.PadRight(nameWidth)}{dishes[i].Price}");
            }
        }

        private enum DishIdInputStatus
        {
            FinishedInput = 200,
            InvalidInput = 400
        }

        //private int GetDishId(int maxDishesCount)
        //{
        //    Console.Write("\nВведите номер блюда (или нажмите Enter для завершения): ");
        //    var input = Console.ReadLine();
        //    if (string.IsNullOrEmpty(input))
        //    {
        //        return (int)DishIdInputStatus.Finished; // закончил ввод
        //    }

        //    // text
        //    if (!int.TryParse(input, out int dishId))
        //    {
        //        Console.WriteLine("Некорректный номер блюда. Повторите ввод.");
        //        return (int)DishIdInputStatus.Invalid;
        //    }

        //    if (dishId > maxDishesCount || dishId <= 0)
        //    {
        //        Console.Write("Нет такого блюда в меню.\n");
        //        return (int)DishIdInputStatus.Invalid;
        //    }
        //    return dishId - 1; // индекс массива с 0
        //}

        private int GetDishId()
        {
            Console.Write("\nВведите номер блюда (или нажмите Enter для завершения): ");
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                return (int)DishIdInputStatus.FinishedInput; // закончил ввод
            }

            //text
            if (!int.TryParse(input, out int dishId))
            {
                Console.WriteLine("Некорректный номер блюда. Введите целое числовое значение.");
                return (int)DishIdInputStatus.InvalidInput;
            }
            return dishId;
        }

        public int GetDishQuantity()
        {
            return Validator.GetValidInteger(
                "Введите количество блюда: ",
                "Количество должно быть больше 0. Повторите ввод.\n",
                dishId => dishId > 0
            );
        }


        public Dictionary<DishDTO, int> GetSelectedDishes(IOrderValidatorService validator)
        {
            // словарь для выбранных блюд
            var selectedDishes = new Dictionary<DishDTO, int>();
            do
            {
                // ввел номер блюда
                int dishNumber = GetDishId();
                if (dishNumber == (int)DishIdInputStatus.FinishedInput)
                {
                    break; // клиент закончил ввод
                }
                if (dishNumber == (int)DishIdInputStatus.InvalidInput)
                {
                    continue;
                }

                try
                {
                    // dishDTO если номер блюда валиден, иначе - exception
                    var dish = validator.ValidateDishByNumber(dishNumber);

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
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            } while (true);
            return selectedDishes;
        }

        //public Dictionary<DishDTO, int> GetSelectedDishes(List<DishDTO> availableDishes)
        //{
        //    // словарь для выбранных блюд
        //    var selectedDishes = new Dictionary<DishDTO, int>();
        //    do
        //    {
        //        // ввел номер блюда
        //        int dishNUmber = GetDishId(availableDishes.Count);
        //        if (dishId == (int)DishIdInputStatus.FinishedInput)
        //        {
        //            break; // клиент закончил ввод
        //        }
        //        if (dishId == (int)DishIdInputStatus.InvalidInput)
        //        {
        //            continue;
        //        }

        //        // блюдо для добавления
        //        var dish = availableDishes[dishId];
        //        // если количество
        //        int quantity = GetDishQuantity();

        //        if (selectedDishes.ContainsKey(dish))
        //        {
        //            selectedDishes[dish] += quantity; // увелич кол-во, если блюдо уже выбрано
        //        }
        //        else
        //        {
        //            selectedDishes[dish] = quantity;
        //        }
        //        Console.WriteLine($"Добавлено: {dish.Name}, кол-во - {selectedDishes[dish]}");
        //    } while (true);
        //    return selectedDishes;
        //}

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
