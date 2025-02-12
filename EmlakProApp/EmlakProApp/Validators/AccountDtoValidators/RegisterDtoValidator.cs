using EmlakProApp.DTOs.AccountDTOs;
using FluentValidation;

namespace EmlakProApp.Validators.AccountDtoValidators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            //RuleFor(r => r.Name)
            //    .NotEmpty().WithMessage("Boş saxlamaq olmaz.")
            //    .MaximumLength(50).WithMessage("Simvol sayı 50-dən artıq olmamalıdır.");

            //RuleFor(r => r.Surname)
            //    .NotEmpty().WithMessage("Boş saxlamaq olmaz.")
            //    .MaximumLength(50).WithMessage("Simvol sayı 50-dən artıq olmamalıdır.");

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Boş saxlamaq olmaz.")
                .MaximumLength(100).WithMessage("Simvol sayı 100-dən çox olmamalıdır.")
                .EmailAddress().WithMessage("Düzgün email formatı olmalıdır.");

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Boş saxlamaq olmaz.")
                .MinimumLength(8).WithMessage("Simvol sayı 8-dən aşağı olmamalıdır.")
                .MaximumLength(50).WithMessage("Simvol sayı 50-dən artıq olmamalıdır.");

            RuleFor(r => r.RepeatPassword)
                .NotEmpty().WithMessage("Boş saxlamaq olmaz.")
                .MinimumLength(8).WithMessage("Simvol sayı 8-dən aşağı olmamalıdır.")
                .MaximumLength(50).WithMessage("Simvol sayı 50-dən artıq olmamalıdır.")
                .Equal(r => r.Password).WithMessage("Şifrə eyni olmalıdır.");
        }
    }
}
