﻿using System.ComponentModel.DataAnnotations;

namespace CCFinal.Dtos;

public class RegisterModel {
    [Required(ErrorMessage = "User Name is required")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}