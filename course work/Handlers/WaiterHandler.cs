using BLL.DTO;
using Microsoft.Extensions.DependencyInjection;
using BLL.ServiceInterfaces.LogicInterfaces;
using course_work.Views;

namespace course_work.Handlers
{
    public class WaiterHandler
    {
        private readonly IWaiterService _waiterService;
        private readonly WaiterView _waiterView;


        public WaiterHandler(IServiceProvider provider)
        {
            _waiterService = provider.GetService<IWaiterService>() ?? throw new ArgumentNullException();
            _waiterView = new WaiterView();
        }

        public void HandleWaiter(WorkerDTO worker)
        {
            Console.WriteLine($"\nДобро пожаловать, {worker.FullName}!");
            bool isLeave = false;
            while (true)
            {
                _waiterView.ShowMenu(); // Menu с функционалом

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
                        //ShowCurrentOrders(worker);
                        break;
                    case 4:
                        //MarkOrderAsCooked(worker);
                        break;
                    case 5:
                        //ViewDishRecipe();
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

        private void TakeAnOrder(WorkerDTO worker)
        {
            string retryInput = "да";
            do
            {
                // заказов нету
                if (ShowAvailableOrders())
                {
                    int selectedOrder = HelperUI.GetSelectedOrderWithMessage("Введите номер заказа, который вы берете: ");
                    try
                    {
                        _waiterService.TakeAnOrder(selectedOrder, worker); // Передаем заказ в сервис
                        Console.WriteLine("\nЗаказ успешно взят. Можете отнести заказ клиенту.");
                        retryInput = "нет";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\nОшибка совершения заказа. " + ex.Message);
                        retryInput = _waiterView.GetYesOrNoAnswer();
                    }
                }
                else
                {
                    break;
                }
            } while (retryInput == "да");
        }

        private bool ShowAvailableOrders()
        {
            var orders = _waiterService.GetAlailableOrders();
            _waiterView.PrintOrders(orders);
            return orders.Count() == 0 ? false : true;
        }
    }
}