<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
     <li>
      <a href="#final-thoughts">Final Thoughts</a>
      <ul>
        <li><a href="#deployment">Deployment</a></li>
      </ul>
    </li>
  </ol>
/details>



<!-- ABOUT THE PROJECT -->
## About The Project

This project was created as part of a coding challenge. The assignment was as follows:

Create a distributed system consisting of two microservices.

Microservice 1 (Coffee Factory)

Implement a service that simulates the production process of coffee. Please take the following points into consideration for the implementation:

* the service should produce one unit of coffee every 5 seconds
* once one unit of coffee has been produced, send it to microservice 2 (coffee store) in order to sell it.
* keep in mind, that the coffee store could not be available at any given time. Each produced unit of coffee should still make it over to the store.

Microservice 2 (Coffee Store)

Implement a service that simulates selling coffee. Please take the following points into consideration for the implementation:

* connect a database to your service in order to manage the coffee in stock.
* provide an interface of your choice (e.g. a REST-interface), through which customers could inform themselves over the items in stock or buy coffee. 

<p align="right">(<a href="#top">back to top</a>)</p>

### Built With

* [Asp.Net Core](https://docs.microsoft.com/de-de/aspnet/core/?view=aspnetcore-6.0)
* [MSSQL](https://www.microsoft.com/de-de/sql-server/sql-server-downloads)
* [EF Core](https://github.com/dotnet/efcore)
* [AutoMapper](https://automapper.org/)
* [RabbitMQ](https://www.rabbitmq.com/)
* [Docker](https://www.docker.com/)

Both microservices were built in ASP.NEt Core with .Net 6. 

The database of choice for the Coffe Store is a MSSQL Database. 
That could have been any SQL Database (for example a PostgreSQL). Even a NoSQL DB like MongoDB could have been used. A SQL DB was chosen over it, in order to leave the possibility to store user credentials in the same DB.
Ef Core is used to assist with the 'code-first' approach of scaffolding the DB. This helps decorate object properties for the ORM and provides tools to migrate data and update the database.

Automapper is a library to help with mapping between objects. This project uses Domain objects, as well as DTOs and Automapper simplifies the mapping process.

RabbitMQ was chosen as a way of communication between both microservices. RabbitMQ is a message broker that allows to publish and subscribe to a message queue. By using this, information between both microservices can be exchanged without tightly coupling them together.

Last but not least Docker was used in order to containerize this application and to orchestrate all of its components in a way that makes it easy to spin up and test locally.


<p align="right">(<a href="#top">back to top</a>)</p>



<!-- Getting Started -->
## Getting Started

### Prerequisites

For this guide it is assumed that the reader will follow these steps on a windows machine.

Make sure you have Docker Desktop installed on your machine with the option for Linux Containers enabled.

[Docker] (https://www.docker.com/)

If you have currently Windows Containers enabled, you can swap to the Linux option via the command line:

* cli
  ```sh
  cd "C:\Program Files\Docker\Docker"
  ./DockerCli.exe -SwitchDaemon
  ```
  
_Optional_

If you want to see what is happening in the database independent of what the Coffee-Store-API grants access to, you can install a database tool of your choice, e.g:
 
* [SSMS] (https://docs.microsoft.com/de-de/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)  or
* [DBeaver] (https://dbeaver.io/)


### Installation

_You can clone this repo to your local machine with the following console command (assuming git is installed on your device)_

  * cli
   ```sh
   git clone https://github.com/Ben-Nodapo-CC/CoffeeStore.git

   ```
   
once the repo is on your machine, please navigate to the root directory, where the 'docker-compose.yml' is located. Open up a command line tool and execute the following command:

  * cli
   ```sh
   docker-compose up -d
   ```
   
Once the command has finished building and starting the related docker containers you should find them in Docker Desktop in the section Containers/Apps under "coffeestore".
The containers "coffeefactory" and "coffeestore" will probably restart a couple of times during this process. This is due to the fact, that they depend on the container "rabbitmq" which takes a long time to be fully operational. As long as "rabbitmq" is not fully ready, "coffeestore" and "coffeefactory" will restart until they can resolve all dependencies.

_side note: even though Visual Studio is able to run this project via it's own 'docker-compose' (set as the startup project), it is not recommended in this case. The restart mechanism does not work the same, which means you will run into exceptions on the first run via Visual Studios 'docker-compose', as 'rabbitmq' will not be available in time. 
On the second run it will work, since 'rabbitmq' will already be running at that point. BUT even then Visual Studios 'docker-compose' will not be able to properly restart containers once the entire cluster is already running. This makes testing the message flow in case of the unavailability of the 'coffeestore' too tedious. Hence we should stick to the cli command in this case._

<p align="right">(<a href="#top">back to top</a>)</p>


<!-- USAGE EXAMPLES -->
## Usage

Once the containers are running, the application will start working on its own. Meaning the Coffee Factory will create one coffee every five seconds and send this information via RabbitMQ to the Coffe Store.
In order to follow along, there are two options to choose from.

1) Connect to the MSSQL and look at the data arriving in the 'Coffees'-table of the 'coffeesdb'.  (Host: localhost, Port:1433, Database/Schema: master, User name:sa, Password: Nodapo_CC)
2) Use the Api of the Coffee Store to query and manipulate the data in the database. You can find the documentation via swagger at http://localhost:84/swagger/index.html.
The API can be tested in swagger as well. Or you can use the swagger.json at the top of the page to import into Postman or another tool for testing API requests.

Since RabbitMQ has been set up with durable message queues, the Coffee Store can also receive messages that have been sent, while it was closed.
To help verify that, Coffee.cs has the property 'SerialNumber' which is an ID the Coffee Factory assigns to each created Coffee. This is a helper to check for missing messages. A little caveat, this SerialNumber is only consistent during the application lifetime of the factory. If it resets, the Serialnumber will restart at 1.
To test the missed messages being resent, do the following:

1) Let the factory generate a couple of coffees.
2) Confirm coffees with incremental serialnumbers have been added to the DB.
3) Stop the 'coffeestore' container for at least 10 seconds.
4) Confirm that there are no more coffees being added to the DB
5) Restart the 'coffestore" container
6) Confirm that all missed messages are being added after restarting.

<p align="right">(<a href="#top">back to top</a>)</p>


<!-- Final Thoughts -->
## Final Thoughts

<!-- Deployment -->
### Deployment

Since this application is already containerized and orchestrated locally with docker-compose, the next logical step would be to deploy it in kubernetes. This is the industry standard and can be used to make this application more fault-tolerant, more scalable and use resources optimally among other things.
