using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shoping.Data;

namespace Shoping.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _context;

        //inyectando la base de datos
        //las inyecciones se hacen en el contructor de la clase
        public CombosHelper(DataContext context)
        {
            _context = context;
        }


        public async  Task<IEnumerable<SelectListItem>> GetComboCategoriesAsync()
        {
            //se convierte una lista en lista de select items usando select
            List<SelectListItem> list = await _context.Categories.
                                            Select(c => new SelectListItem {
                                                Text = c.Name,
                                                Value = c.Id.ToString()
                                            }).OrderBy(c => c.Text)
                                            .ToListAsync();

            //insertando un objeto en la posicion cero que diga seleccionar
            list.Insert(0, new SelectListItem { Text = "Seleccione una categoria ...", Value = "0" });

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboCitiesAsync(int stateId)
        {
            //se convierte una lista en lista de select items usando select
            //verificar que el where se hace antes del select ya que si se hace despues afecta a la selectitem list
            List<SelectListItem> list = await _context.Cities.Where(s => s.State.Id == stateId).
                                            Select(c => new SelectListItem
                                            {
                                                Text = c.Name,
                                                Value = c.Id.ToString()
                                            })
                                            .OrderBy(c => c.Text)
                                            .ToListAsync();

            //insertando un objeto en la posicion cero que diga seleccionar
            list.Insert(0, new SelectListItem { Text = "Seleccione una Ciudad ...", Value = "0" });

            return list;

        }

        public async Task<IEnumerable<SelectListItem>> GetComboCountriesAsync()
        {
            //se convierte una lista en lista de select items usando select
            List<SelectListItem> list = await _context.Countries.
                                            Select(c => new SelectListItem
                                            {
                                                Text = c.Name,
                                                Value = c.Id.ToString()
                                            }).OrderBy(c => c.Text)
                                            .ToListAsync();

            //insertando un objeto en la posicion cero que diga seleccionar
            list.Insert(0, new SelectListItem { Text = "Seleccione un Pais ...", Value = "0" });

            return list;

        }

        public async Task<IEnumerable<SelectListItem>> GetComboStatesAsync(int countryId)
        {
            //se convierte una lista en lista de select items usando select
            //verificar que el where se hace antes del select ya que si se hace despues afecta a la selectitem list
            List<SelectListItem> list = await _context.States.Where(s => s.Country.Id == countryId).
                                            Select(c => new SelectListItem
                                            {
                                                Text = c.Name,
                                                Value = c.Id.ToString()
                                            })
                                            .OrderBy(c => c.Text)
                                            .ToListAsync();

            //insertando un objeto en la posicion cero que diga seleccionar
            list.Insert(0, new SelectListItem { Text = "Seleccione un Estado ...", Value = "0" });

            return list;

        }
    }
}
