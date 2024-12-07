using BLL.DTO;
using ConsoleTables;

namespace course_work.Views
{
    public class CookView
    {
        internal void ShowMenu()
        {
            Console.WriteLine("\n1. Просмотр заказов для приготовления");
            Console.WriteLine("2. Взять заказ для приготовления");
            Console.WriteLine("3. Просмотр текущих заказов");
            Console.WriteLine("4. Отметить приготовленный заказ");
            Console.WriteLine("5. Просмотр рецепта блюда");
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

            Console.WriteLine("\nВсе заказы:");
            for (int i = 0; i < orders.Count; i++)
            {
                Console.WriteLine($"\nЗаказ {i + 1}. (Дата заказа: {orders[i].Date})");

                var dishesTable = new ConsoleTable("№", "Имя блюда", "Количество");

                // по блюдам текущего заказа (по orderItem'ам)
                for (int j = 0; j < orders[i].Items.Count; j++)
                {
                    dishesTable.AddRow(
                        j + 1,
                        orders[i].Items[j].Dish.Name, // получ блюдо для вывода имени
                        orders[i].Items[j].Quantity // у самого ordersItem'а
                    );
                }

                dishesTable.Write(Format.Alternative);
            }
        }

        //internal string GetYesOrNoAnswer()
        //{
        //    string retryInput = "";
        //    while (true)
        //    {
        //        Console.WriteLine("Хотите попробовать выбрать снова? (да/нет):");
        //        retryInput = Console.ReadLine()?.Trim().ToLower();

        //        if (retryInput == "да" || retryInput == "нет")
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            Console.WriteLine("\nВведите корректный ответ: \"да\" или \"нет\".");
        //        }
        //    }
        //    return retryInput;
        //}

        internal void PrintDishes(List<DishDTO> dishes)
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

        internal void PrintRecipes(List<RecipeDTO> recipes)
        {
            if (recipes == null || recipes.Count() == 0)
            {
                Console.WriteLine("Список рецептов пуст");
                return;
            }

            Console.WriteLine("\nРецепт блюда:");
            var table = new ConsoleTable("№", "Название ингредиента", "Количество", "Единицы измерения");
            for (int i = 0; i < recipes.Count; i++)
            {
                table.AddRow(i + 1, recipes[i].Ingredient.Name, recipes[i].Quantity, recipes[i].Unit);
            }
            table.Write(Format.Alternative);
        }
    }
}
