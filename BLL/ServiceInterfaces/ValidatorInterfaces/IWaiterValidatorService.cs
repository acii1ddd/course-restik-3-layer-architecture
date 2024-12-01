﻿using BLL.DTO;

namespace BLL.ServiceInterfaces.ValidatorInterfaces
{
    public interface IWaiterValidatorService
    {
        public OrderDTO ValidateOrderByNumber(int orderNumber);

        public OrderDTO ValidateOrderByNumberToMark(int orderNumber);

        public void ValidateWaiter(WorkerDTO worker);

        public void ValidateTakeOrder(int selectedOrder, WorkerDTO cook);

        public OrderDTO ValidateOrderByNumberToAcceptPayment(int orderNumber);
    }
}
