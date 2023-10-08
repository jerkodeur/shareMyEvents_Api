﻿using System.ComponentModel.DataAnnotations;

namespace ShareMyEvents.Domain.Dtos.Resquests.UserRequests;
public class UserLostPasswordRequest
{
    [Required(ErrorMessage = "Ce champ ne peut être null")]
    public string Email { get; set; } = string.Empty;
}
