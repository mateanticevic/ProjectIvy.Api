using System.Threading.Tasks;
using Google.Apis.Dialogflow.v2.Data;

namespace ProjectIvy.Business.Handlers.Webhooks
{
    public interface IDialogflowHandler
    {
        Task<GoogleCloudDialogflowV2WebhookResponse> ProcessWebhook(GoogleCloudDialogflowV2WebhookRequest request);
        Task<GoogleCloudDialogflowV2WebhookResponse> SetLatestOdometer(GoogleCloudDialogflowV2WebhookRequest request);
    }
}