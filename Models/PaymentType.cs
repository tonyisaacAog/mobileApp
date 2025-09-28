namespace CompanyApi.Models
{
    public enum PaymentType
    {
        Cash,
        Visa,
        CashAndVisa
    }
    public static class PaymentTypeTypeMapper
    {
        public static PaymentType MapApiCodeToEnum(string apiCode)
        {
            switch (apiCode.ToLower())
            {
                case "visa":
                    return PaymentType.Visa;
                case "cash":
                    return PaymentType.Cash;
                case "cashandvisa":
                    return PaymentType.CashAndVisa;
                case "v":
                    return PaymentType.Visa;
                case "c":
                    return PaymentType.Cash;
                case "cv":
                    return PaymentType.CashAndVisa;
                default:
                    return PaymentType.Cash;
            }
        }
    }

}
