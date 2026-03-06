<h2>SkyNetDemo Project: A SkyNet Framework Showcase</h2>

<h3>Modern ASP.NET Core Implementation | .NET 10</h3>


<br>

- SKYLITE framework (C#/VB) <br>
Platform: IIS / .Net framework 4.5 +<br>

- SKYNET framework (C# only)<br>
Platform: ASP.NET Core / .NET 10<br>
Architecture: Middleware-based <br>

- Technical Documents : https://www.theskylite.com/document<br>
- Showcase Demo. Site: https://www.theskylite.com/SkyLiteProject<br>
- Download GitHub: https://github.com/hkim6000/SkyNetDemo-AspNetCore<br><br>

support: hkim6000@gmail.com<br><br>


<h3>Introduction</h3>
The SkyNetDemo project serves as a comprehensive and practical showcase of the SkyNet framework's capabilities for building modern, data-driven web applications on ASP.NET Core. It demonstrates how various UI controls and core framework features can be seamlessly integrated to create a cohesive, interactive, and personalized user experience.
The application presents a classic dashboard-style interface, typical of an administrative portal or a logged-in user's home page, and effectively utilizes the framework's server-centric, AJAX-driven architecture built on modern ASP.NET Core middleware.
SkyNet is the next-generation evolution of the SKYLITE framework(.Net framework 4.5+), rebuilt from the ground up for ASP.NET Core and .NET 10, providing a modern, cross-platform foundation while maintaining the same elegant patterns and developer-friendly API.

<h3>Architectural and Feature Highlights</h3>
<b>1. Core Architectural Patterns & Design Choices</b><br>
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

<b>2. Code-Level Best Practices</b><br><br>
<b>Security First</b><br>
•	Parameterized Queries: All data manipulation functions use SqlParameter lists to prevent SQL injection vulnerabilities<br>
•	Encrypted Passwords: Encryptor.EncryptData() for secure password storage<br>
•	Encrypted Cookies: SerializeObjectEnc() for secure session data<br>
•	Time-Limited Download Tokens: File downloads protected with encrypted, expiring URLs<br>
•	Method-Level Permissions: Every API method checked against user role permissions<br><br>
<b>Clean Architecture</b><br>
•	Centralized Constants: References structure provides single source for page names, session keys, element IDs<br>
•	Separation of Concerns: Clear boundaries between data access, business logic, and presentation<br>
•	Reusable Components: Base classes and shared utilities minimize code duplication<br><br>
<b>Thin Client Pattern</b><br>
•	JavaScript files kept minimal<br>
•	Primary responsibilities: DOM data collection and $ApiRequest initiation<br>
•	All significant logic, validation, and UI orchestration handled securely on server<br>
•	Reduces client-side complexity and attack surface<br><br>
<b>Modern .NET Practices</b><br>
•	Built on ASP.NET Core and .NET 10<br>
•	Cross-platform compatibility (Windows, Linux, macOS)<br>
•	Can run on IIS, Kestrel, or Docker<br>
•	Cloud-ready and container-friendly<br>
________________________________________

<h3>Project Structure</h3><br>
ASPNETCoreWeb/<br>
├── Codes/              # web page classes (C#)<br>
│   ├── XysUser         # no routing rule, free webpage class location<br>
│   ├── XysUserMV       # web page class can be anywhere in the project folder<br>
│   ├── XysUserEV       <br>
│   ├── XysRole         <br>
│   ├── XysPermission   <br>
│   └── ...<br>
├── bin\Debug\net10.0<br>
│      └── <b>SkyNet.dll<b> ++ # ⭐ Core framework (single dependency)<br>
├── appConfig/<br>
│   └── application.cfg    # Application configuration<br>
├── data/                  # Data storage folder<br>
├── htmls/                 # HTML email templates<br>
├── images/                # Static images<br>
├── logs/                  # Application logs<br>
├── scripts/               # JavaScript files<br>
│   ├── Home.js<br>
│   └── WebScript.js<br>
├── styles/                # CSS stylesheets<br>
│   ├── Home.css<br>
│   └── WebStyle.css<br>
├── temp/                  # Temporary files<br>
├── Program.cs             # ASP.NET Core startup<br><br>


**** unzip "bin_folder_backup.zip" to restore project-bin-folder<br>

----------------------------------------------------------------------------------------<br>

<h3>Getting Started</h3><br>

<b>program.cs</b><br><br>

using System.Reflection.Metadata;<br>
using SkyNet;<br>
<br>
var builder = WebApplication.CreateBuilder(args);<br>
<br>
//////////////////////////////////////////////////////////<br>
//  0.prerequisite : install - view terminal & Edit project file<br>
//  - dotnet add package Microsoft.Data.SqlClient<br>
//  - dotnet add package System.Drawing.Common<br>
//  - Add option to Properties/launchsetting.json file  : "hotReloadEnabled": false<br>
//////////////////////////////////////////////////////////<br>

//////////////////////////////////////////////////////////<br>
//1.Add HttpContext Service<br>
builder.Services.AddHttpContextAccessor(); <br>
//////////////////////////////////////////////////////////<br>

var app = builder.Build();<br>

//////////////////////////////////////////////////////////<br>
//2. use SKYNET.IHANDLER as middleware service<br>
//////////////////////////////////////////////////////////<br>
app.UseMiddleware<IHandler>();<br>
//////////////////////////////////////////////////////////<br>
 
//////////////////////////////////////////////////////////<br>
//3. use static http class service<br>
//////////////////////////////////////////////////////////<br>
app.UseStaticHttpCurrent(); <br>
//////////////////////////////////////////////////////////<br>
<br>
app.Run();<br>
<br>
//////////////////////////////////////////////////////////<br>
//////////////////////////////////////////////////////////<br>

<br>
<h3>Key Features Demonstrated</h3><br>
<b>Complete Authentication System</b><br>
•	Secure login with encrypted passwords<br>
•	Email-based registration with OTP verification<br>
•	Password reset flow<br>
•	Session management with encrypted cookies<br>
•	Two-factor authentication support<br><br>
<b>User & Role Management</b><br>
•	Full CRUD operations for users<br>
•	Role assignment and management<br>
•	Hierarchical permission system<br>
•	User profile with photo upload<br>
•	Account status management (active/suspended/terminated)<br><br>
<b>Permission Matrix</b><br>
•	Page-level access control<br>
•	Menu-level permissions<br>
•	Button/action-level permissions<br>
•	Dynamic UI based on user role<br>
•	Database-driven permission configuration<br><br>
<b>Multi-Language Support</b><br>
•	Complete translation system<br>
•	Language selection per user<br>
•	Page-specific and global translations<br>
•	Fallback language support<br>
•	Dictionary management UI<br><br>
<b>Data Management</b><br>
•	Dynamic grid with sorting, paging, filtering<br>
•	Excel-like data entry forms<br>
•	File upload and management<br>
•	Bulletin/announcement system<br>
•	Audit logging (SYSDTE, SYSUSR fields)<br><br>
<b>Modern UI Components</b><br>
All components from SkyNet ToolKit:<br>
•	Texts (text, password, email, date, number, etc.)<br>
•	Dropdown (with database binding)<br>
•	CheckBox, Switch, Button<br>
•	TextArea, FileUpload, ImageBox<br>
•	Grid, SQLGridSection, DataGrid<br>
•	DialogBox, FilterSection, MenuList<br>
•	TitleSection2, ItemPanel, TreeView<br><br>

<h3>Framework Philosophy</h3>
<b>Server-Centric, AJAX-Driven</b>br>
<b>SkyNet embraces a server-centric architecture where:</b><br>
•	Business logic stays on the server (secure, maintainable)<br>
•	Client makes lightweight AJAX calls via $ApiRequest<br>
•	Server responds with ApiResponse commands (Navigate, SetElementContents, PopUpWindow, etc.)<br>
•	Result: 80% less JavaScript, 100% C# for most logic<br>
Less JavaScript, More C#<br>
•	Write your application logic in C#<br>
•	Use JavaScript only for UI interactions and DOM manipulation<br>
•	Framework handles the communication layer<br>
•	Stay in your comfort zone as a .NET developer<br>
Rapid Development<br>
•	Pre-built enterprise components (auth, permissions)<br>
•	Minimal boilerplate code<br>
•	Consistent patterns across entire application<br>
•	From idea to production in days, not months<br><br>

 
<h3>Technology Stack</h3>
•	Framework: ASP.NET Core (.NET 10)<br>
•	Middleware: Custom SkyNet IHandler<br>
•	Frontend: HTML5, CSS3, Minimal JavaScript<br>
•	Authentication: Customizable, Cookie-based with encrypted AppKey in Showcase version<br>
•	Deployment: IIS, Kestrel, Docker-ready<br>
  
<h3>Use Cases</h3>
<b>SkyNet is perfectly suited for:</b><br>
•	✅ Enterprise internal portals<br>
•	✅ Business management systems (ERP, CRM)<br>
•	✅ Admin dashboards and back-office applications<br>
•	✅ Line-of-business (LOB) applications<br>
•	✅ Data-heavy CRUD applications<br>
•	✅ Multi-tenant SaaS platforms<br>
•	✅ Applications requiring strong RBAC<br>
•	✅ Multi-language enterprise applications<br><br>

<h3>Conclusion</h3><br>
The SkyNetDemo project is a masterclass in building a secure, scalable, and maintainable web application with the SkyNet framework on modern ASP.NET Core. Its architecture is perfectly suited for complex business applications like ERPs, CRMs, or internal admin portals where data integrity, role-based security, and rapid development of standardized forms are paramount.<br>
SkyNet brings the proven patterns of SKYLITE to the modern .NET ecosystem, providing a clear migration path for legacy applications while enabling new projects to benefit from cross-platform, cloud-ready ASP.NET Core.<br>


