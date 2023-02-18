# SignalR Streaming Data Demo

This project contains an example of a C# SignalR hub that reads the top posts from a specified subreddit in the Reddit API and sends the data to a JavaScript client.

## Requirements

- Visual Studio
- .NET 6
- Node.js and npm

## Compatibility

Only tested with:
  - Visual Studio Community 2022
  - Windows 10
  - Node 14.18.1
  - NPM 6.14.15

## Prerequisites

1. Create a Script Application for the Reddit API
    - Navigate to https://www.reddit.com/prefs/apps/
    - At the bottom, click the 'Create An App' button
    - Give it any name, such as `demo`
    - Select `script` for the type
    - For `redirect uri`, enter the URL the project will be running on:
      - Using Visual Studio run/debug: `https://localhost:7025/`

## Installation

1. Clone this repository:

   ```
   git clone https://github.com/AlexReff/StreamingDemo.git
   ```

2. Open the project in Visual Studio

3. Right click on the Project in the Solution Explorer, and select `Manage User Secrets`

4. From the Reddit application, copy your `NAME` and `SECRET` so the file looks like:

   ```
   {
    "Reddit:AppId": "NAME",
    "Reddit:AppSecret": "SECRET"
   }
   ```

5. Build or run the solution in Visual Studio
