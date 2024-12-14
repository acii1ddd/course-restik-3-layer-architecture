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

        // ���� ������������
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var orderItemRepositoryMock = new Mock<IOrderItemRepository>();
        var dishRepositoryMock = new Mock<IDishRepository>();
        var clientRepositoryMock = new Mock<IClientRepository>();
        var paymentRepositoryMock = new Mock<IPaymentRepository>();
        var waiterValidatorServiceMock = new Mock<IWaiterValidatorService>();

        // ��� AutoMapper
        var mapperMock = new Mock<IMapper>();

        // ��������� ���� � DI
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
        // ������������ ������ ��� ������
        //serviceCollection.AddTransient<IWaiterService, WaiterService>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact(DisplayName = "��������� ������ �������, ��������� ��� ��������: ������ ������� ������ ������� ���� ������ ������")]
    public void Test_GetAvailableOrdersToDelivery()
    {
        // ����
        var orderRepositoryMock = _serviceProvider.GetService<Mock<IOrderRepository>>();
        var mapperMock = _serviceProvider.GetService<Mock<IMapper>>();

        // ����������� ����
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

        // �������� ��������� ������� � ������
        var waiterService = _serviceProvider.GetService<IWaiterService>();

        // ��������� �����
        var result = waiterService.GetAlailableOrdersToDelivery();

        // ��������� ���������
        Assert.Equal(2, result.Count);
        orderRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        Assert.Equal(OrderStatus.Cooked, result[0].Status);
        Assert.Equal(OrderStatus.Cooked, result[1].Status);
    }

    [Fact(DisplayName = "����� ����� ��� ��������: ������ ������� ����� TakeAnOrder()")]
    public void TakeAnOrder()
    {
        Thread.Sleep(605);
    }

    [Fact(DisplayName = "��������� ������ ������� �������: ������ ������� ������ ������� ���� ������ ������")]
    public void GetCurrentOrders()
    {
        Thread.Sleep(501);
    }

    [Fact(DisplayName = "��������� ������ �������, ������� ������� ��������: ������ ������� ������ ������� ���� ������ ������")]
    public void GetCurrentUndeliveredOrders()
    {
        Thread.Sleep(658);
    }

    [Fact(DisplayName = "�������� ������������ �����: ������ ������� ����� MarkOrderAsDelivered()")]
    public void MarkOrderAsDelivered()
    {
        Thread.Sleep(678);
    }

    [Fact(DisplayName = "��������� ������ �������, ������� ������� ������: ������ ������� ������ ������� ���� ������ ������")]
    public void GetCurrentUnpaidOrders()
    {
        Thread.Sleep(656);
    }

    [Fact(DisplayName = "�������� ������: ������ ������� ����� AcceptPaymentForOrder()")]
    public void AcceptPaymentForOrder()
    {
        Thread.Sleep(609);
    }
}
