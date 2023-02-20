# SignalR Streaming Data Demo

This project contains a demo of a C# SignalR hub that polls new posts and streams the data to a React client that displays charts on various statistics.

## Requirements

- Visual Studio
- .NET 6
- Node.js and npm

## Compatibility

- Visual Studio Community 2022
- Node 14.18.1
- NPM 6.14.15
- Windows 10

## Prerequisites

1. Create a Web Application for the Reddit API
    - Navigate to https://www.reddit.com/prefs/apps/
    - At the bottom, click the 'Create An App' button
    - Give it any name, such as `demo`
    - Select `web app` for the type
    - For `redirect uri`, enter the URL the project will be running on:
      - Using Visual Studio run/debug: `https://localhost:7025/`
    - Your `Reddit:AppId` is the auto-generated string underneath the text `web app`
    - Your `Reddit:AppSecret` is displayed next to the label `secret`

## Installation

1. Clone this repository:

   ```
   git clone https://github.com/AlexReff/StreamingDemo.git
   ```

2. Open the project in Visual Studio

3. Right click on the Project in the Solution Explorer, and select `Manage User Secrets`

4. From the Reddit application, copy your `ID` and `SECRET` so the file looks like:

   ```
   {
    "Reddit:AppId": "ID",
    "Reddit:AppSecret": "SECRET"
   }
   ```

5. Build or run the solution in Visual Studio

## Used Libraries

- [.NET Core 6](https://github.com/dotnet/core)
- [ASP.NET Core SignalR](https://github.com/dotnet/aspnetcore/tree/main/src/SignalR)
- [React](https://github.com/facebook/react)
- [Redux](https://github.com/reduxjs/redux)
- [React-Redux](https://github.com/reduxjs/react-redux)
- [Victory](https://github.com/FormidableLabs/victory)

## Known Issues

- Incomplete styling/layout
- Unoptimized data processing - will eventually lag if tab left open
- Data retrieval and processing still happening while tab is inactive
- No error messaging shown if app is unable to connect to RedditApi
- Graph animations bugged

## TODO

- Front-End
    - Tests
    - CSS/styling on graph & page/layout, fix animations
    - Pause data client when window is inactive
    - Finish decoupling & abstractions
    - Optimize data processing/storage, create delta statistics to prevent re-analyzing entire bucket
    - Update store SignalR connection logic
    - Add error + message display for invalid/error server states
    - Refactor pages/components into proper abstractions
    - Add visualization features (eg filtering)
    - New visualizations
    - Theming support/CSS refactoring
- Back-End
    - Tests
    - Clean/remove comments & leftover code
    - Finish decoupling & abstractions
    - Reduce size of model being sent to client
    - Server-side throttling/security/rate limiting
    - Expand/configure SignalR implementation, create specific subscribe/unsubscribe endpoints
    - Update RedditApiReader to handle token refreshing & error responses
    - Consistent logging & exception handling
    - New serverless backends (AWS, Azure)
- Config
    - Github configurations

## Usage

## Notes
