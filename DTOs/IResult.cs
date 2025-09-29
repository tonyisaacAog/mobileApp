namespace CompanyApi.DTOs
{
    public interface IResult<T>
    {
        List<string> Messages { get; set; }
        bool Succeeded { get; set; }
        T? Data { get; set; }
        List<ValidationResults> ValidationErrors { get; set; }
        int Code { get; set; }
    }
}
