using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace DDDCqrsEs.Application.Exceptions
{

    public class ValidationException : Exception
    {

        public List<ValidationFailureRespone> Failures { get; set; }
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Failures = new List<ValidationFailureRespone>();
        }

        public ValidationException(List<ValidationFailure> failures)
            : this()
        {
            Failures = new List<ValidationFailureRespone>();
            foreach(var failure in failures)
            {
                Failures.Add(new ValidationFailureRespone()
                {
                    PropertyName = failure.PropertyName,
                    ErrorMessage = failure.ErrorMessage,
                });
            }
        }

        public ValidationException(List<ValidationFailureRespone> failures)
          : this()
        {
            Failures = failures;
        }
    }

  
}
