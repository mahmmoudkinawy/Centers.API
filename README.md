# API Documentation

Welcome to the API documentation for our .NET Core application. This API incorporates authentication, authorization, Mediator, CQRS, and Cloudinary integration. Below, you'll find an overview of its key features and usage examples.

---

## Features

- **Authentication & Authorization:** Secure your API endpoints with authentication and manage access with different user roles.
- **Mediator & CQRS:** Implement the Mediator pattern for handling requests and following CQRS architecture.
- **Cloudinary Integration:** Easily handle media uploads and storage using Cloudinary.

---

## Code Example

Below is an example code snippet that illustrates the structure and usage of a single file process in the API.

```csharp
namespace Centers.API.Processes.Account
{
    public class ResetPasswordProcess
    {
        public class Request : IRequest<Response>
        {
            public string Email { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
            public bool EmailSent { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                // do some authentication ...
                // retrieve from db ...
                // generate reset link ...
                // send email to user ...

                return Task.FromResult(new Response
                {
                    Success = true,
                    EmailSent = true,
                });
            }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Email).NotEmpty();
            }
        }
    }
}
```
## Endpoints
Reset Password
Endpoint: POST /reset-password

Reset a user's password and send a reset link to their email.

Request:
```csharp
{
  "Email": "bob@test.com"
}
```

Request:
```csharp
{
  "Success": true,
  "EmailSent": true
}
```

## User Roles and Permissions

- **Super Admin:** Full CRUD privileges for all resources.
- **Users:** CRUD operations for specific resources.
- **Exams:** Create, manage exams, and upload CSV files.
- **Types of Exams:** Create different exam types.
- **Centers:** Book exams.
- **Teacher:** Create questions.
- **Reviewer:** Review questions.
- **Students:** Solve questions.
- **Center Admin:** Block students.

## User Registration

New users receive an OTP via Twilio during the registration process.

## Documentation

API endpoints are documented using Swagger. To explore and test the API, access the Swagger UI: <a href="http://centers-startup.somee.com/swagger/index.html">Swagger<a/>.
