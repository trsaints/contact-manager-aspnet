using ContactManager.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ContactManager.Pages.Contacts
{
    public class DetailsModel : DI_BasePageModel
    {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DetailsModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<IdentityUser> userManager) : base (context, authorizationService, userManager)
        {
        }

        public Contact Contact { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var _contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);
            
            if (_contact is null)
                return NotFound();
            
            Contact = _contact;

            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole) ||
                               User.IsInRole(Constants.ContactManagersRole);
            
            var currentUserId = UserManager.GetUserId(User);
            
            if (!isAuthorized
                && currentUserId != Contact.OwnerID
                && Contact.Status != ContactStatus.Approved)
                return Forbid();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, ContactStatus status)
        {
            var contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);

            if (contact is null)
                return NotFound();
            
            var contactOperation = (status == ContactStatus.Approved) ? ContactOperations.Approve : ContactOperations.Reject;
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, contact, contactOperation);
            
            if (!isAuthorized.Succeeded)
                return Forbid();
            
            contact.Status = status;
            Context.Contact.Update(contact);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
