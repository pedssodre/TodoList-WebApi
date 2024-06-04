# To do List WebApi
To do List is an API made for tracking daily activities, you can create a task, edit it, mark as done and delete it. This API also provides the possibility to create pagination for frontend.


## How does it work?
We have an in memory database, that it populated on project init. If you want to see the documentation of the API you can check the payload in Swagger.

We also have a [frontend application](https://github.com/pedssodre/TodoList-Front/tree/main) that you can run to test it with the UI.

## How to start?
### Prerequisites 
- .Net 8.0;
- Docker (optional);

### How to run
- Clone the repository;
- Make sure that the TodoList-WebApi is set as Startup Project 
- Start the application on Development settings;

#### Running with docker
If you have docker set up on your machine you can also run this application with it's docker file.
Just start the application with ``container (Docker)``
