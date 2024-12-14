using BLL.DTO;
using ConsoleTables;

namespace course_work.Views
{
    internal class AdminView
    {
        internal void ShowMenu()
        {
            Console.WriteLine("\nМеню администратора:");
            Console.WriteLine("1. Управление сотрудниками (просмотр, добавление, удаление)");
            Console.WriteLine("2. Просмотр статистики по заказам (все заказы за период, самые популярные блюда)");
            Console.WriteLine("0. Выход");
            Console.WriteLine("Сделайте выбор:");
        }

        internal void ManageWorkersMenu()
        {
            Console.WriteLine("\nУправление сотрудниками:");
            Console.WriteLine("1. Просмотр всех сотрудников");
            Console.WriteLine("2. Добавить сотрудника");
            Console.WriteLine("3. Удалить сотрудника");
            Console.WriteLine("0. Назад");
        }

        internal void ManageStatisticsMenu()
        {
            Console.WriteLine("\nПросмотр статистики:");
            Console.WriteLine("1. Просмотр заказов за период");
            Console.WriteLine("2. Просмотр самых популярных блюд");
            Console.WriteLine("0. Назад");
        }

        internal void PrintWorkers(List<WorkerDTO> workers, string message)
        {
            if (workers == null || workers.Count() == 0)
            {
                Console.WriteLine("Список работников пуст.");
                return;
            }

            Console.WriteLine("\n" + message);
            var workersTable = new ConsoleTable("№", "Должность", "Номер телефона", "Дата найма", "Имя работника");
            for (int i = 0; i < workers.Count; i++)
            {
                workersTable.AddRow(i + 1, HelperUI.GetRoleName(workers[i].Role.Name), workers[i].PhoneNumber, workers[i].HireDate.ToString("dd.MM.yyyy"), workers[i].FullName);
            }
            workersTable.Write(Format.Alternative);
        }

        // возаращает либо пустую строку, либо строку, подходящую по формату
        internal DateTime GetValidDate(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine()?.Trim();

                // если строка пустая, возвращаем текущую дату
                if (string.IsNullOrEmpty(input))
                {
                    return DateTime.Now.Date;
                }

                // проверка на корректный формат даты
                if (DateTime.TryParse(input, out DateTime hireDate))
                {
                    return hireDate.Date;
                }

                Console.WriteLine("Неверный формат даты. Попробуйте снова.");
            }
        }

        internal void PrintOrders(List<OrderDTO> orders, string message, string ifEmptyMessage)
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

        internal void PrintDishes(List<DishDTO> dishes, string message, string ifEmptyMessage)
        {
            if (dishes == null || dishes.Count() == 0)
            {
                Console.WriteLine(ifEmptyMessage);
                return;
            }

            Console.WriteLine(message);
            var table = new ConsoleTable("№", "Название блюда", "Цена").Configure(opt => opt.EnableCount = false);
            for (int i = 0; i < dishes.Count; i++)
            {
                table.AddRow(i + 1, dishes[i].Name, $"{dishes[i].Price:C}"); // currency
            }
            table.Write();
        }
    }
}
