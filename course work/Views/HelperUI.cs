using DAL.Entities;

namespace course_work.Views
{
    internal static class HelperUI
    {
        // Метод для получения читаемого статуса заказа
        public static string GetOrderStatusDescription(OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.InProcessing:
                    return "В обработке";
                case OrderStatus.IsCooking:
                    return "Готовится";
                case OrderStatus.Cooked:
                    return "Приготовлен";
                case OrderStatus.InDelivery:
                    return "В доставке";
                case OrderStatus.Delivered:
                    return "Доставлен";
                case OrderStatus.Completed:
                    return "Обработан";
                default:
                    return "Неизвестный статус";
            }
        }

        public static string GetPaymentStatusDescription(PaymentStatus status)
        {
            switch (status)
            {
                case PaymentStatus.Paid:
                    return "Оплачен";
                case PaymentStatus.Unpaid:
                    return "Не оплачен";
                default:
                    return "Неизвестный статус";
            }
        }

        public static int GetSelectedOrderWithMessage(string message)
        {
            Console.WriteLine(message);
            return Validator.GetValidInteger("Введите числовое значение: ");
        }
    }
}
