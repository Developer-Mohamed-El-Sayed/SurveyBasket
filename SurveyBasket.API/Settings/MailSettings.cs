﻿namespace SurveyBasket.API.Settings;

public class MailSettings
{
    public string Mail { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
}
