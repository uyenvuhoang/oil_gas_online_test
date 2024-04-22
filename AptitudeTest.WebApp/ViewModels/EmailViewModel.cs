// EmailViewModel.cs
using System.ComponentModel.DataAnnotations;

public class EmailViewModel
{
    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; }
}
