﻿using BLL.DTO;
using ConsoleTables;

namespace course_work.Views
{
    public class WaiterView
    {
        internal void ShowMenu()
        {
            Console.WriteLine("\n1. Просмотр заказов, доступных для доставки");
            Console.WriteLine("2. Взять заказ для доставки");
            Console.WriteLine("3. Просмотр текущих заказов");
            Console.WriteLine("4. Отметить доставленный заказ");
            Console.WriteLine("5. Принять оплату");
            Console.WriteLine("0. Выход");
            Console.WriteLine("Сделайте выбор:");
        }

        internal void PrintOrders(List<OrderDTO> orders)
        {
            if (orders == null || orders.Count() == 0)
            {
                Console.WriteLine("Заказов пока еще нету.");
                return;
            }

            Console.WriteLine("\nЗаказы для доставки:");
            for (int i = 0; i < orders.Count; i++)
            {
                string orderStatusDescription = HelperUI.GetOrderStatusDescription(orders[i].Status);
                string paymentStatusDescription = HelperUI.GetPaymentStatusDescription(orders[i].PaymentStatus);
                Console.WriteLine($"\nЗаказ {i + 1}. Клиент: {orders[i].Client.Name}, Столик: {orders[i].TableNumber}, Общая стоимость: {orders[i].TotalCost}, Статус: {orderStatusDescription}, Статус оплаты: {paymentStatusDescription}, Дата: {orders[i].Date.ToShortTimeString()}");

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

        internal string GetYesOrNoAnswer()
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
    }
}