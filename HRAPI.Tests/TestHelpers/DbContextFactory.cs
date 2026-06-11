using HRAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Tests.TestHelpers;

public static class DbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"HRForWS-Test-{Guid.NewGuid()}")
            .Options;

        return new AppDbContext(options);
    }
}
