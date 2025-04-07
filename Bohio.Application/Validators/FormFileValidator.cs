using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Bohio.Application.Validators
{
  public class FormFileValidator : AbstractValidator<FormFile>
  {
    public FormFileValidator()
    {

      RuleFor(formFile => formFile.Length)
          .GreaterThan(0);

      RuleFor(formFile => formFile.ContentType)
          .Must(contentType =>
              contentType == "image/jpeg" ||
              contentType == "image/webp" ||
              contentType == "image/png" ||
              contentType == "video/webm" ||
              contentType == "video/ogg" ||
              contentType == "video/mp4"
          )
          .WithMessage("Invalid file type.");

    }

  }
}
