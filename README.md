# Project Information


# Structure Project

Aplikasi terbagi menjadi dua project besar **Service Project**. **Service Project** merupakan tempat dimana kita mengelola semua Code berurusan dengan API dan Bisnis Logik.  


## Structure Service Project
```bash
├── Idams.Core
│   ├── Enums
│   ├── ── Services
│   ├── ── ── xxx
│   ├── Extenstions
│   ├── Model
│   ├── Repositories
│   ├── Services
├── Idams.Infrastructure
│   ├── Services
│   ├── Utils
├── Idams.Infrastructure.EntityFramework
│   ├── Repositories
├── Idams.WebApi
│   ├── Configurations
│   ├── ── AutoMapperConfiguration.cs
│   ├── Controllers
├── Idams.WebApi.Utils
│   ├── Attribute
│   ├── Extenstions
└── README.md
```
### Idams.WebApi\Configurations
Folder **configuration** menyimpan semua class yang berhubungan dengan Config aplikasi, contoh: Jika kita ingin menambahkan Class baru untuk membaca "AppSetting.json", kita bisa menambahkan class pada folder ini, atau jika kita ingin melakukan mapping antara class kita bisa menambahkan nya pada  AutoMapperConfiguration.cs

### Enums\Services
Pada folder "**Enums\Services**"  menyimpan semua kode yang akan digunakan untuk return logic dari Service. 
```bash
├── Enums
│   ├── ApiConstants.cs
│   ├── SmtpConstants.cs
│   ├── LoginConstants.cs
│   ├── LoginConstants.cs
│   └── RoleConstants.cs
```


### Controllers
Pada folder "**Controllers**"  menyimpan semua kode yang berhubungan dengan API,  jika kita mengigninkan API yang terproteksi (Harus mengirimkan ApiAuthorize header). Kita bisa menambahkan Attribute [**Shared.Configurations.Attributes.Authorize**]
```C#
    [Route("user/auth")]
    [ApiController]
    public class AuthController : ApiController
    {
	    [ApiAuthorize]
        [HttpGet("current")]
        public IActionResult Get()
        {
            return Ok(_httpContextAccessor.HttpContext?.CurrentUser());
        }
    }
```


### Extensions
Pada folder "**Extensions**" kita dapat menambahkan Custom Extensions jika membutuhkan. contoh : Kita ingin menambahkan Fungsi untuk Merubah PascalCase menjadi StatementCase yang dapat di gunakan untuk semua string (type object). maka kita dapat menyimpan Fungsi tersebut pada folder ini. dan pada setiap name file tambahkan "**Extensions**" pada akhir file

```C#    
    public static class StringExtensions
    {
        public static string ToStatementCase(this string value)
        {
			```
			```
			```
        }
    }
	"SampleData".ToStatementCase() // output : "Sample Data"
