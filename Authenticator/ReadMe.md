# Authenticator OAuth Library

**Authenticator OAuth Library** is a .NET class library that simplifies the integration of OAuth-based authentication for multiple providers. Currently, it supports **Google** and **GitHub** OAuth authentication, with plans to extend support to other OAuth providers like Facebook, Twitter, etc. This library is designed to be a one-stop solution for adding authentication to your .NET applications.

## Features

- **Google Authentication**: Easily integrates Google OAuth 2.0 login.
- **GitHub Authentication**: Seamless integration with GitHub's OAuth 2.0.
- **Scalable**: Designed to add more OAuth providers with minimal effort.
- **Customizable**: Flexible configurations to customize authentication flows and error handling.
- **Security**: Uses OAuth 2.0 protocols to ensure secure authentication.

## Installation

You can add the library to your project by including it as a reference or by using NuGet (when available).

1. Clone this repository to your local machine:

    ```bash
    git clone https://github.com/abdulmoiz99/Authenticator.git
    ```

2. Add the Class Library to your .NET project:

    - Open your project in Visual Studio.
    - Right-click on **Solution Explorer** and choose **Add > Existing Project**.
    - Navigate to the folder where the library is located and select the `.csproj` file.

## Configuration

### Authentication Settings

To configure authentication, add the following settings to your `appsettings.json` file:

```json
        {
        "AuthenticationSettings": {
            "Google": {
                "ClientId": "your-google-client-id",
                "ClientSecret": "your-google-client-secret"
                },
            "GitHub": {
                "ClientId": "your-github-client-id",
                "ClientSecret": "your-github-client-secret"
                }
            }
        }
```

### Google Authentication Setup

To enable Google OAuth, follow these steps:

1. **Create Google OAuth Credentials**:
   - Go to the [Google Developer Console](https://console.developers.google.com/).
   - Create a new project.
   - Enable the **Google+ API** and create OAuth credentials.
   - Add your client ID and client secret to your `.NET` application configuration.

2. **Configure in Your App**:

    In the S`Startup.cs` or `Program.cs`, register the Google authentication without the need for clientId and clientSecret parameters:

    ```csharp
    builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("AuthenticationSettings"));
    builder.Services.AddGoogleAndGithubAuthentication();
    ```

### GitHub Authentication Setup

To enable GitHub OAuth:

1. **Create GitHub OAuth Credentials**:
   - Go to [GitHub Developer Settings](https://github.com/settings/developers).
   - Register a new OAuth application and get your **Client ID** and **Client Secret**.
   - Set the callback URL to match your application configuration (e.g., `https://localhost:7148/api/authentication/github-response`).

2. **Configure in Your App**:

   In the `Startup.cs` or `Program.cs`, register GitHub authentication as shown below:

    ```csharp
    builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("AuthenticationSettings"));
    builder.Services.AddGoogleAndGithubAuthentication();
    ```

## Usage

### Google Login Example

To initiate Google login, call the `GoogleLogin` method in your authentication service:

```csharp
public IActionResult LoginWithGoogle()
{
    return _authenticationService.GoogleLogin("/api/authentication/google-response");
}
