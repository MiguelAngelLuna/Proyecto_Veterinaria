using System.ComponentModel.DataAnnotations;

namespace MivetOnline.Models.Usuario
{
    public class PasswordValidacion : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string password)
            {
                if (password.Length < 8)
                    return new ValidationResult("La contraseña debe tener al menos 8 caracteres.");

                if (!password.Any(char.IsUpper))
                    return new ValidationResult("La contraseña debe contener al menos una letra mayúscula.");

                if (!password.Any(char.IsDigit))
                    return new ValidationResult("La contraseña debe contener al menos un número.");

                return ValidationResult.Success;
            }
            return new ValidationResult("Formato de contraseña inválido.");
        }
    }
}