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

        public static string GetRoleName(string role)
        {
            switch (role.ToLower())
            {
                case "waiter":
                    return "Официант";
                case "cook":
                    return "Повар";
                default:
                    throw new ArgumentException($"Роль '{role}' не существует.");
            }
        }

        public static int GetSelectedNumberWithMessage(string message)
        {
            Console.WriteLine(message);
            return Validator.GetValidInteger("Введите числовое значение: ");
        }

        internal static string GetYesOrNoAnswer()
        {
            string retryInput = "";
            while (true)
            {
                Console.WriteLine("Хотите попробовать выбрать снова? (да/нет):");
                retryInput = Console.ReadLine()?.Trim().ToLower();

                if (retryInput == "да" || retryInput == "нет")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("\nВведите корректный ответ: \"да\" или \"нет\".");
                }
            }
            return retryInput;
        }
    }
}
