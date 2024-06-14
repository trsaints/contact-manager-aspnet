using ContactManager.Authorization;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Pages.Contacts
{
    public class IndexModel : DI_BasePageModel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IndexModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public IList<Contact> Contact { get;set; }

        public async Task OnGetAsync()
        {
            var contacts = from c in Context.Contact select c;
            var isAuthorized = User.IsInRole(Constants.ContactManagersRole) || User.IsInRole(Constants.ContactAdministratorsRole);
            var currendId = UserManager.GetUserId(User);
            
            if (!isAuthorized)
                contacts = contacts.Where(c => c.Status == ContactStatus.Approved || c.OwnerID == currendId);
            
            Contact = await contacts.ToListAsync();
        }
    }
}
