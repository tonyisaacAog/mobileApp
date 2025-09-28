namespace CompanyApi.Models
{
    public enum CustomerType
    {
        P,
        B,
        F
    }
    public static class CustomerTypeMapper
    {
        public static CustomerType ToCustomerType(string customerType)
        {
            switch (customerType.ToLower())
            {
                case "p":
                    return CustomerType.P;
                case "b":
                    return CustomerType.B;
                case "f":
                    return CustomerType.F;
                default:
                    return CustomerType.P;
            }
        }
    }
}
