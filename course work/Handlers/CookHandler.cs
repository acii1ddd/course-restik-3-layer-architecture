using BLL.DTO;
using BLL.ServiceInterfaces.LogicInterfaces;
using course_work.Views;
using DAL.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace course_work.Handlers
{
    public class CookHandler
    {
        private readonly ICookService _cookService;
        private readonly CookView _cookView;

        public CookHandler(IServiceProvider provider)
        {
            _cookService = provider.GetService<ICookService>() ?? throw new ArgumentNullException();
            _cookView = new CookView();
        }

        public void HandleCook(WorkerDTO worker)
        {
            Console.WriteLine($"\nДобро пожаловать, {worker.FullName}!");
            bool isLeave = false;
            while (true)
            {
                _cookView.ShowMenu(); // Menu с функционалом

                // выбор клиента
                int choice = Validator.GetValidInteger("Введите корректное значение:");
                switch (choice)
                {
                    case 1:
                        ShowAvailableOrders();
                        break;
                    case 2:
                        TakeAnOrder(worker);
                        break;
                    case 3:
                        ShowCurrentOrders(worker);
                        break;
                    case 4:
                        MarkOrderAsCooked(worker);
                        break;
                    case 5:
                        ViewDishRecipe();
                        break;
                    case 0:
                        isLeave = true;
                        Console.WriteLine("Выход");
                        break; // switch
                    default:
                        Console.WriteLine("Выберите корректный номер.");
                        break;
                }
                if (isLeave)
                {
                    break; // while
                }
            }
        }

        private bool ShowAvailableOrders()
        {
            var orders = _cookService.GetAlailableOrders();
            _cookView.PrintOrders(orders);
            //HelperUI.PrintOrders(orders, "\nВсе заказы: ", "Заказов пока еще нету.");
            return orders.Count() == 0 ? false : true;
        }

        private void TakeAnOrder(WorkerDTO worker)
        {
            string retryInput = "да";
            do
            {
                // заказов нету
                if (ShowAvailableOrders())
                {
                    int selectedOrder = HelperUI.GetSelectedNumberWithMessage("Введите номер заказа, который вы хотите приготовить: ");
                    try
                    {
                        _cookService.TakeAnOrder(selectedOrder, worker); // Передаем заказ в сервис
                        Console.WriteLine("\nЗаказ успешно взят. Можете приступать к приготовлению.");
                        retryInput = "нет";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\nОшибка совершения заказа. " + ex.Message);
                        retryInput = HelperUI.GetYesOrNoAnswer();
                    }
                }
                else
                {
                    break;
                }
            } while (retryInput == "да");
        }

        private bool ShowCurrentOrders(WorkerDTO worker)
        {
            try
            {
                var currentOrders = _cookService.GetCurrentOrders(worker);
                _cookView.PrintOrders(currentOrders);
                //HelperUI.PrintOrders(currentOrders, "\nВсе заказы: ", "Заказов пока еще нету.");
                return currentOrders.Count() == 0 ? false : true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении текущих заказов + {ex.Message}");
                throw;
            }
        }

        private void MarkOrderAsCooked(WorkerDTO worker)
        {
            string retryInput = "да";
            do
            {
                // заказов нету
                if (ShowCurrentOrders(worker))
                {
                    int selectedOrder = HelperUI.GetSelectedNumberWithMessage("Введите номер заказа, который вы приготовили: ");
                    try
                    {
                        _cookService.MarkOrderAsCooked(selectedOrder);
                        Console.WriteLine("\nЗаказ успешно отмечен как приготовленный.");
                        retryInput = "нет";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\nОшибка совершения заказа. " + ex.Message);
                        retryInput = HelperUI.GetYesOrNoAnswer();
                    }
                }
                else
                {
                    break;
                }
            } while (retryInput == "да");
        }

        private void ShowAvailableDishes()
        {
            var availableDishes = _cookService.GetAvailableDishes().ToList();
            _cookView.PrintDishes(availableDishes);
        }

        private void ViewDishRecipe()
        {
            string retryInput = "да";
            do
            {
                ShowAvailableDishes();
                int selectedDish = HelperUI.GetSelectedNumberWithMessage("Введите номер блюда для просмотра его рецепта: ");
                try
                {
                    var recipe = _cookService.GetDishRecipe(selectedDish);
                    _cookView.PrintRecipes(recipe);
                    retryInput = "нет";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nОшибка просмотра рецепта. " + ex.Message);
                    retryInput = HelperUI.GetYesOrNoAnswer();
                }
            } while (retryInput == "да");
        }
    }
}
