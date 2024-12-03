
namespace course_work.Views
{
    internal class AdminView
    {
        internal void ShowMenu()
        {
            Console.WriteLine("\nМеню администратора:");
            Console.WriteLine("1. Управление сотрудниками (добавление, удаление, редактирование)");
            Console.WriteLine("2. Управление меню (добавление, удаление, редактирование блюд)");
            Console.WriteLine("3. Просмотр статистики по заказам (все заказы за период, самые   популярные блюда)");
            Console.WriteLine("0. Выход");
            Console.WriteLine("Сделайте выбор:");
        }


    }
}
