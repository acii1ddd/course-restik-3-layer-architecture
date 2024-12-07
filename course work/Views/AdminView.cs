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
            Console.WriteLine("2. Управление меню (добавление, удаление, редактирование блюд)");
            Console.WriteLine("3. Просмотр статистики по заказам (все заказы за период, самые популярные блюда)");
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

        internal void ManageDishesMenu()
        {
            Console.WriteLine("\nУправление меню:");
            Console.WriteLine("1. Просмотр меню блюд");
            Console.WriteLine("2. Добавить блюдо");
            Console.WriteLine("3. Удалить блюдо");
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
        internal DateTime GetValidHireDate()
        {
            while (true)
            {
                Console.Write("\nВведите дату найма (в формате ДД.ММ.ГГГГ) или нажмите Enter для установки текущей даты: ");
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
    }
}
