using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var msAppId = "e2a75787-8ac9-4d5f-8f2f-e976e77f7730";
var msPwd = "6bk7Q~JsPVYBps1Vfez_u_u0l5Qkfue54V-D9";

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