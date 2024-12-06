# AliAlpKUREL-16070006047_airbnbMidterm
# Airbnb Minimal Project
#### Developer - Ali Alp Kurel

### ER Diagram
![ER Diagram](./erDiagram.png)

### Technologies Used in the Project
- Dotnet 8 Web API
- Swagger/OpenAPI
- Entity Framework
- MySQL
- REST
- Others -> Markdown, JSON, Git

### Models
- **Booking** - Reservations
- **Comment** - Comments
- **Listing** - Listings
- **Location** - Address (Country, City, District, Neighborhood)
- **Role** - User Roles
- **User** - Users

### API Documentation

#### Authentication

- **POST /api/v1/Authentications/register** [Anonymous] - Registers a new user and returns a JWT token.
- **POST /api/v1/Authentications/Login** [Admin, Guest, Host] - Logs in and returns a JWT token.

#### Bookings

- **POST /api/v1/Bookings/AddBooking** [Guest] - Adds a reservation.
- **PUT /api/v1/Bookings/ApproveBooking** [Host] - Approves a reservation.
- **GET /api/v1/Bookings/GetAllBookings** [Admin] - Retrieves all bookings.

#### Comments

- **POST /api/v1/Comments/AddComment** [Host, Guest] - Adds a comment for a listing or user.
- **GET /api/v1/Comments/GetComments** [Admin] - Retrieves comments for a listing or user.
- **DELETE /api/v1/Comments/DeleteComment** [Host, Guest] - Deletes a comment.
- **PUT /api/v1/Comments/UpdateComment** [Host, Guest] - Updates a comment.

#### Listings

- **POST /api/v1/Listings/AddListing** [Host] - Adds a new listing.
- **GET /api/v1/Listings/GetAllListings** [*] - Retrieves all listings with filtering and pagination.
- **GET /api/v1/Listings/GetListingById** [*] - Retrieves a listing by its ID.
- **PUT /api/v1/Listings/UpdateListing** [Host] - Updates a listing.
- **DELETE /api/v1/Listings/DeleteListing** [Host] - Deletes a listing.

#### Locations

- **POST /api/v1/Locations/AddLocation** [Admin] - Adds a new location.
- **GET /api/v1/Locations/GetAllLocations** [*] - Retrieves all locations.

#### Users

- **GET /api/v1/Users/GetAllUsers** [Admin] - Retrieves all users.
- **GET /api/v1/Users/GetUserById** [Host, Guest, Admin] - Retrieves a user by their ID.
- **PUT /api/v1/Users/UpdateUser** [Guest, Host] - Updates a user.
- **DELETE /api/v1/Users/DeleteUser** [Admin] - Deletes a user.
- **PUT /api/v1/Users/ChangePassword** [Host, User] - Changes a user's password.

- **PUT /api/v1/Users/BanUser** [Admin] - Bans a specified user.
- **PUT /api/v1/Users/UnbanUser** [Admin] - Unbans a specified user.
- **GET /api/v1/Users/GetUserStats** [Admin] - Returns user role-based statistics.
- **GET /api/v1/Users/GetBookingStats** [Admin] - Returns booking statistics.
- **GET /api/v1/Users/TopRatedListings** [Admin] - Lists the highest-rated listings.
