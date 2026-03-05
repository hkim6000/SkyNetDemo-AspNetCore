<h2>SkyNetDemo Project: A SkyNet Framework Showcase</h2>

<h3>Modern ASP.NET Core Implementation | .NET 10</h3>
<h4>Educational purposes</h4>
<h4>Demo.Site:  https://www.theskylite.com/SkyLiteProject</h4>


<h3>Introduction</h3>
The SkyNetDemo project serves as a comprehensive and practical showcase of the SkyNet framework's capabilities for building modern, data-driven web applications on ASP.NET Core. It demonstrates how various UI controls and core framework features can be seamlessly integrated to create a cohesive, interactive, and personalized user experience.
The application presents a classic dashboard-style interface, typical of an administrative portal or a logged-in user's home page, and effectively utilizes the framework's server-centric, AJAX-driven architecture built on modern ASP.NET Core middleware.
SkyNet is the next-generation evolution of the SKYLITE framework(.Net framework 4.5+), rebuilt from the ground up for ASP.NET Core and .NET 10, providing a modern, cross-platform foundation while maintaining the same elegant patterns and developer-friendly API.

<h3>Architectural and Feature Highlights</h3>
<b>1. Core Architectural Patterns & Design Choices</b>b<br>
The project consistently follows several powerful architectural patterns that are central to the SkyNet philosophy:<br><br>

<b>Middleware-Based Architecture</b><br>
•	Custom IHandler Middleware: SkyNet operates as ASP.NET Core middleware, registered via app.UseMiddleware<IHandler>() in Program.cs<br>
•	Clean integration with the ASP.NET Core pipeline while maintaining framework independence<br>
•	Full control over request/response lifecycle without the constraints of MVC or Razor Pages<br>

<b>Single-Page Application (SPA)-like Navigation</b><br>
•	The application avoids full page reloads for most actions<br>
•	A main "shell" page (e.g., XysUser/XysUser.cs) loads the master layout<br>
•	Content area is dynamically replaced with partial views (MV or EV) using ApiResponse commands (SetElementContents)<br>
•	Creates a seamless, modern user experience with minimal JavaScript<br>

<b>Robust Role-Based Access Control (RBAC)</b><br>
•	Security is deeply integrated throughout the framework<br>
•	The WebBase class centralizes permission checking for both page access (ViewAccess()) and specific actions/methods (ViewMethods())<br>
•	UI is dynamically rendered based on these permissions, ensuring users only see and interact with functions they are authorized to use<br>
•	Database-driven permission matrix for flexible security configuration<br><br>

<b>2. Code-Level Best Practices</b><br>
<b>Security First</b><br>
•	Parameterized Queries: All data manipulation functions use SqlParameter lists to prevent SQL injection vulnerabilities<br>
•	Encrypted Passwords: Encryptor.EncryptData() for secure password storage<br>
•	Encrypted Cookies: SerializeObjectEnc() for secure session data<br>
•	Time-Limited Download Tokens: File downloads protected with encrypted, expiring URLs<br>
•	Method-Level Permissions: Every API method checked against user role permissions<br><br>
Clean Architecture
•	Centralized Constants: References structure provides single source for page names, session keys, element IDs
•	Separation of Concerns: Clear boundaries between data access, business logic, and presentation
•	Reusable Components: Base classes and shared utilities minimize code duplication
Thin Client Pattern
•	JavaScript files kept minimal
•	Primary responsibilities: DOM data collection and $ApiRequest initiation
•	All significant logic, validation, and UI orchestration handled securely on server
•	Reduces client-side complexity and attack surface
Modern .NET Practices
•	Built on ASP.NET Core and .NET 10
•	Cross-platform compatibility (Windows, Linux, macOS)
•	Can run on IIS, Kestrel, or Docker
•	Cloud-ready and container-friendly
________________________________________
Project Structure
SkyNetDemo/
├── Codes/              # Business logic classes (C#)
│   ├── XysUser         # User management main page
│   ├── XysUserMV       # User list view
│   ├── XysUserEV       # User edit view
│   ├── XysRole         # Role management
│   ├── XysPermission   # Permission matrix
│   └── ...
├── Bin/
│   └── SkyNet.dll ++        # ⭐ Core framework (single dependency)
├── appConfig/
│   └── application.cfg    # Application configuration
├── data/                  # Data storage folder
├── htmls/                 # HTML email templates
├── images/                # Static images
├── logs/                  # Application logs
├── scripts/               # JavaScript files
│   ├── Home.js
│   └── WebScript.js
├── styles/                # CSS stylesheets
│   ├── Home.css
│   └── WebStyle.css
├── temp/                  # Temporary files
├── Program.cs             # ASP.NET Core startup


**** unzip "bin.zip" and copy to project folder
________________________________________
Getting Started

//////////////////////////////////////////////////////////
//  0.prerequisite : install - view terminal & Edit project file
//  - dotnet add package Microsoft.Data.SqlClient
//  - dotnet add package System.Drawing.Common
//  - Add option to Properties/launchsetting.json file  : "hotReloadEnabled": false
//////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////
//1.Add HttpContext Service
//////////////////////////////////////////////////////////
builder.Services.AddHttpContextAccessor(); 
//////////////////////////////////////////////////////////

var app = builder.Build();

//////////////////////////////////////////////////////////
//2. use SKYNET.IHANDLER as middleware service
//////////////////////////////////////////////////////////
app.UseMiddleware<IHandler>();
//////////////////////////////////////////////////////////

 
//////////////////////////////////////////////////////////
//3. use static http class service
//////////////////////////////////////////////////////////
app.UseStaticHttpCurrent(); 
//////////////////////////////////////////////////////////

