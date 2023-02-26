# SignalR Streaming Data Demo

This work-in-progress project contains a demo of a C# .NET Core application that polls new posts and streams the data via SignalR to a React client, which displays charts on various statistics via Recharts.

## Requirements

- [Visual Studio](https://visualstudio.microsoft.com/)
- [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Node.js and npm](https://nodejs.org/)

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
- [Recharts](https://github.com/recharts/recharts)

## Known Issues

- Incomplete styling/layout
- Some browser extensions, such as `uMatrix`, will prevent the web worker from starting when running the project locally

## TODO

- Front-End
    - Tests
    - CSS/styling on graph & page/layout, fix animations
    - Finish decoupling & abstractions
    - Add error + message display for invalid/error server states
    - Refactor pages/components into proper abstractions + layout
    - Add visualization features (eg filtering)
    - New visualizations/statistics
    - Theming support/CSS refactoring
    - Add ability to view previous stat states (redux undo/redo)
- Back-End
    - Tests
    - Clean/remove comments & leftover code
    - Finish decoupling & abstractions
    - New serverless backends (AWS, Azure)
- Config
    - Github configurations & production-ready branch management

## Usage

## Notes
