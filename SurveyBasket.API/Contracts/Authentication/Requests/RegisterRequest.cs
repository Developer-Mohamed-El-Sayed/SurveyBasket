﻿namespace SurveyBasket.API.Contracts.Authentication.Requests;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password
);