app.Run();


//////////////////////////////////////////////////////////
/// 4. define "StaticHttpCurrent"
//////////////////////////////////////////////////////////
public static class StaticHttpCurrent
{
    public static void UseStaticHttpCurrent(this IApplicationBuilder app)
    {
        var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
        var WebHostEnvironment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        SkyNet.HttpCurrent.Configure(httpContextAccessor, WebHostEnvironment);
    }
}
//////////////////////////////////////////////////////////________________________________________
Key Features Demonstrated
🔐 Complete Authentication System
•	Secure login with encrypted passwords
•	Email-based registration with OTP verification
•	Password reset flow
•	Session management with encrypted cookies
•	Two-factor authentication support
👥 User & Role Management
•	Full CRUD operations for users
•	Role assignment and management
•	Hierarchical permission system
•	User profile with photo upload
•	Account status management (active/suspended/terminated)
🛡️ Permission Matrix
•	Page-level access control
•	Menu-level permissions
•	Button/action-level permissions
•	Dynamic UI based on user role
•	Database-driven permission configuration
🌍 Multi-Language Support
•	Complete translation system
•	Language selection per user
•	Page-specific and global translations
•	Fallback language support
•	Dictionary management UI
📊 Data Management
•	Dynamic grid with sorting, paging, filtering
•	Excel-like data entry forms
•	File upload and management
•	Bulletin/announcement system
•	Audit logging (SYSDTE, SYSUSR fields)
📱 Modern UI Components
All components from SkyNet ToolKit:
•	Texts (text, password, email, date, number, etc.)
•	Dropdown (with database binding)
•	CheckBox, Switch, Button
•	TextArea, FileUpload, ImageBox
•	Grid, SQLGridSection, DataGrid
•	DialogBox, FilterSection, MenuList
•	TitleSection2, ItemPanel, TreeView
________________________________________
Framework Philosophy
Server-Centric, AJAX-Driven
SkyNet embraces a server-centric architecture where:
•	Business logic stays on the server (secure, maintainable)
•	Client makes lightweight AJAX calls via $ApiRequest
•	Server responds with ApiResponse commands (Navigate, SetElementContents, PopUpWindow, etc.)
•	Result: 80% less JavaScript, 100% C#/VB.NET for most logic
Less JavaScript, More C#
•	Write your application logic in C# (or VB.NET)
•	Use JavaScript only for UI interactions and DOM manipulation
•	Framework handles the communication layer
•	Stay in your comfort zone as a .NET developer
Rapid Development
•	Pre-built enterprise components (auth, permissions, i18n)
•	Minimal boilerplate code
•	Consistent patterns across entire application
•	From idea to production in days, not months
________________________________________
 
Technology Stack
•	Framework: ASP.NET Core (.NET 10)
•	Middleware: Custom SkyNet IHandler
•	Frontend: HTML5, CSS3, Minimal JavaScript
•	Authentication: Customizable, Cookie-based with encrypted AppKey in Showcase version
•	Deployment: IIS, Kestrel, Docker-ready
________________________________________
Comparison: SKYLITE vs SKYNET
Feature	SKYLITE	SKYNET
Platform	.NET Framework 4.5+	ASP.NET Core (.NET 10)
OS Support	Windows only	Cross-platform (Win/Linux/Mac)
Hosting	IIS only	IIS, Kestrel, Docker, Cloud
Architecture	HttpHandler	Middleware
API/Patterns	✅ Same	✅ Same
Code Migration	N/A	Near-zero (same folder structure)
Future Support	Maintenance mode	Active development
Migration from SKYLITE to SKYNET:
•	Same folder structure
•	Same API patterns (ApiRequest/ApiResponse)
•	Same UI controls (ToolKit)
•	Minimal code changes required
•	Replace SkyLite.dll with SkyNet.dll
•	Update configuration files
________________________________________
Learning Path
For New Users:
1.	Start Here: Examine XysSignin - simple authentication page
2.	Understand Navigation: See how Support loads partial views
3.	Learn Data Display: Study XysUserMV - SQLGridSection usage
4.	Master Data Entry: Review XysUserEV - form handling and validation
5.	Explore Advanced: Check XysPermission - dynamic permission matrix
For Experienced Developers:
1.	Review WebBase: Understand the core architecture
2.	Study References: See centralized constants pattern
3.	Examine ViewPart class: Understand the ViewModel pattern
4.	Check middleware integration: See Program.cs and IHandler
________________________________________
Use Cases
SkyNet is perfectly suited for:
•	✅ Enterprise internal portals
•	✅ Business management systems (ERP, CRM)
•	✅ Admin dashboards and back-office applications
•	✅ Line-of-business (LOB) applications
•	✅ Data-heavy CRUD applications
•	✅ Multi-tenant SaaS platforms
•	✅ Applications requiring strong RBAC
•	✅ Multi-language enterprise applications
________________________________________
Conclusion
The SkyNetDemo project is a masterclass in building a secure, scalable, and maintainable web application with the SkyNet framework on modern ASP.NET Core. Its architecture is perfectly suited for complex business applications like ERPs, CRMs, or internal admin portals where data integrity, role-based security, and rapid development of standardized forms are paramount.
SkyNet brings the proven patterns of SKYLITE to the modern .NET ecosystem, providing a clear migration path for legacy applications while enabling new projects to benefit from cross-platform, cloud-ready ASP.NET Core.
