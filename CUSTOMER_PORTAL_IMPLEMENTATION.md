# Customer Portal Implementation - Progress Summary

## Completed Work

### Phase 1: Database & Domain Layer ✅
- **Customer Entity**: Created `Customer.cs` with all required fields (name, email, phone, company, address, etc.)
- **Event Entity Updates**: Added customer linkage, cancellation request fields (CancellationRequested, CancellationRequestedAt, CancellationReason)
- **ApplicationUser Updates**: Added CustomerId property to link users to customers
- **Database Migration**: Created migration `AddCustomerAndEventCancellation` for all schema changes
- **ApplicationDbContext**: Added Customers DbSet and configured relationships

### Phase 2: Application Layer (CQRS) ✅
**Commands Created:**
- `CreateCustomerCommand` - Creates new customer profiles
- `LinkCustomerToEventCommand` - Links a customer to an event
- `RequestEventCancellationCommand` - Allows customers to request event cancellation

**Queries Created:**
- `GetAllCustomersQuery` - Retrieves all customers
- `GetCustomerByIdQuery` - Gets customer by ID
- `GetCustomerByUserIdQuery` - Gets customer by user ID
- `SearchCustomersQuery` - Searches customers by name or email

**Supporting Infrastructure:**
- `CustomerDTO` - Data transfer object for customer data
- `CustomerMapper` - Maps between Customer entity and DTO
- `ICustomerRepository` - Repository interface
- `CustomerRepository` - Repository implementation
- All command and query handlers implemented in Application layer
- Registered all handlers in DependencyInjection.cs

### Phase 3: Customer Portal Blazor App ✅
- Created new Blazor Server app in CustomerPortal folder
- Configured to use **shared database** (lotus.db) with main LotusPlanningApp
- Added references to Application, Entities, and Infrastructure projects
- Copied and adapted Account components from main app
- Registered all infrastructure services (repositories, email, calendar)
- Registered all CQRS command/query handlers
- Added Blazor Bootstrap for UI components
- Added "Customer" role to Program.cs seed data

**File Structure:**
```
CustomerPortal/
├── Components/
│   ├── Account/          # Authentication & account management
│   ├── Layout/           # Main layout and navigation
│   └── Pages/            # Page components
├── Program.cs            # Startup configuration
├── appsettings.json      # Configuration
└── CustomerPortal.csproj # Project file
```

## Remaining Work

### Phase 4: Customer Portal Features (In Progress)

#### 4.1 Registration Enhancements
- [ ] Update Register.razor to automatically assign "Customer" role on registration
- [ ] Create Customer profile automatically when user registers
- [ ] Send email notification to admin when customer registers
- [ ] Add reCAPTCHA to registration form (security requirement)

#### 4.2 Customer Dashboard
- [ ] Create Dashboard.razor page showing:
  - Summary of customer's events
  - Upcoming events list
  - Past events list
  - Quick stats (total events, pending requests, etc.)
- [ ] Use Bootstrap cards for visual appeal
- [ ] Make responsive for mobile devices

#### 4.3 Event Management
- [ ] Create CreateEvent.razor page for customers to submit new events
  - Form fields: Name, StartDate, EndDate, Location, Description
  - Contact info fields: ContactPerson, ContactPhone, ContactEmail
  - Form validation
  - Submit creates event with status "Requested"
  - Automatically link event to logged-in customer
- [ ] Create EventList.razor to show customer's events
- [ ] Create EventDetails.razor for viewing event details
- [ ] Add cancellation request button on event details
  - Modal popup to enter cancellation reason
  - Updates event with cancellation request flag
  - Admin must approve cancellation

#### 4.4 Localization (Dutch/English)
- [ ] Create Resources folder with resource files
  - Resources.nl-NL.resx (Dutch)
  - Resources.en-US.resx (English)
- [ ] Add IStringLocalizer to components
- [ ] Create language switcher component in navigation
- [ ] Store user's language preference in database or cookie
- [ ] Translate all UI text in Customer Portal

#### 4.5 UI/UX Improvements
- [ ] Update NavMenu.razor with customer-specific links
- [ ] Add custom styling for branding
- [ ] Ensure mobile responsiveness
- [ ] Add loading spinners
- [ ] Add toast notifications for success/error messages
- [ ] Add icons using Bootstrap Icons

### Phase 5: Admin Integration

#### 5.1 Customer Management in LotusPlanningApp
- [ ] Create Customers.razor page to list all customers
  - Table with customer details
  - Search functionality
  - Edit/Delete actions
- [ ] Update EventEdit.razor to include customer search/select
  - Dropdown or autocomplete for customer selection
  - Search customers by name or email
  - Display selected customer info

