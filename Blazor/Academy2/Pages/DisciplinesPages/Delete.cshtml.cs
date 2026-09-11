using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Academy2.Data;

namespace Academy2.Pages.DisciplinesPages;

public class DeleteModel : PageModel
{
    private readonly Academy2Context _context;

    public DeleteModel(Academy2Context context)
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
        else
        {
            Disciplines = disciplines;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(short? disciplineid)
    {
        if (disciplineid is null)
        {
            return NotFound();
        }

        var disciplines = await _context.Disciplines.FindAsync(disciplineid);
        if (disciplines != null)
        {
            Disciplines = disciplines;
            _context.Disciplines.Remove(Disciplines);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
