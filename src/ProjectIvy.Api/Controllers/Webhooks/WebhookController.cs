using Google.Apis.Dialogflow.v2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Webhooks;
using ProjectIvy.Model.Constants.Database;
using System.Threading.Tasks;

namespace ProjectIvy.Api.Controllers.Webhooks
{
    [Authorize(Roles = UserRole.User)]
    [Route("[controller]")]
    public class WebhookController : BaseController<WebhookController>
    {
        private readonly IDialogflowHandler _dialogflowHandler;

        public WebhookController(ILogger<WebhookController> logger, IDialogflowHandler dialogflowHandler) : base(logger)
        {
            _dialogflowHandler = dialogflowHandler;
        }

        [HttpPost("Dialogflow")]
        public async Task<GoogleCloudDialogflowV2WebhookResponse> PostDialogflow([FromBody] GoogleCloudDialogflowV2WebhookRequest request) => await _dialogflowHandler.ProcessWebhook(request);
    }
}
