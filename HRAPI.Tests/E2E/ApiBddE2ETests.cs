using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace HRAPI.Tests.E2E;

public class ApiBddE2ETests : IAsyncLifetime
{
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri(Environment.GetEnvironmentVariable("HRAPI_E2E_BASE_URL") ?? "http://localhost:5065")
    };

    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
    private HttpResponseMessage? _lastResponse;
    private HttpResponseMessage? _secondResponse;
    private string? _adminToken;
    private string? _userToken;

    public Task InitializeAsync() => Task.CompletedTask;

    private void DisposeResources()
    {
        _lastResponse?.Dispose();
        _secondResponse?.Dispose();
        _client.Dispose();
    }

    public Task DisposeAsync()
    {
        DisposeResources();
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Authentication: Admin can log in successfully")]
    public async Task AdminCanLogInSuccessfully()
    {
        await GivenTheBackendIsRunning();
        await WhenTheAdminLogsInWithValidCredentials();
        ThenTheApiReturnsAJwtToken(_adminToken);
        await ThenProtectedEndpointsCanBeAccessedWithTheToken();
    }

    [Fact(DisplayName = "Department management: Admin can create and retrieve a department")]
    public async Task AdminCanCreateAndRetrieveDepartment()
    {
        await GivenAnAuthenticatedAdminUser();
        var departmentId = await WhenANewDepartmentIsCreated();
        ThenTheResponseStatusIs(HttpStatusCode.Created);
        await ThenTheResourceCanBeRetrievedFromTheApi($"/api/departments/{departmentId}");
    }

    [Fact(DisplayName = "Position management: Admin can create a position for an existing department")]
    public async Task AdminCanCreatePositionForExistingDepartment()
    {
        await GivenAnAuthenticatedAdminUser();
        var departmentId = await GivenAnExistingDepartment();
        var positionId = await WhenANewPositionIsCreatedForThatDepartment(departmentId);
        ThenTheResponseStatusIs(HttpStatusCode.Created);
        await ThenTheResourceCanBeRetrievedFromTheApi($"/api/positions/{positionId}");
    }

    [Fact(DisplayName = "Employee management: Admin can create an employee")]
    public async Task AdminCanCreateEmployee()
    {
        await GivenAnAuthenticatedAdminUser();
        var departmentId = await GivenAnExistingDepartment();
        var positionId = await GivenAnExistingPosition(departmentId);
        var employeeId = await WhenANewEmployeeIsCreated(departmentId, positionId);
        ThenTheResponseStatusIs(HttpStatusCode.Created);
        await ThenTheResourceCanBeRetrievedFromTheApi($"/api/employees/{employeeId}");
    }

    [Fact(DisplayName = "Leave request management: End date before start date is rejected")]
    public async Task LeaveRequestCannotHaveEndDateBeforeStartDate()
    {
        await GivenAnAuthenticatedUser();
        var departmentId = await GivenAnExistingDepartment();
        var positionId = await GivenAnExistingPosition(departmentId);
        var employeeId = await GivenAnExistingEmployee(departmentId, positionId);
        var leaveTypeId = await GivenAnExistingLeaveType();
        await WhenALeaveRequestIsSubmittedWithAnEndDateBeforeTheStartDate(employeeId, leaveTypeId);
        ThenTheResponseStatusIs(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Attendance management: Duplicate attendance for same employee and date is rejected")]
    public async Task AttendanceCannotBeDuplicatedForSameEmployeeAndDate()
    {
        await GivenAnAuthenticatedAdminUser();
        var departmentId = await GivenAnExistingDepartment();
        var positionId = await GivenAnExistingPosition(departmentId);
        var employeeId = await GivenAnExistingEmployee(departmentId, positionId);
        await WhenAnAttendanceRecordIsCreatedForADate(employeeId);
        ThenTheResponseStatusIs(HttpStatusCode.Created);
        await WhenAnotherAttendanceRecordIsCreatedForTheSameEmployeeAndDate(employeeId);
        ThenTheSecondResponseStatusIs(HttpStatusCode.BadRequest);
    }

    private async Task GivenTheBackendIsRunning()
    {
        using var response = await _client.GetAsync("/swagger/index.html");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task GivenAnAuthenticatedAdminUser()
    {
        _adminToken ??= await LoginAndGetToken("admin@hr.com", "Admin123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);
    }

    private async Task GivenAnAuthenticatedUser()
    {
        _userToken ??= await LoginAndGetToken("admin@hr.com", "Admin123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userToken);
    }

    private async Task WhenTheAdminLogsInWithValidCredentials()
    {
        _adminToken = await LoginAndGetToken("admin@hr.com", "Admin123!");
    }

    private static void ThenTheApiReturnsAJwtToken(string? token)
    {
        token.Should().NotBeNullOrWhiteSpace();
        token!.Split('.').Should().HaveCount(3);
    }

    private async Task ThenProtectedEndpointsCanBeAccessedWithTheToken()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);
        using var response = await _client.GetAsync("/api/auth/me");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<int> GivenAnExistingDepartment() => await CreateDepartment();

    private async Task<int> GivenAnExistingPosition(int departmentId) => await CreatePosition(departmentId);

    private async Task<int> GivenAnExistingEmployee(int departmentId, int positionId) =>
        await CreateEmployee(departmentId, positionId);

    private async Task<int> GivenAnExistingLeaveType()
    {
        using var response = await _client.GetAsync("/api/leavetypes");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var document = await ReadJson(response);
        var firstLeaveType = document.RootElement.GetProperty("items").EnumerateArray().FirstOrDefault();
        firstLeaveType.ValueKind.Should().NotBe(JsonValueKind.Undefined);
        return firstLeaveType.GetProperty("id").GetInt32();
    }

    private async Task<int> WhenANewDepartmentIsCreated() => await CreateDepartment();

    private async Task<int> WhenANewPositionIsCreatedForThatDepartment(int departmentId) =>
        await CreatePosition(departmentId);

    private async Task<int> WhenANewEmployeeIsCreated(int departmentId, int positionId) =>
        await CreateEmployee(departmentId, positionId);

    private async Task WhenALeaveRequestIsSubmittedWithAnEndDateBeforeTheStartDate(int employeeId, int leaveTypeId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(30));
        _lastResponse?.Dispose();
        _lastResponse = await _client.PostAsJsonAsync("/api/leaverequests", new
        {
            employeeId,
            leaveTypeId,
            startDate = today.ToString("yyyy-MM-dd"),
            endDate = today.AddDays(-1).ToString("yyyy-MM-dd"),
            reason = "BDD negative date validation"
        });
    }

    private async Task WhenAnAttendanceRecordIsCreatedForADate(int employeeId)
    {
        _lastResponse?.Dispose();
        _lastResponse = await _client.PostAsJsonAsync("/api/attendances", CreateAttendancePayload(employeeId));
    }

    private async Task WhenAnotherAttendanceRecordIsCreatedForTheSameEmployeeAndDate(int employeeId)
    {
        _secondResponse?.Dispose();
        _secondResponse = await _client.PostAsJsonAsync("/api/attendances", CreateAttendancePayload(employeeId));
    }

    private void ThenTheResponseStatusIs(HttpStatusCode expectedStatusCode)
    {
        _lastResponse.Should().NotBeNull();
        _lastResponse!.StatusCode.Should().Be(expectedStatusCode);
    }

    private void ThenTheSecondResponseStatusIs(HttpStatusCode expectedStatusCode)
    {
        _secondResponse.Should().NotBeNull();
        _secondResponse!.StatusCode.Should().Be(expectedStatusCode);
    }

    private async Task ThenTheResourceCanBeRetrievedFromTheApi(string path)
    {
        using var response = await _client.GetAsync(path);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var document = await ReadJson(response);
        document.RootElement.TryGetProperty("data", out _).Should().BeTrue();
    }

    private async Task<string> LoginAndGetToken(string email, string password)
    {
        using var response = await _client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var document = await ReadJson(response);
        return document.RootElement.GetProperty("token").GetString()
            ?? throw new InvalidOperationException("Login response did not include a token.");
    }

    private async Task<int> CreateDepartment()
    {
        var suffix = UniqueSuffix();
        _lastResponse?.Dispose();
        _lastResponse = await _client.PostAsJsonAsync("/api/departments", new
        {
            name = $"BDD Department {suffix}",
            code = $"BDD{suffix}",
            description = "Created by BDD E2E tests",
            parentDepartmentId = (int?)null
        });

        return await ExtractCreatedId(_lastResponse);
    }

    private async Task<int> CreatePosition(int departmentId)
    {
        var suffix = UniqueSuffix();
        _lastResponse?.Dispose();
        _lastResponse = await _client.PostAsJsonAsync("/api/positions", new
        {
            title = $"BDD Position {suffix}",
            description = "Created by BDD E2E tests",
            minSalary = 1000m,
            maxSalary = 2000m,
            departmentId
        });

        return await ExtractCreatedId(_lastResponse);
    }

    private async Task<int> CreateEmployee(int departmentId, int positionId)
    {
        var suffix = UniqueSuffix();
        _lastResponse?.Dispose();
        _lastResponse = await _client.PostAsJsonAsync("/api/employees", new
        {
            employeeNumber = $"BDD-{suffix}",
            firstName = "BDD",
            lastName = $"Employee{suffix}",
            email = $"bdd.employee.{suffix}@example.com",
            phone = "5550100",
            dateOfBirth = "1990-01-01",
            hireDate = DateOnly.FromDateTime(DateTime.UtcNow.Date).ToString("yyyy-MM-dd"),
            terminationDate = (string?)null,
            address = "BDD Street 1",
            city = "Vilnius",
            state = "Vilnius",
            postalCode = "00000",
            country = "Lithuania",
            departmentId,
            positionId,
            managerId = (int?)null
        });

        return await ExtractCreatedId(_lastResponse);
    }

    private object CreateAttendancePayload(int employeeId)
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(120));
        return new
        {
            employeeId,
            date = date.ToString("yyyy-MM-dd"),
            checkIn = "09:00:00",
            checkOut = "17:00:00",
            status = 0,
            notes = "BDD duplicate validation"
        };
    }

    private async Task<int> ExtractCreatedId(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        using var document = await ReadJson(response);
        return document.RootElement.GetProperty("data").GetProperty("id").GetInt32();
    }

    private async Task<JsonDocument> ReadJson(HttpResponseMessage response)
    {
        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonDocument.ParseAsync(stream, new JsonDocumentOptions(), default);
    }

    private static string UniqueSuffix() => Guid.NewGuid().ToString("N")[..10];
}