#### 5.2 Cancellation Workflow
- [ ] Add "Cancellation Requests" section to Events page
  - Show events with pending cancellation requests
  - Approve/Deny buttons
  - Approving sets event status to "Cancelled"
- [ ] Update EventDetails.razor to show cancellation request info
  - Display if cancellation requested
  - Show cancellation reason
  - Show who requested and when
- [ ] Send email notification to customer when cancellation is approved/denied

#### 5.3 Event Display Enhancements
- [ ] Show customer name on event cards
- [ ] Add customer filter to events list
- [ ] Link from event to customer details

### Phase 6: Testing & Documentation

#### 6.1 Testing
- [ ] Test customer registration with reCAPTCHA
- [ ] Test customer profile creation on registration
- [ ] Test admin email notification on registration
- [ ] Test customer login
- [ ] Test event creation by customer
- [ ] Test event visibility (customer sees only their events)
- [ ] Test cancellation request flow
- [ ] Test cancellation approval/denial
- [ ] Test language switching
- [ ] Test on mobile devices

#### 6.2 Documentation
- [ ] Create CUSTOMER_PORTAL_SETUP.md
  - How to run CustomerPortal
  - Port configuration
  - Database setup
- [ ] Create CUSTOMER_PORTAL_FEATURES.md
  - User guide for customers
  - Screenshots of key features
- [ ] Update main README.md with CustomerPortal info
- [ ] Create LOCALIZATION_GUIDE.md for translators

#### 6.3 Security
- [ ] Run CodeQL security scan
- [ ] Fix any security vulnerabilities
- [ ] Add CSRF protection
- [ ] Validate all user inputs
- [ ] Ensure customers can only access their own data

## Architecture Overview

### Database Schema
```
ApplicationUser (Identity)
├── StaffId (nullable) - Links to Staff for admin/Lotus users
└── CustomerId (nullable) - Links to Customer for customer portal users

Customer
├── UserId - Links back to ApplicationUser
└── Events - Collection of events created by this customer

Event
├── CustomerId (nullable) - Who created/requested this event
├── CancellationRequested - Boolean flag
├── CancellationRequestedAt - Timestamp
└── CancellationReason - Text explanation
```

### Application Flow

**Customer Registration:**
1. User fills out registration form (with reCAPTCHA)
2. ApplicationUser created with "Customer" role
3. Customer profile created and linked to user
4. Email sent to admin
5. User can log in immediately (no approval needed for customers)

**Event Creation:**
1. Customer logs into CustomerPortal
2. Navigates to "Create Event" page
3. Fills out event form
4. Event created with status "Requested" and linked to customer
5. Admin reviews in LotusPlanningApp and can approve/schedule

**Cancellation Request:**
1. Customer views their event
2. Clicks "Request Cancellation"
3. Enters reason in modal
4. Event marked with CancellationRequested flag
5. Admin sees request in LotusPlanningApp
6. Admin approves/denies
7. Customer notified via email

### Technology Stack
- **Frontend**: Blazor Server, Bootstrap, Blazor Bootstrap
- **Backend**: ASP.NET Core 10.0, C# 12
- **Database**: SQLite (shared between apps)
- **Authentication**: ASP.NET Core Identity
- **Architecture**: CQRS pattern, Clean Architecture
- **Localization**: ASP.NET Core Localization with resource files

## Next Steps

### Immediate Priorities (Next Session)
1. Update Registration to create Customer profile
2. Create Customer Dashboard
3. Create Event Creation form for customers
4. Add cancellation request feature
5. Test basic customer workflow

### Future Enhancements (If Time Permits)
1. Add reCAPTCHA integration
2. Implement full localization
3. Admin customer management pages
4. Email notifications
5. Advanced UI polish

## Notes

- **Shared Database**: Both apps use the same SQLite database at `LotusPlanningApp/LotusPlanningApp/AppData/lotus.db`
- **Separate Apps**: CustomerPortal runs on a different port than LotusPlanningApp
- **Shared Code**: Both apps reference the same Application, Entities, and Infrastructure projects
- **Customer vs Lotus**: "Customer" role for customer portal users, "Lotus" role for staff users
- **Admin Access**: Admins can access both LotusPlanningApp and CustomerPortal if needed

## Commands

**Build CustomerPortal:**
```bash
cd /home/runner/work/LotusPlanningApp/LotusPlanningApp
dotnet build CustomerPortal/CustomerPortal.csproj
```

**Run CustomerPortal:**
```bash
cd CustomerPortal
dotnet run
```

**Apply Migrations:**
```bash
cd Infrastructure
dotnet ef database update --startup-project ../LotusPlanningApp/LotusPlanningApp
```
