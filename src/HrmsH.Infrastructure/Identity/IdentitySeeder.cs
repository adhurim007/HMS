using HrmsH.Domain.Identity;
using HrmsH.Domain.Organization;
using HrmsH.Domain.Localization;
using HrmsH.Domain.Menus;
using HrmsH.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HrmsH.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<HrmsDbContext>();
        await context.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var roleName in SystemRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }
        }

        const string adminEmail = "admin@hrmsh.local";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var create = await userManager.CreateAsync(adminUser, "Admin123!");
            if (!create.Succeeded)
                throw new InvalidOperationException(string.Join("; ", create.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(adminUser, SystemRoles.SuperAdmin);
        }

        if (!await context.Hospitals.AnyAsync())
        {
            context.Hospitals.Add(new Hospital
            {
                Name = "Default Hospital",
                Code = "DEF-H",
                Address = "Main Street"
            });
            await context.SaveChangesAsync();
        }

        var defaultHospitalId = await context.Hospitals
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => x.Id)
            .FirstAsync();

        if (!await context.Facilities.AnyAsync())
        {
            context.Facilities.Add(new Facility
            {
                HospitalId = defaultHospitalId,
                Name = "Default Hospital",
                Code = "DEF",
                Address = "Main Street"
            });
            await context.SaveChangesAsync();
        }

        await SeedLocalizationAsync(context);
        await SeedMenusAsync(context);
    }

    private static async Task SeedLocalizationAsync(HrmsDbContext context)
    {
        if (!await context.Languages.AnyAsync())
        {
            context.Languages.AddRange(
                new Language
                {
                    Code = "en",
                    Name = "English",
                    IsDefault = true,
                    IsActive = true,
                },
                new Language
                {
                    Code = "sq",
                    Name = "Shqip",
                    IsDefault = false,
                    IsActive = true,
                });
            await context.SaveChangesAsync();
        }

        if (!await context.Translations.AnyAsync())
        {
            var en = new Dictionary<string, string>
            {
                // Layout / menu
                ["layout.brand"] = "HRMSH",
                ["app.tagline"] = "Hospital Management System",
                ["menu.dashboard"] = "Dashboard",
                ["menu.patients"] = "Patients",
                ["menu.appointments"] = "Appointments",
                ["menu.visits"] = "Visits",
                ["menu.doctors"] = "Doctors",
                ["menu.billing"] = "Billing",
                ["menu.pharmacy"] = "Pharmacy",
                ["menu.pharmacy.products"] = "Pharmacy Products",
                ["menu.pharmacy.stock"] = "Stock",
                ["menu.facilities"] = "Facilities",
                ["menu.departments"] = "Departments",
                ["menu.admin"] = "Administration",
                ["menu.menus"] = "Menu Management",
                ["menu.users"] = "Users",
                ["menu.localization"] = "Localization",

                // Auth
                ["auth.login.title"] = "Sign In",
                ["auth.login.welcomeTitle"] = "Welcome Back!",
                ["auth.login.subtitle"] = "Sign in to continue to HRMSH.",
                ["auth.login.email"] = "Email",
                ["auth.login.emailPlaceholder"] = "Enter email",
                ["auth.login.emailRequired"] = "Email is required",
                ["auth.login.emailInvalid"] = "Email must be valid",
                ["auth.login.password"] = "Password",
                ["auth.login.passwordPlaceholder"] = "Enter password",
                ["auth.login.passwordRequired"] = "Password is required",
                ["auth.login.submit"] = "Sign In",
                ["auth.login.loading"] = "Signing in...",
                ["auth.login.noAccount"] = "Don't have an account?",
                ["auth.login.goToDashboard"] = "Go to Dashboard",

                // Patients
                ["patients.title"] = "Patients",
                ["patients.list"] = "Patient List",
                ["patients.add"] = "Add Patient",
                ["patients.mrn"] = "MRN",
                ["patients.name"] = "Name",
                ["patients.dob"] = "Date of Birth",
                ["patients.gender"] = "Gender",
                ["patients.contact"] = "Contact",
                ["patients.address"] = "Address",

                // Patient billing
                ["patientBilling.title"] = "Patient Billing",
                ["patientBilling.totalInvoiced"] = "Total invoiced",
                ["patientBilling.totalPaid"] = "Total paid",
                ["patientBilling.outstanding"] = "Outstanding balance",
                ["patientBilling.invoices"] = "Invoices",
                ["patientBilling.goToBilling"] = "Go to Billing",
                ["patientBilling.addPayment"] = "Add payment",
                ["patientBilling.recordPayment"] = "Record payment",

                // Common
                ["common.cancel"] = "Cancel",
                ["common.save"] = "Save",

                // Admin / localization
                // Admin / menus
                ["admin.menus.title"] = "Menu management",
                ["admin.menus.menus"] = "Menus",
                ["admin.menus.newMenu"] = "New menu",
                ["admin.menus.editMenu"] = "Edit menu",
                ["admin.menus.menuName"] = "Menu name",
                ["admin.menus.menuKey"] = "Menu key",
                ["admin.menus.url"] = "Route path",
                ["admin.menus.parent"] = "Parent menu",
                ["admin.menus.noParent"] = "No parent (top level)",
                ["admin.menus.displayOrder"] = "Display order",
                ["admin.menus.icon"] = "Icon class",
                ["admin.menus.active"] = "Active",
                ["admin.menus.actions"] = "Actions",
                ["admin.menus.noMenus"] = "No menus defined yet.",
                ["admin.menus.savingMenu"] = "Saving menu...",
                ["admin.menus.roleAssignment"] = "Role menu assignment",
                ["admin.menus.selectRole"] = "Select role",
                ["admin.menus.selectRolePlaceholder"] = "Choose a role...",
                ["admin.menus.allowed"] = "Allowed",
                ["admin.menus.saveRoleMenus"] = "Save permissions",
                ["admin.menus.savingRoleMenus"] = "Saving permissions...",
                ["admin.menus.selectRoleHint"] =
                    "Select a role to configure which menus it can access.",
                ["admin.localization.title"] = "Localization",
                ["admin.localization.languages"] = "Languages",
                ["admin.localization.newLanguage"] = "New language",
                ["admin.localization.languageCode"] = "Language code",
                ["admin.localization.languageName"] = "Language name",
                ["admin.localization.default"] = "Default",
                ["admin.localization.active"] = "Active",
                ["admin.localization.savingLanguage"] = "Saving language...",
                ["admin.localization.translations"] = "Translations",
                ["admin.localization.searchTranslations"] = "Search key or value...",
                ["admin.localization.newTranslation"] = "New translation",
                ["admin.localization.selectLanguageHint"] =
                    "Select or create a language to manage its translations.",
                ["admin.localization.key"] = "Key",
                ["admin.localization.value"] = "Value",
                ["admin.localization.saveTranslation"] = "Save translation",
                ["admin.localization.savingTranslation"] = "Saving translation...",
                ["admin.localization.actions"] = "Actions",
                ["admin.localization.noTranslations"] = "No translations found for this language.",
            };

            var sq = new Dictionary<string, string>
            {
                // Layout / menu
                ["layout.brand"] = "HRMSH",
                ["app.tagline"] = "Sistemi i menaxhimit të spitalit",
                ["menu.dashboard"] = "Paneli",
                ["menu.patients"] = "Pacientët",
                ["menu.appointments"] = "Terminët",
                ["menu.visits"] = "Vizitat",
                ["menu.doctors"] = "Mjekët",
                ["menu.billing"] = "Faturimi",
                ["menu.pharmacy"] = "Farmacia",
                ["menu.pharmacy.products"] = "Produkte Farmacie",
                ["menu.pharmacy.stock"] = "Stoku",
                ["menu.facilities"] = "Institucionet",
                ["menu.departments"] = "Departamentet",
                ["menu.admin"] = "Administrimi",
                ["menu.menus"] = "Menutë",
                ["menu.users"] = "Përdoruesit",
                ["menu.localization"] = "Përkthimet",

                // Auth
                ["auth.login.title"] = "Hyrja",
                ["auth.login.welcomeTitle"] = "Mirë se u kthyet!",
                ["auth.login.subtitle"] = "Hyni për të vazhduar në HRMSH.",
                ["auth.login.email"] = "Email",
                ["auth.login.emailPlaceholder"] = "Shkruani emailin",
                ["auth.login.emailRequired"] = "Emaili është i detyrueshëm",
                ["auth.login.emailInvalid"] = "Emaili duhet të jetë i vlefshëm",
                ["auth.login.password"] = "Fjalëkalimi",
                ["auth.login.passwordPlaceholder"] = "Shkruani fjalëkalimin",
                ["auth.login.passwordRequired"] = "Fjalëkalimi është i detyrueshëm",
                ["auth.login.submit"] = "Hyr",
                ["auth.login.loading"] = "Duke u identifikuar...",
                ["auth.login.noAccount"] = "Nuk keni llogari?",
                ["auth.login.goToDashboard"] = "Shko te paneli",

                // Patients
                ["patients.title"] = "Pacientët",
                ["patients.list"] = "Lista e pacientëve",
                ["patients.add"] = "Shto pacient",
                ["patients.mrn"] = "Numri i kartelës",
                ["patients.name"] = "Emri",
                ["patients.dob"] = "Data e lindjes",
                ["patients.gender"] = "Gjinia",
                ["patients.contact"] = "Kontakti",
                ["patients.address"] = "Adresa",

                // Patient billing
                ["patientBilling.title"] = "Bilanci i pacientit",
                ["patientBilling.totalInvoiced"] = "Totali i faturuar",
                ["patientBilling.totalPaid"] = "Totali i paguar",
                ["patientBilling.outstanding"] = "Detyrimi i mbetur",
                ["patientBilling.invoices"] = "Faturat",
                ["patientBilling.goToBilling"] = "Shko te Faturimi",
                ["patientBilling.addPayment"] = "Shto pagesë",
                ["patientBilling.recordPayment"] = "Regjistro pagesën",

                // Common
                ["common.cancel"] = "Anulo",
                ["common.save"] = "Ruaj",

                // Admin / localization
                // Admin / menus
                ["admin.menus.title"] = "Menaxhimi i menuseve",
                ["admin.menus.menus"] = "Menutë",
                ["admin.menus.newMenu"] = "Meny e re",
                ["admin.menus.editMenu"] = "Përditëso menynë",
                ["admin.menus.menuName"] = "Emri i menysë",
                ["admin.menus.menuKey"] = "Çelësi i menysë",
                ["admin.menus.url"] = "Rruga (route)",
                ["admin.menus.parent"] = "Meny prind",
                ["admin.menus.noParent"] = "Pa prind (niveli kryesor)",
                ["admin.menus.displayOrder"] = "Renditja",
                ["admin.menus.icon"] = "Klasa e ikonës",
                ["admin.menus.active"] = "Aktive",
                ["admin.menus.actions"] = "Veprimet",
                ["admin.menus.noMenus"] = "Ende nuk ka meny të përcaktuara.",
                ["admin.menus.savingMenu"] = "Duke ruajtur menynë...",
                ["admin.menus.roleAssignment"] = "Caktimi i menuseve për rol",
                ["admin.menus.selectRole"] = "Zgjidh rolin",
                ["admin.menus.selectRolePlaceholder"] = "Zgjidh një rol...",
                ["admin.menus.allowed"] = "Lejohet",
                ["admin.menus.saveRoleMenus"] = "Ruaj lejet",
                ["admin.menus.savingRoleMenus"] = "Duke ruajtur lejet...",
                ["admin.menus.selectRoleHint"] =
                    "Zgjidh një rol për të konfiguruar cilat meny mund të shohë.",
                ["admin.localization.title"] = "Përkthimet",
                ["admin.localization.languages"] = "Gjuhët",
                ["admin.localization.newLanguage"] = "Gjuhë e re",
                ["admin.localization.languageCode"] = "Kodi i gjuhës",
                ["admin.localization.languageName"] = "Emri i gjuhës",
                ["admin.localization.default"] = "Parazgjedhur",
                ["admin.localization.active"] = "Aktive",
                ["admin.localization.savingLanguage"] = "Duke ruajtur gjuhën...",
                ["admin.localization.translations"] = "Përkthimet",
                ["admin.localization.searchTranslations"] = "Kërko sipas çelësit ose vlerës...",
                ["admin.localization.newTranslation"] = "Përkthim i ri",
                ["admin.localization.selectLanguageHint"] =
                    "Zgjidhni ose krijoni një gjuhë për të menaxhuar përkthimet e saj.",
                ["admin.localization.key"] = "Çelësi",
                ["admin.localization.value"] = "Vlera",
                ["admin.localization.saveTranslation"] = "Ruaj përkthimin",
                ["admin.localization.savingTranslation"] = "Duke ruajtur përkthimin...",
                ["admin.localization.actions"] = "Veprimet",
                ["admin.localization.noTranslations"] =
                    "Nuk u gjetën përkthime për këtë gjuhë.",
            };

            foreach (var pair in en)
            {
                context.Translations.Add(new Translation
                {
                    LanguageCode = "en",
                    Key = pair.Key,
                    Value = pair.Value,
                });
            }

            foreach (var pair in sq)
            {
                context.Translations.Add(new Translation
                {
                    LanguageCode = "sq",
                    Key = pair.Key,
                    Value = pair.Value,
                });
            }

            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedMenusAsync(HrmsDbContext context)
    {
        if (await context.Menus.AnyAsync())
        {
            return;
        }

        var dashboard = new Menu
        {
            Name = "Dashboard",
            MenuKey = "menu.dashboard",
            Url = "/dashboard",
            DisplayOrder = 1,
            Icon = "ri-dashboard-2-line",
            IsActive = true,
        };

        var patients = new Menu
        {
            Name = "Patients",
            MenuKey = "menu.patients",
            Url = "/patients",
            DisplayOrder = 2,
            Icon = "ri-user-3-line",
            IsActive = true,
        };

        var appointments = new Menu
        {
            Name = "Appointments",
            MenuKey = "menu.appointments",
            Url = "/appointments",
            DisplayOrder = 3,
            Icon = "ri-calendar-check-line",
            IsActive = true,
        };

        var visits = new Menu
        {
            Name = "Visits",
            MenuKey = "menu.visits",
            Url = "/visits",
            DisplayOrder = 4,
            Icon = "ri-hospital-line",
            IsActive = true,
        };

        var doctors = new Menu
        {
            Name = "Doctors",
            MenuKey = "menu.doctors",
            Url = "/doctors",
            DisplayOrder = 5,
            Icon = "ri-user-star-line",
            IsActive = true,
        };

        var billing = new Menu
        {
            Name = "Billing",
            MenuKey = "menu.billing",
            Url = "/billing",
            DisplayOrder = 6,
            Icon = "ri-file-list-3-line",
            IsActive = true,
        };

        var pharmacy = new Menu
        {
            Name = "Pharmacy",
            MenuKey = "menu.pharmacy",
            Url = null,
            DisplayOrder = 7,
            Icon = "ri-medicine-bottle-line",
            IsActive = true,
        };

        var pharmacyProducts = new Menu
        {
            Name = "Pharmacy Products",
            MenuKey = "menu.pharmacy.products",
            Url = "/pharmacy/products",
            Parent = pharmacy,
            DisplayOrder = 1,
            Icon = "ri-medicine-bottle-line",
            IsActive = true,
        };

        var pharmacyStock = new Menu
        {
            Name = "Stock",
            MenuKey = "menu.pharmacy.stock",
            Url = "/pharmacy/stock",
            Parent = pharmacy,
            DisplayOrder = 2,
            Icon = "ri-box-3-line",
            IsActive = true,
        };

        var admin = new Menu
        {
            Name = "Administration",
            MenuKey = "menu.admin",
            Url = null,
            DisplayOrder = 100,
            Icon = "ri-settings-3-line",
            IsActive = true,
        };

        var facilities = new Menu
        {
            Name = "Facilities",
            MenuKey = "menu.facilities",
            Url = "/admin/facilities",
            Parent = admin,
            DisplayOrder = 1,
            Icon = "ri-building-line",
            IsActive = true,
        };

        var departments = new Menu
        {
            Name = "Departments",
            MenuKey = "menu.departments",
            Url = "/admin/departments",
            Parent = admin,
            DisplayOrder = 2,
            Icon = "ri-apps-2-line",
            IsActive = true,
        };

        var menuManagement = new Menu
        {
            Name = "Menu Management",
            MenuKey = "menu.menus",
            Url = "/admin/menus",
            Parent = admin,
            DisplayOrder = 3,
            Icon = "ri-settings-3-line",
            IsActive = true,
        };

        var users = new Menu
        {
            Name = "Users",
            MenuKey = "menu.users",
            Url = "/admin/users",
            Parent = admin,
            DisplayOrder = 4,
            Icon = "ri-team-line",
            IsActive = true,
        };

        var localization = new Menu
        {
            Name = "Localization",
            MenuKey = "menu.localization",
            Url = "/admin/localization",
            Parent = admin,
            DisplayOrder = 5,
            Icon = "ri-translate-2",
            IsActive = true,
        };

        context.Menus.AddRange(
            dashboard,
            patients,
            appointments,
            visits,
            doctors,
            billing,
            pharmacy,
            pharmacyProducts,
            pharmacyStock,
            admin,
            facilities,
            departments,
            menuManagement,
            users,
            localization);

        await context.SaveChangesAsync();

        if (!await context.RoleMenus.AnyAsync())
        {
            var menuIds = await context.Menus
                .AsNoTracking()
                .Select(m => m.Id)
                .ToListAsync();

            var roleIds = await context.Roles
                .AsNoTracking()
                .Select(r => r.Id)
                .ToListAsync();

            foreach (var roleId in roleIds)
            {
                foreach (var menuId in menuIds)
                {
                    context.RoleMenus.Add(new RoleMenu
                    {
                        RoleId = roleId,
                        MenuId = menuId,
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}

