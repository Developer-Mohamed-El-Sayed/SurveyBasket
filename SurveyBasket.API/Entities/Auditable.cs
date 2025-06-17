﻿namespace SurveyBasket.API.Entities;

public class Auditable
{
    public string CreatedById { get; set; } = string.Empty;
    public string? UpdatedById { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
    public ApplicationUser CreatedBy { get; set; } = default!;
    public ApplicationUser? UpdatedBy { get; set; }
}
