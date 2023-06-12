namespace DDDCqrsEs.Application.Exceptions
{
    public class ValidationFailureRespone
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}