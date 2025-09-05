# ChhayaNirh-ছায়ানীড় 🏠
SD PROJECT: ChhayaNirh-ছায়ানীড় - House Rent Platform for Dhaka, Bangladesh

## Team Members
| Name                   | Roll Number   | Email                               | Role                  |
|------------------------|---------------|-------------------------------------|-----------------------|
| Twaseen Tabassum       | 20220104134   | twaseen.cse.20220104134@aust.edu    | Backend, Frontend     |
| Mohsin Ul Hassan Nihal | 20220104135   | mohsin.cse.20220104135@aust.edu     | Backend, Frontend     |
| Ishrat Jahan Mim       | 20220104137   | ishratjahanmim2624@gmail.com        | Backend, Frontend     |

## Project Overview
"ChhayaNirh" is a house rent platform designed for Dhaka, Bangladesh. It connects homeowners and renters, providing secure verification, real-time location viewing via Google Maps, messaging between users, and a safe advance payment system.

## Title
🏠 ChhayaNirh-ছায়ানীড়

## Key Features
- **User Management:**
  - Role-based access. Admin, User(Homeowner, Renter).
  - Secure authentication (login/register).
  - Profile management with photo, email, phone, and NID verification.
    
- **House Posting & Searching:**
  - Homeowners can post rental houses with details, images, and location.
  - Renters can search for houses using filters.
  - Google Maps integration for accurate location display.
    
 - **User Verification:**
   - NID scan and latest electricity bill upload.
   - Admin approval ensures trust and security.
    
- **Payments:**
  - Renters can pay advance rent through the platform.
  - Service charges are handled automatically.
  - Currently supports Cash on Delivery.
   
- **Messaging & Communication:**
  - Built-in chat system between renters and homeowners.
  - Chatbot support for FAQs.
    
- **Admin Role:**
  - Verify users and documents.
  - Approve/reject house posts.
  - Manage payments and service charges.
   
## Target Audience
ChhayaNirh is designed for:

- **Homeowners:** Post rental houses, track inquiries, and receive payments.
- **Renters:** Search, view, and rent houses safely.
- **Admins:** Manage users, posts, and payments.

## User Interface
- **Home Page:**
- Navigation (Login/Signup).
- Featured posts.
- Search and filter options.

- **Profile Page:**
- View and edit personal information.
- Upload verification documents.

- **Post Page (Users only):**
- Add images, location, rent, and other house details.
- Submit for admin approval.

- **Chat Page:**
- Communicate with other users.
- Chatbot support for FAQs.

- **Admin Panel:**
- Manage users and house posts.
- Verify documents.
- Track payments and service charges.
  
## Usage
1. Access the platform through your browser.
2. Sign up as a Homeowner or Renter.
3. Homeowners can create house posts; Renters can search and request rentals.
4. Use the chat system or chatbot for support.
5. Admins can manage users, posts, and payments.

## Setup Instructions

### Prerequisites
1. Visual Studio or preferred IDE for ASP.NET MVC
2. .NET Framework (4.8 recommended)
3. SQL Server (for database)
4. Browser (Chrome, Edge, Firefox)

### Running the Project
1. Open the project in Visual Studio.
2. Update connection string in Web.config for your SQL Server.
3. Launch the website using Visual Studio (F5).
4. Sign up as a user or log in using admin credentials.
5. Use the platform features as described above.

### Notes
1. Admin accounts cannot sign up via the frontend; create them directly in the database with UserType = Admin.
2. Only verified users can post or rent houses.
3. All payments are currently Cash on Delivery.

Now, you can run and use the application.
