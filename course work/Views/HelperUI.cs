using BLL.DTO;
using ConsoleTables;
using DAL.Entities;
using System.Reflection;

namespace course_work.Views
{
    internal static class HelperUI
    {
        // Метод для получения читаемого статуса заказа
        public static string GetOrderStatusDescription(OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.InProcessing:
                    return "В обработке";
                case OrderStatus.IsCooking:
                    return "Готовится";
                case OrderStatus.Cooked:
                    return "Приготовлен";
                case OrderStatus.InDelivery:
                    return "В доставке";
                case OrderStatus.Delivered:
                    return "Доставлен";
                case OrderStatus.Completed:
                    return "Обработан";
                default:
                    return "Неизвестный статус";
            }
        }

        public static string GetPaymentStatusDescription(PaymentStatus status)
        {
            switch (status)
            {
                case PaymentStatus.Paid:
                    return "Оплачен";
                case PaymentStatus.Unpaid:
                    return "Не оплачен";
                default:
                    return "Неизвестный статус";
            }
        }

        public static string GetRoleName(string role)
        {
            switch (role.ToLower())
            {
                case "waiter":
                    return "Официант";
                case "cook":
                    return "Повар";
                default:
                    throw new ArgumentException($"Роль '{role}' не существует.");
            }
        }

        public static int GetSelectedNumberWithMessage(string message)
        {
            Console.WriteLine(message);
            return Validator.GetValidInteger("Введите числовое значение: ");
        }

        internal static string GetYesOrNoAnswer()
        {
            string retryInput = "";
            while (true)
            {
                Console.WriteLine("Хотите попробовать выбрать снова? (да/нет):");
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

        internal static void PrintOrders(List<OrderDTO> orders, string message, string ifEmptyMessage)
        {
            if (orders == null || orders.Count() == 0)
            {
                Console.WriteLine(ifEmptyMessage);
                return;
            }

            Console.WriteLine("\n" + message);
            for (int i = 0; i < orders.Count; i++)
            {
                string orderStatusDescription = HelperUI.GetOrderStatusDescription(orders[i].Status);
                string paymentStatusDescription = HelperUI.GetPaymentStatusDescription(orders[i].PaymentStatus);
                Console.WriteLine($"\nЗаказ {i + 1}. Клиент: {orders[i].Client.Name}, Столик: {orders[i].TableNumber}, Общая стоимость: {orders[i].TotalCost}, Статус: {orderStatusDescription}, Статус оплаты: {paymentStatusDescription} \nДата: {orders[i].Date.ToString("dd.MM.yyyy HH.mm")}");

                var dishesTable = new ConsoleTable("№", "Имя блюда", "Количество", "Цена");

                // по блюдам текущего заказа (по orderItem'ам)
                for (int j = 0; j < orders[i].Items.Count; j++)
                {
                    dishesTable.AddRow(
                        j + 1,
                        orders[i].Items[j].Dish.Name, // получ блюдо для вывода имени
                        orders[i].Items[j].Quantity, // у самого ordersItem'а
                        orders[i].Items[j].CurrDishPrice
                    );
                }

                dishesTable.Write(Format.Alternative);
            }
        }

        internal static void PrintDishes(List<DishDTO> dishes, string message, string ifEmptyMessage)
        {
            if (dishes == null || dishes.Count() == 0)
            {
                Console.WriteLine(message);
                return;
            }

            Console.WriteLine(ifEmptyMessage);
            var table = new ConsoleTable("№", "Название блюда", "Цена").Configure(opt => opt.EnableCount = false);
            for (int i = 0; i < dishes.Count; i++)
            {
                table.AddRow(i + 1, dishes[i].Name, $"{dishes[i].Price:C}"); // currency
            }
            table.Write();
        }
    }
}
