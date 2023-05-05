using System.ComponentModel.DataAnnotations;

namespace CCFinal.Dtos;

public class ChangePasswordModel {
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }
}
