using BibliotekaSzkolnaAI.API.Data;
using Microsoft.AspNetCore.Identity;

public static class UserInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // --- Testowy Admin ---
        string adminEmail = "mmickiewicz@sp123testy.edu.pl";
        string adminPassword = "adm123";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Michał",
                LastName = "Mickiewicz",
                UserType = "pracownik",
                LibraryId = 10001,
                Pesel = "90010112345"
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // --- Testowy Bibliotekarz 1 ---
        string librarianEmail = "jkowalska@sp123testy.edu.pl";
        string librarianPassword = "lib123";
        if (await userManager.FindByEmailAsync(librarianEmail) == null)
        {
            var librarianUser = new ApplicationUser
            {
                UserName = librarianEmail,
                Email = librarianEmail,
                EmailConfirmed = true,
                FirstName = "Janina",
                LastName = "Kowalska",
                UserType = "pracownik",
                LibraryId = 20001,
                Pesel = "85050554321"
            };

            var result = await userManager.CreateAsync(librarianUser, librarianPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(librarianUser, "Librarian");
            }
        }

        // --- Testowy Użytkownik 1 - Uczeń---
        string userEmail1 = "71345@uczen.sp123testy.edu.pl";
        string userPassword1 = "use123";
        if (await userManager.FindByEmailAsync(userEmail1) == null)
        {
            var regularUser = new ApplicationUser
            {
                UserName = userEmail1,
                Email = userEmail1,
                EmailConfirmed = true,
                FirstName = "Anna",
                LastName = "Nowak",
                UserType = "uczeń",
                Class = "8A",
                LibraryId = 30001,
                Pesel = "11260267890"
            };

            var result = await userManager.CreateAsync(regularUser, userPassword1);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(regularUser, "User");
            }
        }

        // --- Testowy Użytkownik 2 - Nauczyciel---
        string userEmail2 = "kbluszcz@sp123testy.edu.pl";
        string userPassword2 = "use123";
        if (await userManager.FindByEmailAsync(userEmail2) == null)
        {
            var regularUser = new ApplicationUser
            {
                UserName = userEmail2,
                Email = userEmail2,
                EmailConfirmed = true,
                FirstName = "Krystyna",
                LastName = "Bluszcz",
                UserType = "pracownik",
                LibraryId = 20002,
                Pesel = "94060267890"
            };

            var result = await userManager.CreateAsync(regularUser, userPassword2);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(regularUser, "User");
            }
        }
    }
}
// uczniowie - format maila to ID ucznia z systemu szkoły (tutaj tylko przykładowe) + @uczen.sp123testy.edu.pl, ID=LibraryId zaczynające się od 5....
// pracownicy - format maila to pierwsza litera imienia + nazwisko + @sp123testy.edu.pl, ID=LibraryId zaczynające się od 2....