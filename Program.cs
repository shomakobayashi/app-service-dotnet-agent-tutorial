using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    bool resetPressed = context.Request.Query.ContainsKey("reset");
    bool buttonPressed = context.Request.Query.ContainsKey("increment");

    int pressCount = 0;
    if (context.Request.Cookies.TryGetValue("pressCount", out var cookieVal))
        int.TryParse(cookieVal, out pressCount);

    if (resetPressed)
        pressCount = 0;

    if (buttonPressed && !resetPressed)
        pressCount++;

    context.Response.Cookies.Append("pressCount", pressCount.ToString(), new CookieOptions { Expires = DateTimeOffset.Now.AddHours(1) });

    string buttonColor = "#22c55e";
    string buttonHover = "#15803d";

    await context.Response.WriteAsync($@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>.NET Button Click Demo</title>
    <style>
        body {{
            background: #f8fafc;
            font-family: 'Segoe UI', Arial, sans-serif;
            text-align: center;
            margin: 0; padding: 0;
        }}
        .container {{
            margin-top: 80px;
            background: #fff;
            border-radius: 18px;
            box-shadow: 0 6px 24px rgba(0,0,0,0.08);
            display: inline-block;
            padding: 40px 36px 36px 36px;
        }}
        .number {{
            font-size: 3.2em;
            color: #2563eb;
            margin-bottom: 18px;
        }}
        .note {{
            margin-top: 12px;
            color: #ad6800;
            font-size: 1em;
        }}
        button {{
            margin-top: 30px;
            background: {buttonColor};
            color: #fff;
            border: none;
            border-radius: 6px;
            font-size: 1.2em;
            padding: 12px 28px;
            cursor: pointer;
            transition: background 0.2s;
        }}
        button:disabled {{
            opacity: 0.5;
            cursor: not-allowed;
        }}
        button:hover:enabled {{
            background: {buttonHover};
        }}
        .reset-btn {{
            margin-top: 16px;
            background: #2563eb;
            color: #fff;
            border: none;
            border-radius: 6px;
            font-size: 1em;
            padding: 8px 22px;
            cursor: pointer;
        }}
        .reset-btn:hover {{
            background: #1e40af;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='number' id='counter'>{pressCount}</div>
        <form method='GET' style='display:inline'>
            <input type='hidden' name='increment' value='1' />
            <button id='incrementBtn' type='submit'>Increment</button>
        </form>
        <form method='GET' style='display:inline'>
            <input type='hidden' name='reset' value='1' />
            <button class='reset-btn' type='submit'>Reset Counter</button>
        </form>
    </div>
</body>
</html>
    ");
});

app.Run();
