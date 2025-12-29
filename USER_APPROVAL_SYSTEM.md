# User Approval System - Implementation Guide

## Overview
A complete admin approval system has been implemented for the Lotus Planning App. Users must be approved by an administrator before they can log in to the system.

## System Architecture

### 1. **Database Schema** (`ApplicationUser` model)
The `ApplicationUser` class has been extended with approval-related fields:

```csharp
public bool IsApproved { get; set; }                    // Indicates user approval status
public DateTime? ApprovedAt { get; set; }               // When the user was approved
public string? ApprovedByUserId { get; set; }           // Which admin approved them
public DateTime RegisteredAt { get; set; }              // When the user registered
```

**Migration**: `20251229222354_AddUserApprovalFields.cs`

### 2. **User Registration Flow**

#### Registration Page: `/Account/Register`
- Users fill in email, password, and confirm password
- Upon successful registration:
  - Account is created with `IsApproved = false`
  - User is assigned the "Lotus" role
  - User is redirected to the **Pending Approval** page
  - User **cannot** log in until approved by an admin

#### Pending Approval Page: `/Account/PendingApproval`
- Displays a professional notification that the account is pending approval
- Informs user they'll receive an email when approved
- Provides links back to home and login pages

### 3. **Login Process** (`/Account/Login`)
The login flow includes an approval check:

```csharp
// Check if user exists and is approved
var user = await UserManager.FindByEmailAsync(Input.Email);
if (user != null && !user.IsApproved)
{
    errorMessage = "Your account is pending approval by an administrator. Please wait for approval before logging in.";
    return;
}
```

**Result**: If not approved, login is blocked with a clear message.

### 4. **Admin Approval Screen** (`/admin/user-approvals`)

**Access**: Only users with the "Admin" role can access this page.

**Features**:

#### Pending Approvals Section
- Shows all unapproved users registered with the "Lotus" role
- Displays:
  - Email address
  - Username
  - Registration date/time
  - Action buttons
- **Actions**:
  - ✅ **Approve**: Sets `IsApproved = true`, records approval time and admin ID
  - ❌ **Reject**: Deletes the user account

#### Approved Users Section
- Shows all approved users
- Displays:
  - Email address
  - Username
  - Registration date
  - Approval date
  - Action buttons
- **Actions**:
  - 🔄 **Revoke**: Revokes approval (sets `IsApproved = false`)

#### Visual Features
- Responsive Bootstrap design
- Toast notifications for success/error messages
- Loading spinner while fetching data
- Badge showing count of pending approvals
- Clean table-based layout with Bootstrap icons

### 5. **Navigation**

The admin approval page is linked in the navigation menu (`/Components/Layout/NavMenu.razor`):

```
🔗 Gebruikers goedkeuren (User Approvals)
```

This link is only visible to users with the "Admin" role.

### 6. **Admin User Seeding** (`Program.cs`)

On application startup, the system:
1. Creates "Admin" and "Lotus" roles if they don't exist
2. Creates a default admin user:
   - **Email**: `admin@lotus-tilburg.nl`
   - **Password**: `Test123!`
   - **Status**: Automatically approved
   - **Role**: Admin

## User Flows

### New User Registration Flow
```
User Registers
    ↓
Account Created (IsApproved = false)
    ↓
Redirected to Pending Approval Page
    ↓
Admin Reviews in User Approvals Dashboard
    ↓
Admin Approves ✅
    ↓
User Can Now Login
```

### Login Attempt (Not Approved)
```
User Enters Credentials
    ↓
System Checks IsApproved
    ↓
IsApproved = false?
    ↓
❌ Login Blocked
   Message: "Your account is pending approval..."
```

### Login Attempt (Approved)
```
User Enters Credentials
    ↓
System Checks IsApproved
    ↓
IsApproved = true?
    ↓
✅ Login Successful
   User Redirected to Dashboard
```

## How to Use

### For Administrators

1. **Access User Approvals Dashboard**:
   - Log in with admin credentials
   - Click "Gebruikers goedkeuren" in the navigation menu
   - Or navigate directly to `/admin/user-approvals`

2. **Approve a User**:
   - Locate the user in the "Pending Approvals" section
   - Click the green ✅ **Approve** button
   - User will appear in the "Approved Users" section
   - User can now log in

3. **Reject a User**:
   - Click the red ❌ **Reject** button
   - User account is deleted
   - User will not be able to access the system

4. **Revoke User Approval**:
   - In the "Approved Users" section
   - Click the yellow 🔄 **Revoke** button
   - User will return to pending status
   - User cannot log in until re-approved

### For New Users

1. **Register**:
   - Go to `/Account/Register`
   - Enter email and password
   - Click Register

2. **Wait for Approval**:
   - You'll see the "Pending Approval" page
   - You'll be notified when an admin approves your account
   - You can return to the login page once approved

3. **Login**:
   - Once approved, log in at `/Account/Login`
   - Use your registered email and password

## Files Involved

### Core Files
- `Infrastructure/Data/ApplicationUser.cs` - Extended user model with approval fields
- `Infrastructure/Data/Migrations/20251229222354_AddUserApprovalFields.cs` - Database migration
- `LotusPlanningApp/Program.cs` - Role seeding and admin user creation
- `Components/Account/Pages/Register.razor` - Registration flow with pending approval redirect
- `Components/Account/Pages/Login.razor` - Login with approval check
- `Components/Account/Pages/PendingApproval.razor` - User-facing pending approval page
- `Components/Pages/UserApprovals.razor` - Admin approval dashboard
- `Components/Layout/NavMenu.razor` - Navigation menu with admin-only approval link

## Security Considerations

✅ **Approved by role**: Only users with "Admin" role can access the approval screen  
✅ **Status check on login**: Prevents unapproved users from gaining access  
✅ **Approval audit trail**: Records who approved and when  
✅ **Default admin account**: Pre-configured with temporary password (change on first login)  

## Future Enhancements

- Email notifications when users are approved/rejected
- Email notifications for admins when new users register
- Bulk approval/rejection of users
- User profile fields (name, phone, role) in approval form
- Approval notes/comments from admin
- Activity logs for all approvals/rejections
- Automatic email confirmation requirement

## Testing Checklist

- [ ] Register as a new user
- [ ] Verify you're redirected to Pending Approval page
- [ ] Try to login (should be blocked)
- [ ] Log in as admin
- [ ] Navigate to User Approvals page
- [ ] See pending user in the list
- [ ] Approve the user
- [ ] Verify user can now login
- [ ] Revoke approval and verify user cannot login
- [ ] Test reject functionality

## Database Reset

To reset the system and start fresh:

```bash
# In the LotusPlanningApp directory
dotnet ef database drop --project ../../Infrastructure/Infrastructure.csproj
dotnet ef database update --project ../../Infrastructure/Infrastructure.csproj
```

This will:
1. Drop the existing database
2. Apply all migrations fresh
3. Re-seed the admin user

**New Admin Credentials**:
- Email: `admin@lotus-tilburg.nl`
- Password: `Test123!`
