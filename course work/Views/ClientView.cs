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
            Console.WriteLine("3. Просмотр текущих заказов");
            Console.WriteLine("0. Выход");
            Console.WriteLine("Сделайте выбор:");
        }
        
        public void PrintDishes(List<DishDTO> dishes)
        {
            if (dishes == null || dishes.Count() == 0)
            {
                Console.WriteLine("Список блюд пуст");
                return;
            }
            
            Console.WriteLine("\nМеню блюд:");
            var table = new ConsoleTable("№", "Название блюда", "Цена").Configure(opt => opt.EnableCount = false);
            for (int i = 0; i < dishes.Count; i++)
            {
                table.AddRow(i + 1, dishes[i].Name, $"{dishes[i].Price:C}"); // currency
            }
            table.Write();
        }

        //public void PrintOrders(List<OrderDTO> currOrders)
        //{
        //    if (currOrders == null || !currOrders.Any())
        //    {
        //        Console.WriteLine("У вас нет активных заказов.");
        //        return;
        //    }

        //    for (int i = 0; i < currOrders.Count; i++)
        //    {
        //        string statusDescription = HelperUI.GetOrderStatusDescription(currOrders[i].Status);
        //        string paymentStatusDescription = HelperUI.GetPaymentStatusDescription(currOrders[i].PaymentStatus);
        //        Console.WriteLine($"\nЗаказ {i + 1}. (Дата: {currOrders[i].Date}, Столик: {currOrders[i].TableNumber}, Общая стоимость: {currOrders[i].TotalCost}, Статус: {statusDescription}, Статус оплаты: {paymentStatusDescription})");

        //        var dishesTable = new ConsoleTable("№", "Имя блюда", "Цена за единицу", "Количество", "Итоговая стоимость");

        //        // по блюдам текущего заказа (по orderItem'ам)
        //        for (int j = 0; j < currOrders[i].Items.Count; j++)
        //        {
        //            dishesTable.AddRow(
        //                j + 1,
        //                currOrders[i].Items[j].Dish.Name, // получ блюдо для вывода имени
        //                currOrders[i].Items[j].CurrDishPrice,
        //                currOrders[i].Items[j].Quantity, // у самого ordersItem'а
        //                currOrders[i].Items[j].TotalDishPrice
        //            );
        //        }

        //        dishesTable.Write(Format.Alternative);
        //    }
        //}

        private enum DishIdInputStatus
        {
            FinishedInput = 200,
            InvalidInput = 400
        }

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
                Console.WriteLine("Некорректный номер блюда. Введите целое числовое значение.\n");
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

        public Dictionary<DishDTO, int> GetSelectedDishes(IClientValidatorService validator)
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
                    Console.WriteLine(ex.Message + "\n");
                }
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
