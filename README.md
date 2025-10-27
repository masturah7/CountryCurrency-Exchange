🌍 Country Currency Exchange API
A .NET 8 Web API that fetches global country data and live exchange rates, stores them in a MySQL database, and exposes endpoints for querying and image summaries.

🚀 Features
Fetches country data from REST Countries API and Open Exchange API
Calculates estimated GDP per country
Generates and caches a summary image
Supports filtering, sorting, and single-country queries
Swagger UI documentation
Built with C#, ASP.NET Core, Entity Framework Core, and MySQL
🛠️ Setup Instructions
1️⃣ Clone and open
Open the solution in Visual Studio 2022 or later.

2️⃣ Update appsettings.json
Add your MySQL connection string:

{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=CountryCurrencyDb;user=root;password=yourpassword"
  }
}
3️⃣ Run migrations (optional)
dotnet ef database update

4️⃣ Run the API
dotnet run


Then open https://localhost:5001/swagger
swagger

🧩 Endpoints
🔁 Refresh data

Fetch countries and exchange rates:

POST /countries/refresh

🌎 Get all countries (with filters)
GET /countries?region=Europe&currency=EUR&sort=gdp_desc

🗺️ Get single country
GET /countries/{name}

❌ Delete a country
DELETE /countries/{name}

🖼️ Get summary image
GET /countries/image

📊 Check system status
GET /status

🧮 Tech Stack

ASP.NET Core 8

Entity Framework Core (MySQL)

RESTCountries API

Open Exchange Rate API

Swagger (Swashbuckle)

💾 Folder Structure
CountryCurrency_Exchange.API/
 ┣ Controllers/
 ┣ Model/
 ┣ Services/
 ┣ Program.cs
 ┗ appsettings.json
