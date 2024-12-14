using BLL.DTO;
using ConsoleTables;
using DAL.Entities;

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

        internal void PrintOrders(List<OrderDTO> orders, string message)
        {
            if (orders == null || orders.Count() == 0)
            {
                Console.WriteLine("Заказов пока еще нету.");
                return;
            }

            Console.WriteLine("\n" + message);
            for (int i = 0; i < orders.Count; i++)
            {
                string orderStatusDescription = HelperUI.GetOrderStatusDescription(orders[i].Status);
                string paymentStatusDescription = HelperUI.GetPaymentStatusDescription(orders[i].PaymentStatus);
                Console.WriteLine($"\nЗаказ {i + 1}. Клиент: {orders[i].Client.Name}, Столик: {orders[i].TableNumber}, Общая стоимость: {orders[i].TotalCost} \nСтатус: {orderStatusDescription}, Статус оплаты: {paymentStatusDescription}, Дата: {orders[i].Date.ToString("dd.MM.yyyy HH.mm")}");

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

        internal PaymentMethod GetPaymentMethod()
        {
            while (true)
            {
                Console.WriteLine("1. Наличные");
                Console.WriteLine("2. Карта");
                Console.Write("Выберите способ оплаты заказа: ");

                int input = Validator.GetValidInteger("Введите числовое значение: ");

                if (input == 1)
                {
                    return PaymentMethod.Cash;
                }
                else if (input == 2)
                {
                    return PaymentMethod.Card;
                }
                else
                {
                    Console.WriteLine("\nВыберите корректный способ: 1 либо 2.");
                }
            }
        }
    }
}
