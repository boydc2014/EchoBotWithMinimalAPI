using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var msAppId = "<your microsoft app id>";
var msPwd = "<your microsoft password>";

app.MapPost("/api/messages", async (Activity activity, HttpContext context) => {
    try 
    {
        var authHeader = context.Request.Headers["Authorization"].SingleOrDefault();
        await JwtTokenValidation.AuthenticateRequest(activity, authHeader, new SimpleCredentialProvider(msAppId, msPwd), null);
    }
    catch (UnauthorizedAccessException) 
    {
        return Results.Unauthorized();
    }

    if (activity.Type == ActivityTypes.Message)
    {
        var reply = activity.CreateReply($"You said: {activity.Text}");
        var connector = new ConnectorClient(new Uri(activity.ServiceUrl, UriKind.Absolute), 
                                            new MicrosoftAppCredentials(msAppId, msPwd));
        await connector.Conversations.ReplyToActivityAsync(reply);
    }

    return Results.Ok();
});

app.Run();