```




### Utils
Pada Project "**Utils**" kita dapat menambahkan fungsi yang akan membantu untuk membuat code lebih mudah namun kita tidak ingin membuat nya secara general (static). contoh : "Validasi email  yang valid"

```C#    
     public class EmailHelper
    {
        public static bool IsValid(string email)
        {
			```
			```
			```
        }
    }
	if ( EmailHelper.IsValid("abc@a.com")){
				```
	}
```



### Infrastructure.EntityFramework\Repositories

Pada folder "**Repositories**"   digunakan untuk menyimpan semua class yang berhubungan dengan Persistance (Database, File, maupun API).  untuk repository harus dipisahkan menjadi dua file (Interface (Simpan pada project Core\Repositories) & Implementation)
tambahkan informasi jenis Repository yang digunakan pada awal file name, contoh **EF** untuk repository  menggunakan Entity Framework, **Rest** untuk repository  menggunakan Rest API pada pengambilan atau perubahan data
```bash
├── Infrastructures
│   ├── Repositories
│   ├── ├──  User
│   ├── ├── └──  EFUserRepository.cs
│   ├── ├──  Template
│   ├── ├── └──  EFTemplateRepository.cs
│   ├── ├──  Activity
│   └── ─── ───  RestActivityRepository.cs
```

### Infrastructures\Services

Pada folder "**Services**"   digunakan untuk menyimpan semua class yang berhubungan dengan business Logic.
Untuk setiap Service harus dipisahkan menjadi dua file (Interface (Simpan pada project Core\Services) & Implementation)
```bash
├── Infrastructures
│   ├── Services
│   ├── ├──  User
│   ├── ├── └──  UserService.cs
│   ├── ├──  Template
│   └── ─── ───  TemplateService.cs
```

### Utils

Pada folder "**Utils**"   digunakan untuk menyimpan semua class yang berhubungan dengan External.
Untuk setiap Utils harus dipisahkan menjadi dua file (Interface & Implementation). contoh : SmtpEmailService di gunakan untuk mengirim email melalui SMTP, dan jiga kita memiliki implementasi menggunakan API, maka kita akan membuat ApiEmailService

```bash
├── Infrastructures
│   ├── Utils
│   ├── ├──  User
│   ├── ├── ├──  IUserCurrentService.cs
│   ├── ├── └──  UserCurrentService.cs
│   ├── ├──  Email
│   ├── ├── ├──  IEmailService.cs
│   └── ─── ───  SmtpEmailService.cs
```
Namun secara default kita memiliki **Shared Code** untuk Utils(ICurrentUserService) . Dan jika kita akan melakukan perubahan/penambahan fungsi pada code tersebut. kita harus melakukan commit pada "Sub Module". jika tidak perubahan yang kita lakukan tidak akan tercatat pada **git**.

### Middlewares
Folder **Middlewares** menyimpan semua class yang berhubungan dengan Middleware pada pipeline yang kita setup, contoh : jika kita ingin menambakan Loggin pada setiap request maka kita dapat membuat LoggingMiddleware dan menyimpan pada folder ini

Namun secara default kita memiliki **Shared Code** untuk Helpers (ExceptionMiddleware, ApiKeyMiddleware, JwtMiddleware) . Dan jika kita akan melakukan perubahan/penambahan fungsi pada code tersebut. kita harus melakukan commit pada "Sub Module". jika tidak perubahan yang kita lakukan tidak akan tercatat pada **git**.

###  Models\Dtos
Folder **Dtos** digunakan untuk meyimpan class yang digunakan untuk melakukan Transfer dari Serive ke Controller

```bash
├── Models
│   ├── Dtos
│   ├── ├──  User
│   ├── ├── ├──  UserDto.cs
│   ├── ├──  Email
│   └── ─── ───  EmailDto.cs
```
Namun secara default kita memiliki **Shared Code** untuk Dtos (PagedDto) . Dan jika kita akan melakukan perubahan/penambahan fungsi pada code tersebut. kita harus melakukan commit pada "Sub Module". jika tidak perubahan yang kita lakukan tidak akan tercatat pada **git**.

###  Models\Entities
Folder **Entities** digunakan untuk menyimpan class yang akan gunakan untuk melakukan penyimpanan data pada persistence. yang kita gunakan saat ini adalah EntityFramework, maka kita harus menambahkan setiap entity yang akand i gunakan kedalam DBContext dan melakukan Configuration yang akan di simpan pada folder **EntityFramework\Configurations**
```bash
├── Models
│   ├── Entities
│   ├── ├── EntityFramework  
│   ├── ├── ├── Configurations
│   ├── ├── ├── ─── UserConfiguration.cs
│   ├── ├── ├── ─── EmailConfiguration.cs
│   ├── ├── ─── DbContext.cs
│   ├── ├──  User.cs
│   └── ───  Email.cs
```

```C#    
      public partial class UserDbContext : DbContext
     {
        public UserDbContext()
        {
        }

        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

       ```
       ```
       ```
        public virtual DbSet<MdEndpoint> Endpoints { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ```
            ```
            // Tambahkan Configuration untuk mapping class ke dalam Database 
            modelBuilder.ApplyConfiguration(new MdEndpointConfiguration());
        }
    }
```
Namun secara default kita memiliki **Shared Code** untuk Entities (MdEndpointConfiguration, AuditEntity, MdEndpoint) . Dan jika kita akan melakukan perubahan/penambahan fungsi pada code tersebut. kita harus melakukan commit pada "Sub Module". jika tidak perubahan yang kita lakukan tidak akan tercatat pada **git**.


###  Core\Filters
Folder **Filters** digunakan untuk meyimpan property untuk filter data pada Service/Repository.
```bash
├── Filters
│   ├── PageFilter
│   ├── RoleFilter
```
```C#    
    public class RoleFilter
    {
        public string Name { get; set; }
        public string Status { get; set; }        
    }
```
Maka paka RoleFilter, kita dapat melakukan filter by "Name" & Status,


###  Idams.WebApi\Models\Requests
Folder **Requests** digunakan untuk meyimpan semua property yang akan dikenal oleh FrontEnd. Folder **Requests** digunakan untuk meyimpan class yang akan digunakan untuk melakukan Transfer dari User/FrontEnd ke Controller. dan validasi untuk property yang required.
```bash
├── Models
│   ├── Requests
│   ├── ├──  User
│   ├── ─── ─── EditUserRequest.cs
│   ├── ─── ─── PagedUserRequest.cs
│   └── ─── ─── CreateUserRequest.cs
```
```C#    
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "The Name is required")]
        [DefaultValue("")]
        public string Name { get; set; } = null!;
        
        [Required(ErrorMessage = "The IsActive is required")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "The IsAdmin is required")]
        public bool IsAdmin { get; set; }
    }
```



###  Idams.WebApi\Models\Responses

Folder **Responses** digunakan untuk meyimpan semua property yang akan dikenal oleh FrontEnd. Folder **Responses** digunakan untuk meyimpan class yang akan digunakan untuk melakukan Transfer dari Controller User/FrontEnd .

```bash
├── Models
│   ├── Responses
│   ├── ├──  User
│   ├── ─── ─── PagedUserResponses.cs
```
```C#    
    public class PagedUserResponses
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public int Id { get; set; }
    }
```


## Structure Service Test Project
Pada Test project kita akan melakukan pemisahan berdasarkan Controller dan Service. Pastikan semua Test class turunan dari BaseControllerFactoryTest. dan untuk melakukan testing untuk validasi Request. Kita harus menggunakan Integration test (bisa di lihat pada sample di bawah) karena validasi di handle oleh .net core
```bash
├── Controllers
│   ├── UserControllerTest.cs
│   ├── RoleControllerTest.cs
├── Services
│   ├── UserServiceTest.cs
│   ├── RoleServiceTest.cs
├── Core
│   ├── BaseControllerFactoryTest.cs
│   ├── WebApiTesterFactory.cs


```

```C#    
    public class AuthControllerTest : BaseControllerFactoryTest
    {
        ```
        ```
        public AuthControllerTest(WebApiTesterFactory factory) : base(factory)
        {
          ```
          ```
        }
		// Integration Testing
        [Theory]
        [InlineData("/user/auth/login")]
        public async Task Post_Invalid_Request(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var content = new StringContent("{}", Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            var message = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("{\"code\":400,\"status\":\"Bad Request\",\"message\":[\"The Password field is required.\",\"The UserName field is required.\"]}", message);
        }
        
		// Unit Testing
        [Fact]
        public async Task Post_Login_Success()
        {
            // Arrange
            var loginRequest = new LoginRequest();            
            _configurationRepository.GetActiveDirectory().Returns(activeDirectories);
            _authenticationRepository.LoginAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(CreateFakeUser());
		
            //Act
            var result = await _service.LoginAsync(loginRequest.UserName, loginRequest.Password);

            //Assert
           ```
           ```
            _ = await _authenticationRepository.Received(1).LoginAsync(Arg.Any<string>(), Arg.Any<string>());
            Assert.Equal(LoginStateEnum.Success, result.Item1);
            ```
        }


       
    }
