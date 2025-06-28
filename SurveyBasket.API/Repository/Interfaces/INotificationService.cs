namespace SurveyBasket.API.Repository.Interfaces;

public interface INotificationService
{
    Task SendNewPollNotification(int? pollId = null);
}
