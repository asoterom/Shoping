#nullable disable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoping.Data;
using Shoping.Data.Entities;
using Shoping.Models;

namespace Shoping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CountriesController : Controller
    {
        private readonly DataContext _context;

        public CountriesController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {


            return View(await _context.Countries.Include(c => c.States).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .Include(c => c.States)
                .ThenInclude(s => s.Cities)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        public IActionResult Create()
        {
            Country country = new()
            {
                States = new List<State>(),
            };

            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(country);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un país con el mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return View(country);

        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .Include(c => c.States)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Country country)
        {
            if (id != country.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un país con el mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return View(country);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .Include(c => c.States)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        #region Estado
        
        public async Task<IActionResult> AddState(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            StateViewModel stateViewModel = new StateViewModel
            {
                CountryId = country.Id
            };

            return View(stateViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddState(StateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    State state = new()
                    {
                        Cities = new List<City>(),
                        Country = await _context.Countries.FindAsync(model.CountryId),
                        Name = model.Name,
                    };
                    
                    _context.Add(state);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details),new {Id = model.CountryId});

                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un departamento/estado con el mismo nombre en este pais.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return View(model);

        }


        public async Task<IActionResult> EditState(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var state = await _context.States
                .Include(s => s.Country)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (state == null)
            {
                return NotFound();
            }

            StateViewModel model = new()
            {
                CountryId = state.Country.Id,
                Name = state.Name,
                Id = state.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditState(int id, StateViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    State state = new State()
                    {
                        Id = model.Id,
                        Name = model.Name
                    };

                    _context.Update(state);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new {Id = model.CountryId });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un departamento/estado con el mismo nombre en este pais.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return View(model);
        }


        public async Task<IActionResult> DetailsState(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var state = await _context.States
                .Include(s => s.Country)
                .Include(s => s.Cities)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (state == null)
            {
                return NotFound();
            }

            return View(state);
        }

        public async Task<IActionResult> DeleteState(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var state = await _context.States
                .Include(c => c.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (state == null)
            {
                return NotFound();
            }

            return View(state);
        }

        [HttpPost, ActionName("DeleteState")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStateConfirmed(int id)
        {
            var state = await _context.States
                .Include(s => s.Country)
                .FirstOrDefaultAsync(s => s.Id == id);

            _context.States.Remove(state);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details),new {id = state.Country.Id });
        }


        #endregion

        #region City
        public async Task<IActionResult> AddCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var state = await _context.States.FindAsync(id);
            if (state == null)
            {
                return NotFound();
            }
            CityViewModel cityViewModel = new CityViewModel
            {
                StateId = state.Id
            };

            return View(cityViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCity(CityViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    City city = new()
                    {
                        State = await _context.States.FindAsync(model.StateId),
                        Name = model.Name,
                    };

                    _context.Add(city);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(DetailsState), new { Id = model.StateId });

                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una ciudad con el mismo nombre en este departamento/estado.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return View(model);

        }

        public async Task<IActionResult> EditCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.State)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            CityViewModel model = new()
            {
                StateId = city.State.Id,
                Name = city.Name,
                Id = city.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCity(int id, CityViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    City city = new City()
                    {
                        Id = model.Id,
                        Name = model.Name
                    };

                    _context.Update(city);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(DetailsState), new { Id = model.StateId });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una ciudad con el mismo nombre en este departamento/estado.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return View(model);
        }
        public async Task<IActionResult> DetailsCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.State)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }
        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.State)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        [HttpPost, ActionName("DeleteCity")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCityConfirmed(int id)
        {
            var city = await _context.Cities
                .Include(s => s.State)
                .FirstOrDefaultAsync(c => c.Id == id);

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DetailsState), new { id = city.State.Id });
        }

        #endregion

    }
}
