﻿namespace SurveyBasket.API.Abstractions;

public record Error(string Code, string Decription, int? StatusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty, null);
}
