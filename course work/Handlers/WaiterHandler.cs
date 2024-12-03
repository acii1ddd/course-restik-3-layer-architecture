using BLL.DTO;
using Microsoft.Extensions.DependencyInjection;
using BLL.ServiceInterfaces.LogicInterfaces;
using course_work.Views;
using DAL.Entities;

namespace course_work.Handlers
{
    internal class WaiterHandler
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
                        ShowAvailableOrdersToDelivery();
                        break;
                    case 2:
                        TakeAnOrder(worker);
                        break;
                    case 3:
                        ShowAllCurrentOrders(worker);
                        break;
                    case 4:
                        MarkOrderAsDelivered(worker);
                        break;
                    case 5:
                        AcceptPayment(worker);
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

        private bool ShowAvailableOrdersToDelivery()
        {
            var orders = _waiterService.GetAlailableOrdersToDelivery();
            _waiterView.PrintOrders(orders, "Заказы, доступные для доставки:");
            return orders.Count() == 0 ? false : true;
        }

        private void TakeAnOrder(WorkerDTO worker)
        {
            string retryInput = "да";
            do
            {
                // заказов нету
                if (ShowAvailableOrdersToDelivery())
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

        // общие заказы (могут быть доставлены или еще не доставлены / не оплачены)
        private bool ShowAllCurrentOrders(WorkerDTO worker)
        {
            try
            {
                var currentOrders = _waiterService.GetCurrentOrders(worker);
                _waiterView.PrintOrders(currentOrders, "Ваши текущие заказы:");
                return currentOrders.Count() == 0 ? false : true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении текущих заказов + {ex.Message}");
                throw;
            }
        }

        // доставленные заказы
        private bool ShowUndeliveredOrders(WorkerDTO worker)
        {
            try
            {
                var currentOrders = _waiterService.GetCurrentUndeliveredOrders(worker);
                _waiterView.PrintOrders(currentOrders, "Заказы для доставки:");
                return currentOrders.Count() == 0 ? false : true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении текущих заказов + {ex.Message}");
                throw;
            }
        }

        private void MarkOrderAsDelivered(WorkerDTO worker)
        {
            string retryInput = "да";
            do
            {
                // если нет заказов
                if (ShowUndeliveredOrders(worker))
                {
                    int selectedOrder = HelperUI.GetSelectedOrderWithMessage("Введите номер заказа, который вы доставили: ");
                    try
                    {
                        _waiterService.MarkOrderAsDelivered(selectedOrder);
                        Console.WriteLine("\nЗаказ успешно отмечен как доставленный.");
                        retryInput = "нет";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\nОшибка при отметке заказа. " + ex.Message);
                        retryInput = _waiterView.GetYesOrNoAnswer();
                    }
                }
                else
                {
                    break;
                }
            } while (retryInput == "да");
        }

        private bool ShowCurrentUnpaidOrders(WorkerDTO worker)
        {
            try
            {
                var unpaidOrders = _waiterService.GetCurrentUnpaidOrders(worker);
                _waiterView.PrintOrders(unpaidOrders, "Не оплаченные заказы:");
                return unpaidOrders.Count() == 0 ? false : true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении текущих заказов + {ex.Message}");
                throw;
            }
        }

        private void AcceptPayment(WorkerDTO worker)
        {
            string retryInput = "да";
            do
            {
                // если нет заказов
                if (ShowCurrentUnpaidOrders(worker))
                {
                    int selectedOrder = HelperUI.GetSelectedOrderWithMessage("Введите номер заказа, чтобы принять оплату: ");
                    PaymentMethod paymentMethod = _waiterView.GetPaymentMethod();
                    
                    try
                    {
                        _waiterService.AcceptPaymentForOrder(selectedOrder, paymentMethod);
                        Console.WriteLine("\nЗаказ успешно оплачен.");
                        retryInput = "нет";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\nОшибка при оплате заказа. " + ex.Message);
                        retryInput = _waiterView.GetYesOrNoAnswer();
                    }
                }
                else
                {
                    break;
                }
            } while (retryInput == "да");
        }
    }
}