```


# How it works

## Create API
untuk membuat API baru, kita harus melakuakn beberap step, mungkin tidak semua nya sama karena mungkin kita menggunakan entity yang sudah ada, adapun step by step yg harus kita lakukan

1. Generate Model Entity dari table yang sudah dibuat (output akan ada pada folder "Generated Model")
2. Modifikasi Entity pada folder "Core\Models\Entities" Sesuai kebutuhan
4. Tambahkan Interface  untuk Repository pada folder "Core\Repositories"
4. Tambahkan  Implementasi dari Repository pada folder "Infrastructure.EntityFramework\Repositories"
5. Tambahkan Dto untuk Service pada folder "Core\Models\Dtos"
8. Tambahkan  Interface Service yang akan dibuat pada folder "Core\Services",  jika Service akan mengembalikan sebuath state (ex: UserNotFound, UserNotActive ) maka tambahakan EnumState pada Folder "Core\Enum"
9. Tambahkan Impementasi dari Service pada folder "Infrastucture\Services"


## Generate Entity dari Database

untuk melakukan generated model, buka Package Console Manager
``` bash
Scaffold-DbContext "Server={ConnectionString}" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir Models\Generated -Context {NameOfContext} -Force
```

Contoh

```bash
Scaffold-DbContext "user id=idam;password=Solecode1234*;Database=DB_PHE_Upstream;server=18.139.95.219;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Model/Entities -Context IdamsDbContext -Force
```