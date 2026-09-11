using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Academy2.Data;

namespace Academy2.Pages.DisciplinesPages;

public class EditModel : PageModel
{
    private readonly Academy2Context _context;

    public EditModel(Academy2Context context)
    {
        _context = context;
    }

    [BindProperty]
    public Disciplines Disciplines { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(short? disciplineid)
    {
        if (disciplineid is null)
        {
            return NotFound();
        }

        var disciplines = await _context.Disciplines.FirstOrDefaultAsync(m => m.DisciplineId == disciplineid);
        if (disciplines is null)
        {
            return NotFound();
        }
        Disciplines = disciplines;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Disciplines).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DisciplinesExists(Disciplines.DisciplineId))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool DisciplinesExists(short disciplineid)
    {
        return _context.Disciplines.Any(e => e.DisciplineId == disciplineid);
    }
}
