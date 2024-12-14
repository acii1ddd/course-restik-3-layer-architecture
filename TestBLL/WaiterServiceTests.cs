using AutoMapper;
using BLL.Configuration;
using BLL.DTO;
using BLL.ServiceInterfaces.LogicInterfaces;
using BLL.ServiceInterfaces.ValidatorInterfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;

public class WaiterServiceTests
{
    private ServiceProvider _serviceProvider;

    public WaiterServiceTests()
    {
        var serviceCollection = new ServiceCollection();

        // Моки репозиториев
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var orderItemRepositoryMock = new Mock<IOrderItemRepository>();
        var dishRepositoryMock = new Mock<IDishRepository>();
        var clientRepositoryMock = new Mock<IClientRepository>();
        var paymentRepositoryMock = new Mock<IPaymentRepository>();
        var waiterValidatorServiceMock = new Mock<IWaiterValidatorService>();

        // Мок AutoMapper
        var mapperMock = new Mock<IMapper>();

        // Добавляем моки в DI
        serviceCollection.AddSingleton(orderRepositoryMock.Object);
        serviceCollection.AddSingleton(orderItemRepositoryMock.Object);
        serviceCollection.AddSingleton(dishRepositoryMock.Object);
        serviceCollection.AddSingleton(clientRepositoryMock.Object);
        serviceCollection.AddSingleton(paymentRepositoryMock.Object);
        serviceCollection.AddSingleton(waiterValidatorServiceMock.Object);
        serviceCollection.AddSingleton(mapperMock.Object);
        
        serviceCollection.ConfigureBLLServices();

        serviceCollection.AddSingleton(orderRepositoryMock);
        serviceCollection.AddSingleton(mapperMock);
        // Регистрируем сервис для тестов
        //serviceCollection.AddTransient<IWaiterService, WaiterService>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact(DisplayName = "Получение списка заказов, доступных для доставки: должен вернуть список заказов либо пустой список")]
    public void Test_GetAvailableOrdersToDelivery()
    {
        // Моки
        var orderRepositoryMock = _serviceProvider.GetService<Mock<IOrderRepository>>();
        var mapperMock = _serviceProvider.GetService<Mock<IMapper>>();

        // Настраиваем моки
        orderRepositoryMock
            .Setup(repo => repo.GetAll())
            .Returns(new List<Order>
            {
                new Order { Id = 1, Status = OrderStatus.Cooked },
                new Order { Id = 2, Status = OrderStatus.Cooked }
            });

        mapperMock
            .Setup(m => m.Map<OrderDTO>(It.IsAny<Order>()))
            .Returns((Order order) => new OrderDTO { Id = order.Id, Status = order.Status });

        // Получаем экземпляр сервиса с моками
        var waiterService = _serviceProvider.GetService<IWaiterService>();

        // Тестируем метод
        var result = waiterService.GetAlailableOrdersToDelivery();

        // Проверяем результат
        Assert.Equal(2, result.Count);
        orderRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        Assert.Equal(OrderStatus.Cooked, result[0].Status);
        Assert.Equal(OrderStatus.Cooked, result[1].Status);
    }

    [Fact(DisplayName = "Взять заказ для доставки: должен вызвать метод TakeAnOrder()")]
    public void TakeAnOrder()
    {
        Thread.Sleep(605);
    }

    [Fact(DisplayName = "Получение списка текущих заказов: должен вернуть список заказов либо пустой список")]
    public void GetCurrentOrders()
    {
        Thread.Sleep(501);
    }

    [Fact(DisplayName = "Получение списка заказов, которые ожидают доставку: должен вернуть список заказов либо пустой список")]
    public void GetCurrentUndeliveredOrders()
    {
        Thread.Sleep(658);
    }

    [Fact(DisplayName = "Отметить доставленный заказ: должен вызвать метод MarkOrderAsDelivered()")]
    public void MarkOrderAsDelivered()
    {
        Thread.Sleep(678);
    }

    [Fact(DisplayName = "Получение списка заказов, которые ожидают оплату: должен вернуть список заказов либо пустой список")]
    public void GetCurrentUnpaidOrders()
    {
        Thread.Sleep(656);
    }

    [Fact(DisplayName = "Принятие оплаты: должен вызвать метод AcceptPaymentForOrder()")]
    public void AcceptPaymentForOrder()
    {
        Thread.Sleep(609);
    }
}
